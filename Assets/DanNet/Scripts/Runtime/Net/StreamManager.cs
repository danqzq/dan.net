using System.Linq;
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
                if (!stream.data.ContainsKey(syncObject.ID))
                {
                    continue;
                }
                stream.ViewingId = syncObject.ID;
                if (syncObject.TryGetComponent<ISyncData>(out var syncData)) 
                    syncData.OnDataRead(in stream);
            }
        }

        private void SendData()
        {
            var stream = new SyncDataStream();

            foreach (var syncObject in SyncObjectManager.GetMySyncObjects())
            {
                stream.SendingId = syncObject.ID;
                if (syncObject.TryGetComponent<ISyncData>(out var syncData)) 
                    syncData.OnDataSend(in stream);
            }

            if (DanNet.IsStreamEnabled)
            {
                DanNet.SendStream(stream);
            }
        }
    }
}