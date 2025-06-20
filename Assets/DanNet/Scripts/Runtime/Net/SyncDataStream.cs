using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dan.Net
{
    /// <summary>
    /// The currently streaming data. SyncObjects can send and receive data using this class, by implementing the ISyncData interface.
    /// </summary>
    [System.Serializable]
    public sealed class SyncDataStream
    {
        [JsonProperty] internal Dictionary<int, List<object>> data = new Dictionary<int, List<object>>();
        [JsonProperty] internal double serverSentTime;
        
        [JsonProperty] internal string __time__ = "DL_SERVER_SENT_TIME";
        
        /// <summary>
        /// The ID of the object that is receiving the data.
        /// </summary>
        public int ViewingId { private get; set; }
        
        /// <summary>
        /// The ID of the object that is sending the data.
        /// </summary>
        public int SendingId { private get; set; }

        public void Send(object obj)
        {
            if (data.ContainsKey(SendingId))
            {
                data[SendingId].Add(obj);
            }
            else
            {
                data.Add(SendingId, new List<object>{obj});
            }
        }
        
        public object Receive()
        {
            if (data.Count == 0 || !data.TryGetValue(ViewingId, out var value))
            {
                return default;
            }
            
            var obj = value[0];
            data[ViewingId].RemoveAt(0);
            return obj;
        }
    }
}