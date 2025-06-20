namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        public struct LatencyResponse
        {
            public double serverTime;
            public double clientTime;
            public double serverAckTime;
        }
    }
}