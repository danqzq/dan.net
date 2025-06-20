namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private sealed class Message
        {
            public string type;
            public object data;
            
            public Message(string type, object data)
            {
                this.type = type;
                this.data = data;
            }
        }
    }
}