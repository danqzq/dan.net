namespace Dan.Net
{
    public static partial class DanNet
    {
        [System.Serializable]
        private sealed class Message
        {
            public byte type;
            public object data;
            
            public Message(byte type, object data)
            {
                this.type = type;
                this.data = data;
            }
        }
    }
}