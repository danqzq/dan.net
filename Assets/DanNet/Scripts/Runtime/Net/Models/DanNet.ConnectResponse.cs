using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// The response received when connecting to the server.
        /// </summary>
        [System.Serializable]
        private readonly struct ConnectResponse
        {
            [JsonProperty("playerId")]
            public readonly string playerId;

            [JsonConstructor]
            public ConnectResponse(string playerId)
            {
                this.playerId = playerId;
            }
        }
    }
}