namespace Dan.Net
{
    /// <summary>
    /// The interface for sync objects that must receive and send data.
    /// Requires the object to have a SyncObject component.
    /// </summary>
    public interface ISyncData
    {
        /// <summary>
        /// Will be called when stream data is received.
        /// </summary>
        /// <param name="stream"></param>
        public void OnDataRead(in SyncDataStream stream);
        
        /// <summary>
        /// Will be called when stream data is sent.
        /// </summary>
        /// <param name="stream"></param>
        public void OnDataSend(in SyncDataStream stream);
    }
}