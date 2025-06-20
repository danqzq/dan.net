namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private struct DestroyResponse
        {
            public int id;
            
            public DestroyResponse(int id) => this.id = id;
        }
    }
}