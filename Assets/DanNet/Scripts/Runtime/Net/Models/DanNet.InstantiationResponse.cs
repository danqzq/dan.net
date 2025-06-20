using UnityEngine;

namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private struct InstantiationResponse
        {
            public int id;
            public string prefabName, creatorId;
            public Vec3 position, rotation;

            public InstantiationResponse(string prefabName, Vector3 position, Quaternion rotation)
            {
                id = 0;
                creatorId = PlayerID;
                this.prefabName = prefabName;
                this.position = new Vec3(position.x, position.y, position.z);
                
                var eulerAngles = rotation.eulerAngles;
                this.rotation = new Vec3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            }
        }
    }
}