using Newtonsoft.Json;

namespace Dan.Net
{
    public enum EventBehaviour : byte
    {
        /// <summary>
        /// The event is sent to all clients. The event will be first executed locally and then sent to other clients.
        /// </summary>
        Normal,
        
        /// <summary>
        /// The event is sent to the server, which will then broadcast it to all clients.
        /// </summary>
        ServerSync,
        
        /// <summary>
        ///  The event is sent to all clients. The event will be first executed locally and then sent to other clients. The event will be buffered and replayed for new clients that join the room.
        /// </summary>
        Buffered
    }
    
    [System.Serializable]
    internal struct DanNetEvent
    {
        [JsonProperty] internal string name;
        [JsonProperty] internal object[] args;
        [JsonProperty] internal int senderId;
        
        internal DanNetEvent(string name, object[] args, int senderId)
        {
            this.name = name;
            this.args = args;
            this.senderId = senderId;
        }
    }
}