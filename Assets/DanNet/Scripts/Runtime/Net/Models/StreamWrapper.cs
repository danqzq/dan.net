using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private readonly struct StreamWrapper
        {
            [JsonProperty("serverSentTime")]
            public readonly double serverSentTime;
            
            [JsonProperty("binaryData")]
            public readonly string binaryData;

            [JsonConstructor]
            public StreamWrapper(double serverSentTime, string binaryData)
            {
                this.serverSentTime = serverSentTime;
                this.binaryData = binaryData;
            }
        }
    }
}