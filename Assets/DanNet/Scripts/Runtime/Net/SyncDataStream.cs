using UnityEngine;
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
        [JsonProperty] internal Dictionary<int, TransformData> transformData = new Dictionary<int, TransformData>();
        [JsonProperty] internal double serverSentTime;
        
        /// <summary>
        /// The ID of the object that is receiving the data.
        /// </summary>
        public int ViewingId { private get; set; }
        
        /// <summary>
        /// The ID of the object that is sending the data.
        /// </summary>
        public int SendingId { private get; set; }

        /// <summary>
        /// Sends transform data for the current object
        /// </summary>
        public void SendTransform(Vector3? position, Quaternion? rotation)
        {
            if (!transformData.ContainsKey(SendingId))
            {
                transformData[SendingId] = new TransformData();
            }
            
            var data = transformData[SendingId];
            data.hasPosition = position.HasValue;
            data.hasRotation = rotation.HasValue;
            data.position = position ?? Vector3.zero;
            data.rotation = rotation ?? Quaternion.identity;
            transformData[SendingId] = data;
        }
        
        /// <summary>
        /// Sends custom data for the current object
        /// </summary>
        public void Send(byte[] customData)
        {
            if (!transformData.ContainsKey(SendingId))
            {
                transformData[SendingId] = new TransformData();
            }
            
            var data = transformData[SendingId];
            data.hasCustomData = true;
            data.customData = customData;
            transformData[SendingId] = data;
        }
        
        /// <summary>
        /// Receives transform data for the viewing object
        /// </summary>
        public TransformData ReceiveTransform()
        {
            if (transformData.TryGetValue(ViewingId, out var data))
            {
                return data;
            }
            return default;
        }
        
        /// <summary>
        /// Receives custom data for the viewing object
        /// </summary>
        public byte[] Receive()
        {
            if (transformData.TryGetValue(ViewingId, out var data) && data.hasCustomData)
            {
                return data.customData;
            }
            return null;
        }
    }
}