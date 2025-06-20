using Newtonsoft.Json;

namespace Dan
{
    [System.Serializable]
    internal struct Vec3
    {
        [JsonProperty] internal float x, y, z;
        
        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}