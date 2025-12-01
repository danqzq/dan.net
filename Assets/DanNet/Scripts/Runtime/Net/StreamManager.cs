using UnityEngine;

namespace Dan.Net
{
    public sealed class StreamManager : MonoBehaviour
    {
        private const float DELAY_TIME = 0.1f;
        
        private static StreamManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        internal static void Init()
        {
            _instance.InvokeRepeating(nameof(SendData), DELAY_TIME, 1f / Globals.Config.dataSendRate);
        }

        internal static void ReceiveStream(SyncDataStream stream)
        {
            foreach (var syncObject in SyncObjectManager.GetForeignSyncObjects())
            {
                if (!stream.transformData.ContainsKey(syncObject.ID))
                {
                    continue;
                }
                
                stream.ViewingId = syncObject.ID;
                var syncDataComponents = syncObject.GetSyncDataComponents();
                foreach (var syncData in syncDataComponents)
                {
                    syncData.OnDataRead(in stream);
                }
            }
        }

        private void SendData()
        {
            var stream = new SyncDataStream();

            foreach (var syncObject in SyncObjectManager.GetMySyncObjects())
            {
                stream.SendingId = syncObject.ID;
                var syncDataComponents = syncObject.GetSyncDataComponents();
                foreach (var syncData in syncDataComponents)
                {
                    syncData.OnDataSend(in stream);
                }
            }

            if (DanNet.IsStreamEnabled && stream.transformData.Count > 0)
            {
                DanNet.SendStream(stream);
            }
        }
    }
}