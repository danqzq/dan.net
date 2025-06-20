namespace Dan.Net.Models
{
    [System.Serializable]
    public sealed class Room
    {
        public string name;
        public byte maxPlayers;
        public byte currentPlayers;
        
        public string creatorId;
    }
}