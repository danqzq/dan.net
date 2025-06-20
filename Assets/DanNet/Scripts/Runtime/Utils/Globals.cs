using System.Linq;
using UnityEngine;

namespace Dan.Net
{
    internal enum Route : byte
    {
        Connect,
        CreateRoom,
        JoinRoom,
        GetRoomList,
        Default
    }
    
    internal static class Globals
    {
        private static string _url;
        private static string Url
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    _url = $"{(Config.isSecure ? "s" : "")}://{Config.serverUrl}";
                }
                return _url;
            }
        }
        
        internal static DanNetConfig Config => Resources.Load<DanNetConfig>(RESOURCE_DAN_NET_CONFIG);

        internal static string GetServerUrl(Route route) => 
            "http" + Url + RouteToString(route);

        internal static string GetWebsocketUrl(Route route, params (string key, string value)[] args) => 
            "ws" + Url + RouteToString(route) + "?" + string.Join("&", args.Select(x => x.key + "=" + x.value));

        private static string RouteToString(Route route) => route switch
        {
            Route.Connect => "/connect",
            Route.CreateRoom => "/create-room",
            Route.JoinRoom => "/join-room",
            Route.GetRoomList => "/get-room-list",
            _ => ""
        };

        #region Routes
        
        internal const string ROUTE_CONNECT_NAME = "name";
        
        internal const string ROUTE_CREATE_ROOM_ROOM_NAME   = "roomName";
        internal const string ROUTE_CREATE_ROOM_PLAYER_ID   = "playerId";
        internal const string ROUTE_CREATE_ROOM_MAX_PLAYERS = "maxPlayers";
        
        internal const string ROUTE_JOIN_ROOM_ROOM_NAME = "roomName";
        internal const string ROUTE_JOIN_ROOM_PLAYER_ID = "playerId";

        #endregion
        
        #region Event Types

        internal const string JOINED_ROOM_EVENT_TYPE  = "joined_room";
        internal const string UPDATE_ROOM_EVENT_TYPE  = "update_room";
        internal const string LEFT_ROOM_EVENT_TYPE    = "left_room";
        internal const string SYNC_OBJECTS_EVENT_TYPE = "sync_objects";
        internal const string STREAM_EVENT_TYPE       = "stream";
        internal const string INSTANTIATE_EVENT_TYPE  = "instantiate";
        internal const string DESTROY_EVENT_TYPE      = "destroy";
        
        internal const string PING_EVENT_TYPE = "ping";
        internal const string PONG_EVENT_TYPE = "pong";
        
        internal const string DAN_NET_EVENT_TYPE_NORMAL      = "event_normal";
        internal const string DAN_NET_EVENT_TYPE_SERVER_SYNC = "event_server_sync";
        internal const string DAN_NET_EVENT_TYPE_BUFFERED    = "event_buffered";
        
        #endregion

        #region Resources
        
        private const string RESOURCE_DAN_NET_CONFIG = "DanNetConfig";

        #endregion
    }
}