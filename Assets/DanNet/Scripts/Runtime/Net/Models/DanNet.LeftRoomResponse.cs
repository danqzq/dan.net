using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// The response received when a player leaves a room.
        /// </summary>
        [System.Serializable]
        public readonly struct LeftRoomResponse
        {
            [JsonProperty("playerId")]
            public readonly string playerId;

            [JsonConstructor]
            public LeftRoomResponse(string playerId)
            {
                this.playerId = playerId;
            }
        }
    }
}