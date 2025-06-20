using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BeardedManStudios;
using Dan.Net.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using HybridWebSocket;

using static Dan.Net.Globals;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// Invoked when a connection to the server is established.
        /// </summary>
        public static event System.Action OnConnected;
        
        /// <summary>
        /// Invoked when disconnected from the server.
        /// </summary>
        public static event System.Action OnDisconnected;
        
        /// <summary>
        /// Invoked when a Room is successfully created.
        /// </summary>
        public static event System.Action<Room> OnRoomCreated;
        
        /// <summary>
        /// Invoked when a player successfully joins a Room.
        /// </summary>
        public static event System.Action<Room> OnJoinedRoom;
        
        /// <summary>
        /// Returns true if the player has established a connection to the DanNet server.
        /// </summary>
        public static bool IsConnected => _webSocket != null && _webSocket.GetState() == WebSocketState.Open;

        /// <summary>
        /// The current Room the player is in.
        /// </summary>
        public static Room CurrentRoom { get; private set; }

        /// <summary>
        /// Returns true if the player is the master client of the current Room.
        /// </summary>
        public static bool IsMasterClient => CurrentRoom.creatorId == PlayerID;

        public static bool IsStreamEnabled { get; set; } = true;

        /// <summary>
        /// When true, server messages will be logged.
        /// </summary>
        public static bool IsLoggingEnabled { get; set; } = true;
        
        public static long Ping { get; private set; }
        
        /// <summary>
        /// The prefix that will be assigned to a player's name if it is not provided.
        /// </summary>
        public static string RandomPlayerNamePrefix { get; set; } = "Player_";

        internal static string PlayerID { get; private set; }
        
        private static WebSocket _webSocket;
        private static JsonSerializer _jsonSerializer = new JsonSerializer
        {
            FloatParseHandling = FloatParseHandling.Double
        };
        
        private static List<string> _messageBatch = new List<string>();
        private static uint _currentMessageBatchSize;
        private const uint MAX_MESSAGE_BATCH_SIZE = 512;
        private const float BATCH_INTERVAL = 0.05f;

        private static Dictionary<string, System.Action<SyncObject, DanNetEvent>> _danNetEventHandlers;
        private static Dictionary<string, GameObject> _prefabCache;
        
        private static IReadOnlyDictionary<string, System.Action<string>> _messageHandlers = 
            new Dictionary<string, System.Action<string>>
        {
            { STREAM_EVENT_TYPE,              OnStreamMessage },
            { INSTANTIATE_EVENT_TYPE,         OnInstantiateMessage },
            { JOINED_ROOM_EVENT_TYPE,         OnJoinedRoomMessage },
            { LEFT_ROOM_EVENT_TYPE,           OnLeftRoomMessage },
            { SYNC_OBJECTS_EVENT_TYPE,        OnSyncObjectsMessage },
            { DESTROY_EVENT_TYPE,             OnDestroyMessage },
            { UPDATE_ROOM_EVENT_TYPE,         OnUpdateRoomMessage },
            { PONG_EVENT_TYPE,                OnPongMessage },
            { DAN_NET_EVENT_TYPE_NORMAL,      OnDanNetEventMessage },
            { DAN_NET_EVENT_TYPE_SERVER_SYNC, OnDanNetEventMessage },
            { DAN_NET_EVENT_TYPE_BUFFERED,    OnDanNetEventMessage }
        };

        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize()
        {
            const string danNetObjectName = "[DanNet]";
            _danNetEventHandlers = new Dictionary<string, System.Action<SyncObject, DanNetEvent>>();
            
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(assembly => assembly.GetTypes()).ToArray();
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var attr = method.GetCustomAttribute<DanNetEventAttribute>();
                    if (attr != null)
                    {
                        CacheDanNetEvent(method, type);
                    }
                }
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            var danNet = new GameObject(danNetObjectName, typeof(MainThreadManager), typeof(SyncObjectManager), typeof(StreamManager));
            Object.DontDestroyOnLoad(danNet);

            Application.quitting += OnApplicationQuit;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += OnApplicationQuit;
#endif

            InitializePrefabCache();
        }

        /// <summary>
        /// Connects to the DanNet server.
        /// </summary>
        /// <param name="username">The player's username (optional).</param>
        public static void Connect(string username = null)
        {
            username ??= $"{RandomPlayerNamePrefix}{Random.Range(0, 10000):0000}";
            var request = UnityWebRequest.Post(GetServerUrl(Route.Connect), new List<IMultipartFormSection>
            {
                new MultipartFormDataSection(ROUTE_CONNECT_NAME, username),
            });
            request.Handle(isSuccessful =>
            {
                if (!isSuccessful)
                {
                    Logger.Log("Error while trying to connect!", Logger.LogType.Error);
                    return;
                }
                var response = Deserialize<ConnectResponse>(request.downloadHandler.text);
                PlayerID = response.playerId;
                OnConnected?.Invoke();
            });
        }
        
        /// <summary>
        /// Spawns a prefab in the network.
        /// </summary>
        /// <param name="prefabName">The prefab's name in the Resources folder.</param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            if (string.IsNullOrEmpty(prefabName) || !_prefabCache.ContainsKey(prefabName))
            {
                Logger.Log("Sync object prefab not found!", Logger.LogType.Error);
                return;
            }

            SendMessage(new Message(INSTANTIATE_EVENT_TYPE, new InstantiationResponse(prefabName, position, rotation)));
        }
        
        /// <summary>
        /// Destroy a network prefab.
        /// </summary>
        /// <param name="gameObject">The network object (must have the SyncObject component).</param>
        public static void Destroy(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<SyncObject>(out var syncObject))
            {
                Logger.Log("Prefab does not have a SyncObject component!", Logger.LogType.Error);
                return;
            }

            SendMessage(new Message(DESTROY_EVENT_TYPE, new DestroyResponse(syncObject.ID)));
        }

        /// <summary>
        /// Creates a room in the DanNet server.
        /// </summary>
        /// <param name="roomName">Name of the room to be created.</param>
        /// <param name="maxPlayers">Max player count (defaulted to 2).</param>
        public static void CreateRoom(string roomName, int maxPlayers = 2)
        {
            if (string.IsNullOrEmpty(PlayerID))
            {
                Logger.Log("You must connect to the DanNet server before creating a room!", Logger.LogType.Error);
                return;
            }
            var request = UnityWebRequest.Post(GetServerUrl(Route.CreateRoom), new List<IMultipartFormSection>
            {
                new MultipartFormDataSection(ROUTE_CREATE_ROOM_ROOM_NAME, roomName),
                new MultipartFormDataSection(ROUTE_CREATE_ROOM_PLAYER_ID, PlayerID),
                new MultipartFormDataSection(ROUTE_CREATE_ROOM_MAX_PLAYERS, maxPlayers.ToString())
            });
            request.Handle(isSuccessful =>
            {
                if (!isSuccessful)
                {
                    Logger.Log("Error while trying to create room!", Logger.LogType.Error);
                    return;
                }
                
                CurrentRoom = Deserialize<Room>(request.downloadHandler.text);
                OnRoomCreated?.Invoke(CurrentRoom);
            });
        }
        
        /// <summary>
        /// Joins the player to a room with a given name in the DanNet server.
        /// </summary>
        /// <param name="roomName">Name of the room to be joined into.</param>
        public static void JoinRoom(string roomName)
        {
            if (string.IsNullOrEmpty(PlayerID))
            {
                Logger.Log("You must connect to the DanNet server before joining a room!", Logger.LogType.Error);
                return;
            }

            AttemptConnection(3);
            return;
            
            void AttemptConnection(int retriesLeft)
            {
                MainThreadManager.RunRepeated(BatchSendMessages, BATCH_INTERVAL);
                _webSocket = WebSocketFactory.CreateInstance(GetWebsocketUrl(Route.JoinRoom,
                    (ROUTE_JOIN_ROOM_ROOM_NAME, roomName),
                    (ROUTE_JOIN_ROOM_PLAYER_ID, PlayerID)));

                _webSocket.OnMessage += OnWebSocketMessage;
                _webSocket.OnClose += code =>
                {
                    Logger.Log("Websocket closed with code: " + code);
                    OnDisconnected?.Invoke();

                    if (retriesLeft > 0)
                    {
                        Logger.Log("Retrying connection...", Logger.LogType.Warning);
                        MainThreadManager.RunDelayed(() => AttemptConnection(retriesLeft - 1), 2f);
                    }
                };

                _webSocket.Connect();
            }
        }
        
        /// <summary>
        /// Creates or joins a room with the given room name.
        /// </summary>
        /// <param name="roomName">Name of the room to create or join into (if it exists).</param>
        /// <param name="maxPlayers">The max player count in the room (min. 2) (default: 2).</param>
        public static void CreateOrJoinRoom(string roomName, int maxPlayers = 2)
        {
            if (string.IsNullOrEmpty(PlayerID))
            {
                Logger.Log("You must connect to the DanNet server before creating a room!", Logger.LogType.Error);
                return;
            }
            
            GetRoomList(rooms =>
            {
                if (rooms == null || rooms.Count == 0)
                {
                    CreateRoom(roomName, maxPlayers);
                    return;
                }
                
                // If no room with the given name exists, create one.
                if (rooms.All(r => r.name != roomName))
                {
                    CreateRoom(roomName, maxPlayers);
                    return;
                }
                
                JoinRoom(roomName);
            });
        }
        
        /// <summary>
        /// Returns a list of rooms that are currently in the server.
        /// </summary>
        /// <param name="roomsCallback">Returns a list of currently open rooms.</param>
        public static void GetRoomList(System.Action<List<Room>> roomsCallback)
        {
            if (string.IsNullOrEmpty(PlayerID))
            {
                Logger.Log("You must connect to the DanNet server before getting the room list!", Logger.LogType.Error);
                roomsCallback.Invoke(null);
                return;
            }

            var request = UnityWebRequest.Get(GetServerUrl(Route.GetRoomList));
            request.Handle(isSuccessful =>
            {
                if (!isSuccessful)
                {
                    Logger.Log("Error while trying to get the room list!", Logger.LogType.Error);
                    return;
                }
                
                var response = Deserialize<List<Room>>(request.downloadHandler.text);
                roomsCallback.Invoke(response);
            });
        }

        /// <summary>
        /// Leaves the room the player is currently in.
        /// </summary>
        public static void LeaveRoom()
        {
            if (CurrentRoom is null)
            {
                Logger.Log("Attempted to leave a room when not in one!", Logger.LogType.Error);
                return;
            }
            _webSocket?.Close();
            CurrentRoom = null;
        }

        /// <summary>
        /// Disconnects from the DanNet server.
        /// </summary>
        public static void Disconnect()
        {
            if (CurrentRoom is not null)
            {
                LeaveRoom();
            }
            PlayerID = null;
        }
        
        internal static void Send(DanNetEvent danNetEvent, EventBehaviour eventBehaviour)
        {
            var type = eventBehaviour switch 
            {
                EventBehaviour.Normal     => DAN_NET_EVENT_TYPE_NORMAL,
                EventBehaviour.ServerSync => DAN_NET_EVENT_TYPE_SERVER_SYNC,
                EventBehaviour.Buffered   => DAN_NET_EVENT_TYPE_BUFFERED,
                _ => throw new System.ArgumentOutOfRangeException(nameof(eventBehaviour), eventBehaviour, null)
            };

            var message = new Message(type, danNetEvent);
            SendMessage(message);
            
            if (eventBehaviour == EventBehaviour.Normal)
            {
                OnDanNetEventMessage(Serialize(danNetEvent));
            }
        }

        internal static void SendStream(SyncDataStream stream)
        {
            SendMessage(new Message(STREAM_EVENT_TYPE, stream));
        }

        private static void OnApplicationQuit()
        {
            Disconnect();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (!IsConnected)
            {
                return;
            }
            
            if (loadMode == LoadSceneMode.Additive || !IsMasterClient)
            {
                return;
            }
            
            SyncObjectManager.ClearSyncObjects();
            SyncObjectManager.FetchAllSyncObjects();

            var syncObjectsOfCreator = SyncObjectManager.GetSyncObjectsOfCreator(PlayerID);
            var syncObjectData = syncObjectsOfCreator.Select(syncObject =>
                new SyncObjectData(syncObject.ID, syncObject.creatorID)).ToList();
            
            var data = Serialize(new Message(SYNC_OBJECTS_EVENT_TYPE, syncObjectData));
            SendMessageExplicit(Encoding.UTF8.GetBytes(data));
        }
        
        private static void InitializePrefabCache()
        {
            _prefabCache = new Dictionary<string, GameObject>();
            var resources = Resources.LoadAll<GameObject>("");
            foreach (var resource in resources)
            {
                if (resource.TryGetComponent<SyncObject>(out _))
                {
                    _prefabCache[resource.name] = resource;
                }
            }
        }

        #region Handling Messages
        
        private static void SendMessageExplicit(byte[] data)
        {
            _webSocket.Send(data);
        }

        private static void SendMessage(Message message)
        {
            if (message.type == STREAM_EVENT_TYPE)
            {
                SendMessageExplicit(Encoding.UTF8.GetBytes(Serialize(message)));
                return;
            }
            
            lock (_messageBatch)
            {
                var size = Encoding.UTF8.GetByteCount(Serialize(message));
                _currentMessageBatchSize += (uint) size;
                if (_currentMessageBatchSize > MAX_MESSAGE_BATCH_SIZE)
                {
                    BatchSendMessages();
                    _currentMessageBatchSize = 0;
                }
                _messageBatch.Add(Serialize(message));
            }
        }
        
        private static void BatchSendMessages()
        {
            byte[] batched;
            lock (_messageBatch)
            {
                if (_messageBatch.Count == 0 || _webSocket == null)
                    return;
                batched = Encoding.UTF8.GetBytes(string.Join("\n", _messageBatch));
            }

            _webSocket.Send(batched);
            _messageBatch.Clear();
        }

        [ExternalThreadEvent]
        private static void OnWebSocketMessage(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            var messages = str.Split('\n');
            
            Log(str);

            foreach (var msg in messages)
            {
                var message = Deserialize<Message>(msg);
                if (message is null)
                {
                    return;
                }

                var messageData = Serialize(message.data);
                MainThreadManager.Run(() => HandleMessageUpdate(message, messageData), MainThreadManager.UpdateType.Update);
            }
        }
        
        private static void HandleMessageUpdate(Message message, string data)
        {
            if (_messageHandlers.TryGetValue(message.type, out var handler))
            {
                handler.Invoke(data);
            }
        }
        
        private static void OnJoinedRoomMessage(string data)
        {
            CurrentRoom = Deserialize<Room>(data);
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            OnJoinedRoom?.Invoke(CurrentRoom);
            StreamManager.Init();
        }
        
        private static void OnLeftRoomMessage(string data)
        {
            var leftRoomResponse = Deserialize<LeftRoomResponse>(data);
            foreach (var syncObject in SyncObjectManager.GetSyncObjectsOfCreator(leftRoomResponse.playerId))
            {
                SyncObjectManager.RemoveSyncObject(syncObject);
                Object.Destroy(syncObject.gameObject);
            }
        }
        
        private static void OnDanNetEventMessage(string data)
        {
            var danNetEvent = Deserialize<DanNetEvent>(data);
            if (!_danNetEventHandlers.TryGetValue(danNetEvent.name, out var handler))
            {
                Logger.Log("No DanNet Event found for: " + danNetEvent.name, Logger.LogType.Error);
                return;
            }

            var sender = SyncObjectManager.GetSyncObjectByID(danNetEvent.senderId);
            if (sender is null)
            {
                Logger.Log("No sync object found with ID " + danNetEvent.senderId, Logger.LogType.Error);
                return;
            }

            handler.Invoke(sender, danNetEvent);
        }
        
        private static void CacheDanNetEvent(MethodInfo method, System.Type type)
        {
            var eventName = method.Name;
            _danNetEventHandlers[eventName] = (syncObject, danNetEvent) =>
            {
                if (!syncObject.TryGetComponent(type, out var component))
                {
                    return;
                }

                var parameters = method.GetParameters();
                var arguments = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    ParseParameter(parameters, i, arguments, danNetEvent);
                }

                method.Invoke(component, arguments);
            };
        }

        private static void ParseParameter(ParameterInfo[] parameters, int i, object[] arguments, DanNetEvent danNetEvent)
        {
            // WHOLE NUMBERS
            if (parameters[i].ParameterType == typeof(int))
                arguments[i] = (int) (long) danNetEvent.args[i];
            else if (parameters[i].ParameterType == typeof(short))
                arguments[i] = (short) (long) danNetEvent.args[i];
            else if (parameters[i].ParameterType == typeof(byte))
                arguments[i] = (byte) (long) danNetEvent.args[i];
            else if (parameters[i].ParameterType == typeof(long))
                arguments[i] = (long) danNetEvent.args[i];
                   
            // DECIMAL NUMBERS
            else if (parameters[i].ParameterType == typeof(decimal))
                arguments[i] = (decimal) (double) danNetEvent.args[i];
            else if (parameters[i].ParameterType == typeof(float))
                arguments[i] = (float) (double) danNetEvent.args[i];
            else if (parameters[i].ParameterType == typeof(double))
                arguments[i] = (double) danNetEvent.args[i];
                    
            // BOOL
            else if (parameters[i].ParameterType == typeof(bool))
                arguments[i] = (bool) danNetEvent.args[i];
                    
            // CHAR & STRING
            else if (parameters[i].ParameterType == typeof(char))
                arguments[i] = char.Parse((string) danNetEvent.args[i]);
            else if (parameters[i].ParameterType == typeof(string))
                arguments[i] = (string) danNetEvent.args[i];
                    
            else
                throw new System.Exception("Unsupported parameter type!");
        }

        private static void OnSyncObjectsMessage(string data)
        {
            var instantiationResponses = Deserialize<List<InstantiationResponse>>(data);
            if (instantiationResponses == null)
            {
                return;
            }
            
            foreach (var objectData in instantiationResponses)
            {
                var prefab = _prefabCache.GetValueOrDefault(objectData.prefabName);
                if (prefab == null)
                {
                    Logger.Log("Sync object prefab not found: " + objectData.prefabName, Logger.LogType.Error);
                    continue;
                }
                var instance = Object.Instantiate(prefab, 
                    objectData.position.ToVector3(), 
                    objectData.rotation.ToQuaternion());
                var syncObject = instance.GetComponent<SyncObject>();
                syncObject.Init(objectData.id, objectData.creatorId);
            }
        }

        private static void OnStreamMessage(string data)
        {
            var stream = Deserialize<SyncDataStream>(data);
            if (IsStreamEnabled)
            {
                StreamManager.ReceiveStream(stream);
            }
            
            SendMessage(new Message(PING_EVENT_TYPE, new LatencyResponse
            {
                serverTime = stream!.serverSentTime,
                clientTime = ClientTime.Get()
            }));
        }
        
        private static void OnInstantiateMessage(string data)
        {
            var instantiationResponse = Deserialize<InstantiationResponse>(data);
            var prefab = _prefabCache.GetValueOrDefault(instantiationResponse.prefabName);
            var instance = Object.Instantiate(prefab,
                instantiationResponse.position.ToVector3(),
                instantiationResponse.rotation.ToQuaternion());
            var syncObject = instance.GetComponent<SyncObject>();
            syncObject.Init(instantiationResponse.id, instantiationResponse.creatorId);
        }

        private static void OnDestroyMessage(string data)
        {
            var destroyResponse = Deserialize<DestroyResponse>(data);
            var obj = SyncObjectManager.GetSyncObjectByID(destroyResponse.id);

            if (obj == null)
            {
                return;
            }
            
            Object.Destroy(obj.gameObject);
        }
        
        private static void OnUpdateRoomMessage(string data)
        {
            var room = Deserialize<Room>(data);
            CurrentRoom = room;
            Logger.Log("Room updated: " + room);
        }
        
        private static void OnPongMessage(string data)
        {
            var pong = Deserialize<LatencyResponse>(data);
            
            Ping = (long) (pong.serverAckTime - pong.serverTime - (ClientTime.Get() - pong.clientTime) * 0.5d);
        }
        
        private static string Serialize<T>(T data)
        {
            var stringBuilder = new StringBuilder();
            using var writer = new JsonTextWriter(new System.IO.StringWriter(stringBuilder));
            _jsonSerializer.Serialize(writer, data);
            return stringBuilder.ToString();
        }
        
        private static T Deserialize<T>(string data)
        {
            using var reader = new JsonTextReader(new System.IO.StringReader(data));
            return _jsonSerializer.Deserialize<T>(reader);
        }

        private static void Log(string message)
        {
#if DEBUG
            if (IsLoggingEnabled)
            {
                Debug.Log(message);
            }
#endif
        }
        
        #endregion
    }
}
