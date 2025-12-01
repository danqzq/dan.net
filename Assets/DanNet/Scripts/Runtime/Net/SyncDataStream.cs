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
        internal void SendTransform(Vector3? position, Quaternion? rotation)
        {
            if (!transformData.TryGetValue(SendingId, out var data))
            {
                data = new TransformData();
            }
            
            data.hasPosition = position.HasValue;
            data.hasRotation = rotation.HasValue;
            data.position = position ?? Vector3.zero;
            data.rotation = rotation ?? Quaternion.identity;
            transformData[SendingId] = data;
        }
        
        /// <summary>
        /// Sends custom data for the current object at a specific component index
        /// </summary>
        internal void Send(byte[] customData, int componentIndex)
        {
            if (!transformData.TryGetValue(SendingId, out var data))
            {
                data = new TransformData();
            }
            
            data.hasCustomData = true;
            
            // Initialize or expand the array if needed
            if (data.customDataArray == null)
            {
                data.customDataArray = new byte[componentIndex + 1][];
            }
            else if (data.customDataArray.Length <= componentIndex)
            {
                // Use Array.Resize for slightly better performance
                System.Array.Resize(ref data.customDataArray, componentIndex + 1);
            }
            
            data.customDataArray[componentIndex] = customData;
            transformData[SendingId] = data;
        }
        
        /// <summary>
        /// Receives transform data for the viewing object
        /// </summary>
        internal TransformData ReceiveTransform()
        {
            if (transformData.TryGetValue(ViewingId, out var data))
            {
                return data;
            }
            return default;
        }
        
        /// <summary>
        /// Receives custom data for the viewing object at a specific component index
        /// </summary>
        internal byte[] Receive(int componentIndex)
        {
            if (transformData.TryGetValue(ViewingId, out var data) && 
                data.hasCustomData && 
                data.customDataArray != null && 
                componentIndex < data.customDataArray.Length)
            {
                return data.customDataArray[componentIndex];
            }
            return null;
        }
    }
}