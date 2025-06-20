namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private struct SyncObjectData
        {
            public int id;
            public string creatorId;
            
            public SyncObjectData(int id, string creatorId)
            {
                this.id = id;
                this.creatorId = creatorId;
            }
        }
    }
}