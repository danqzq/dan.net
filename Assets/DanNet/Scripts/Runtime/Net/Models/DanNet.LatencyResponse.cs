using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// The response received when measuring latency.
        /// </summary>
        [System.Serializable]
        public readonly struct LatencyResponse
        {
            /// <summary>
            /// The server time when the latency measurement was taken.
            /// </summary>
            [JsonProperty("serverTime")]
            public readonly double serverTime;

            /// <summary>
            /// The client time when the latency measurement was taken.
            /// </summary>
            [JsonProperty("clientTime")]
            public readonly double clientTime;

            /// <summary>
            /// The server acknowledgment time for the latency measurement.
            /// </summary>
            [JsonProperty("serverAckTime")]
            public readonly double serverAckTime;

            public LatencyResponse(double serverTime, double clientTime)
            {
                this.serverTime = serverTime;
                this.clientTime = clientTime;
                this.serverAckTime = 0;
            }

            [JsonConstructor]
            public LatencyResponse(double serverTime, double clientTime, double serverAckTime)
            {
                this.serverTime = serverTime;
                this.clientTime = clientTime;
                this.serverAckTime = serverAckTime;
            }
        }
    }
}