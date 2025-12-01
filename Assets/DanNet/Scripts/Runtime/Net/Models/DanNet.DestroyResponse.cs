using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// The response received when destroying a SyncObject.
        /// </summary>
        [System.Serializable]
        private readonly struct DestroyResponse
        {
            /// <summary>
            /// The ID of the destroyed SyncObject.
            /// </summary>
            [JsonProperty("id")]
            public readonly int id;
            
            [JsonConstructor]
            public DestroyResponse(int id)
            {
                this.id = id;
            }
        }
    }
}