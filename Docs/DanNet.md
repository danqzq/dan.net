# DanNet

## Overview
The DanNet class is the main class of the Dan.Net package. It provides methods to connect to the server, manage rooms, and synchronize objects across the network. The class also contains events that are triggered when the player connects to / disconnects from the server, joins a room, or creates a room.


## Events

### Connection Events
- `OnConnected`: Invoked when a connection to the server is established.
- `OnDisconnected`: Invoked when disconnected from the server.

### Room Events
- `OnRoomCreated`: Invoked when a [Room](Room.md) is successfully created.
- `OnJoinedRoom`: Invoked when a player successfully joins a [Room](Room.md).



## Properties

### Connection Properties
- `IsConnected` (bool): Returns `true` if the player has established a connection to the DanNet server.

### Room Properties
- `CurrentRoom` ([Room](Room.md)): The information about the room the player is currently in. Set to `null` when not in a room.
- `IsMasterClient` (bool): Returns `true` if the player is the master client (creator) of the current room.

### Configuration Properties
- `IsStreamEnabled` (bool): Toggles streaming. Default is `true`.
- `IsLoggingEnabled` (bool): Enables or disables logging. When `true`, server messages will be logged. Default is `true`.
- `Ping` (long): The current network ping in milliseconds.
- `RandomPlayerNamePrefix` (string): Prefix used for random player names. Default is `"Player_"`.



## Public Methods

### Initialization
```csharp
[RuntimeInitializeOnLoadMethod]
private static void OnInitialize()
```
Initializes DanNet, registers scene loading events, and ensures essential objects persist across scenes.

### Connection
```csharp
public static void Connect(string username = null)
```
Attempts to connect to the DanNet server. If no username is provided, a random one is generated.

### Object Synchronization
```csharp
public static void Instantiate(string prefabName, Vector3 position, Quaternion rotation)
```
Spawns a networked prefab at the specified position and rotation.

> [!NOTE]
> The prefab must have a `SyncObject` component attached to it.

---

```csharp
public static void Destroy(GameObject gameObject)
```
Destroys a networked prefab instance.

### Room Management
```csharp
public static void CreateRoom(string roomName, int maxPlayers = 2)
```
Creates a new room on the server.

---

```csharp
public static void JoinRoom(string roomName)
```
Joins an existing room by name.

---

```csharp
public static void CreateOrJoinRoom(string roomName, int maxPlayers = 2)
```
Creates a room if none exists with the given name; otherwise, joins the existing one.

```csharp
public static void GetRoomList(System.Action<List<Room>> roomsCallback)
```
Retrieves the list of available rooms and invokes a callback with the results.

```csharp
public static void LeaveRoom()
```
Leaves the current room.

```csharp
public static void Disconnect()
```
Disconnects from the server and leaves any active room.



## Logging
DanNet provides logging functionality to track errors and events. Enable logging using:
```csharp
DanNet.IsLoggingEnabled = true;
```



## Internal Methods

### Networking
```csharp
internal static void Send(DanNetEvent danNetEvent, EventBehaviour eventBehaviour)
```
Sends an event with the specified behavior.

```csharp
internal static void SendStream(SyncDataStream stream)
```
Sends a stream of synchronized data.

### WebSocket Handling
```csharp
[ExternalThreadEvent]
private static void OnWebSocketMessage(byte[] data)
```
Processes incoming WebSocket messages.



## Internal Event Handlers
DanNet processes different message types to synchronize objects, handle room events, and update the network state. Below are key event handlers:

- `OnSceneLoaded`: Synchronizes objects when a new scene is loaded.
- `OnInstantiateMessage`: Handles networked object instantiation.
- `OnDestroyMessage`: Processes object destruction.
- `OnUpdateRoomMessage`: Updates room information.
- `OnStreamMessage`: Handles streaming synchronization.
