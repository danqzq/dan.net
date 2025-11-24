using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dan.Net
{
    /// <summary>
    /// Transform data for binary stream encoding
    /// </summary>
    public struct TransformData
    {
        public bool hasPosition;
        public bool hasRotation;
        public bool hasCustomData;
        public Vector3 position;
        public Quaternion rotation;
        public byte[] customData;
    }
    
    /// <summary>
    /// Binary protocol for efficient streaming of transform data
    /// </summary>
    internal static class BinaryStreamProtocol
    {
        private const byte FLAG_HAS_POSITION = 0x01;
        private const byte FLAG_HAS_ROTATION = 0x02;
        private const byte FLAG_HAS_CUSTOM_DATA = 0x04;
        
        /// <summary>
        /// Encodes transform data into binary format
        /// </summary>
        public static byte[] EncodeStream(Dictionary<int, TransformData> transformData)
        {
            if (transformData == null || transformData.Count == 0)
            {
                return new byte[2];
            }

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            writer.Write((ushort)transformData.Count);

            foreach (var kvp in transformData)
            {
                var objectId = kvp.Key;
                var data = kvp.Value;

                writer.Write(objectId);

                byte flags = 0;
                if (data.hasPosition) flags |= FLAG_HAS_POSITION;
                if (data.hasRotation) flags |= FLAG_HAS_ROTATION;
                if (data.hasCustomData) flags |= FLAG_HAS_CUSTOM_DATA;
                writer.Write(flags);

                if (data.hasPosition)
                {
                    writer.Write(data.position.x);
                    writer.Write(data.position.y);
                    writer.Write(data.position.z);
                }

                if (data.hasRotation)
                {
                    var compressed = CompressQuaternion(data.rotation);
                    writer.Write(compressed);
                }

                if (data.hasCustomData && data.customData != null)
                {
                    writer.Write((ushort)data.customData.Length);
                    writer.Write(data.customData);
                }
            }

            return stream.ToArray();
        }
        
        /// <summary>
        /// Decodes binary stream data back to transform data
        /// </summary>
        public static Dictionary<int, TransformData> DecodeStream(byte[] data)
        {
            var result = new Dictionary<int, TransformData>();
            
            if (data == null || data.Length < 2)
            {
                return result;
            }
            
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var objectCount = reader.ReadUInt16();
                
                for (int i = 0; i < objectCount; i++)
                {
                    var objectId = reader.ReadInt32();
                    
                    var flags = reader.ReadByte();
                    var hasPosition = (flags & FLAG_HAS_POSITION) != 0;
                    var hasRotation = (flags & FLAG_HAS_ROTATION) != 0;
                    var hasCustomData = (flags & FLAG_HAS_CUSTOM_DATA) != 0;
                    
                    var transformData = new TransformData
                    {
                        hasPosition = hasPosition,
                        hasRotation = hasRotation,
                        hasCustomData = hasCustomData
                    };
                    
                    if (hasPosition)
                    {
                        var x = reader.ReadSingle();
                        var y = reader.ReadSingle();
                        var z = reader.ReadSingle();
                        transformData.position = new Vector3(x, y, z);
                    }
                    
                    if (hasRotation)
                    {
                        var compressed = reader.ReadBytes(7);
                        transformData.rotation = DecompressQuaternion(compressed);
                    }
                    
                    if (hasCustomData)
                    {
                        var customDataLength = reader.ReadUInt16();
                        transformData.customData = reader.ReadBytes(customDataLength);
                    }
                    
                    result[objectId] = transformData;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Compresses a quaternion using smallest-three method
        /// Format: 1 byte (largest index) + 3 shorts (other components)
        /// </summary>
        private static byte[] CompressQuaternion(Quaternion q)
        {
            q = Quaternion.Normalize(q);
            
            var components = new[] { q.x, q.y, q.z, q.w };
            var largestIndex = 0;
            var largestValue = Mathf.Abs(components[0]);
            
            for (int i = 1; i < 4; i++)
            {
                var abs = Mathf.Abs(components[i]);
                if (abs > largestValue)
                {
                    largestValue = abs;
                    largestIndex = i;
                }
            }
            
            var sign = components[largestIndex] < 0 ? -1f : 1f;
            
            var result = new byte[7];
            result[0] = (byte)largestIndex;
            
            var writeIndex = 1;
            for (int i = 0; i < 4; i++)
            {
                if (i == largestIndex) continue;
                
                var value = components[i] * sign;
                var compressed = (short)(value * short.MaxValue);
                
                result[writeIndex++] = (byte)(compressed & 0xFF);
                result[writeIndex++] = (byte)((compressed >> 8) & 0xFF);
            }
            
            return result;
        }
        
        /// <summary>
        /// Decompresses a quaternion from smallest-three format
        /// </summary>
        private static Quaternion DecompressQuaternion(byte[] data)
        {
            if (data.Length != 7)
            {
                return Quaternion.identity;
            }
            
            var largestIndex = data[0];
            var components = new float[4];
            
            var readIndex = 1;
            for (int i = 0; i < 4; i++)
            {
                if (i == largestIndex) continue;
                
                var compressed = (short)(data[readIndex] | (data[readIndex + 1] << 8));
                components[i] = compressed / (float)short.MaxValue;
                readIndex += 2;
            }
            
            var sumSquares = components[0] * components[0] +
                             components[1] * components[1] +
                             components[2] * components[2] +
                             components[3] * components[3];
            
            components[largestIndex] = Mathf.Sqrt(Mathf.Max(0, 1f - sumSquares));
            
            return new Quaternion(components[0], components[1], components[2], components[3]);
        }
    }
}
