namespace Dan
{
    public static class ClientTime
    {
        public static double Get()
        {
            return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}