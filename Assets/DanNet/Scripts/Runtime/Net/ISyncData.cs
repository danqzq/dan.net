using UnityEngine;

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
    
    /// <summary>
    /// Extension methods for ISyncData to simplify usage
    /// </summary>
    public static class ISyncDataExtensions
    {
        /// <summary>
        /// Gets the component index for this ISyncData component.
        /// This index is used to identify which slot in the customDataArray belongs to this component.
        /// </summary>
        public static int GetComponentIndex(this ISyncData syncData, SyncObject syncObject)
        {
            return syncObject.GetSyncDataIndex(syncData);
        }
        
        /// <summary>
        /// Sends custom data with automatic component index resolution
        /// </summary>
        public static void Send(this ISyncData syncData, in SyncDataStream stream, byte[] data, SyncObject syncObject)
        {
            var index = syncObject.GetSyncDataIndex(syncData);
            if (index >= 0)
            {
                stream.Send(data, index);
            }
        }
        
        /// <summary>
        /// Receives custom data with automatic component index resolution
        /// </summary>
        public static byte[] Receive(this ISyncData syncData, in SyncDataStream stream, SyncObject syncObject)
        {
            var index = syncObject.GetSyncDataIndex(syncData);
            return index >= 0 ? stream.Receive(index) : null;
        }
    }
}