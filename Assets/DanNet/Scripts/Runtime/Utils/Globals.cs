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

        internal const byte JOINED_ROOM_EVENT_TYPE  = 0x01;
        internal const byte UPDATE_ROOM_EVENT_TYPE  = 0x02;
        internal const byte LEFT_ROOM_EVENT_TYPE    = 0x03;
        internal const byte SYNC_OBJECTS_EVENT_TYPE = 0x04;
        internal const byte STREAM_EVENT_TYPE       = 0x05;
        internal const byte INSTANTIATE_EVENT_TYPE  = 0x06;
        internal const byte DESTROY_EVENT_TYPE      = 0x07;
        
        internal const byte PING_EVENT_TYPE = 0x08;
        internal const byte PONG_EVENT_TYPE = 0x09;
        
        internal const byte DAN_NET_EVENT_TYPE_NORMAL      = 0x0A;
        internal const byte DAN_NET_EVENT_TYPE_SERVER_SYNC = 0x0B;
        internal const byte DAN_NET_EVENT_TYPE_BUFFERED    = 0x0C;
        
        #endregion

        #region Resources
        
        private const string RESOURCE_DAN_NET_CONFIG = "DanNetConfig";

        #endregion
    }
}