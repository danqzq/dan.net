using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private readonly struct SyncObjectData
        {
            [JsonProperty("id")]
            public readonly int id;
            [JsonProperty("creatorId")]
            public readonly string creatorId;
            
            [JsonConstructor]
            public SyncObjectData(int id, string creatorId)
            {
                this.id = id;
                this.creatorId = creatorId;
            }
        }
    }
}