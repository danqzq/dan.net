using UnityEngine;
using Newtonsoft.Json;

namespace Dan.Net
{
    public static partial class DanNet
    {
        /// <summary>
        /// The response received when instantiating a SyncObject.
        /// </summary>
        [System.Serializable]
        private readonly struct InstantiationResponse
        {
            /// <summary>
            /// The ID of the instantiated SyncObject.
            /// </summary>
            [JsonProperty("id")]
            public readonly int id;

            /// <summary>
            /// The name of the prefab instantiated.
            /// </summary>
            [JsonProperty("prefabName")]
            public readonly string prefabName;

            /// <summary>
            /// The ID of the player who created the SyncObject.
            /// </summary>
            [JsonProperty("creatorId")]
            public readonly string creatorId;

            [JsonProperty("position")]
            public readonly Vec3 position;

            [JsonProperty("rotation")]
            public readonly Vec3 rotation;

            public InstantiationResponse(string prefabName, Vector3 position, Quaternion rotation)
            {
                id = 0;
                creatorId = PlayerID;
                this.prefabName = prefabName;
                this.position = new Vec3(position.x, position.y, position.z);
                
                var eulerAngles = rotation.eulerAngles;
                this.rotation = new Vec3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            }

            [JsonConstructor]
            public InstantiationResponse(int id, string prefabName, string creatorId, Vec3 position, Vec3 rotation)
            {
                this.id = id;
                this.prefabName = prefabName;
                this.creatorId = creatorId;
                this.position = position;
                this.rotation = rotation;
            }
        }
    }
}