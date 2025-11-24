using System;
using System.Text;

namespace Dan.Net
{
    /// <summary>
    /// Binary protocol for WebSocket communication.
    /// Format: [Type(1B)][Length(4B)][Payload(JSON)]
    /// </summary>
    internal static class BinaryProtocol
    {
        /// <summary>
        /// Encodes a message into binary format.
        /// </summary>
        /// <param name="messageType">Message type identifier (0x01-0x0C)</param>
        /// <param name="jsonPayload">JSON-encoded payload string</param>
        /// <returns>Binary encoded message</returns>
        public static byte[] Encode(byte messageType, string jsonPayload)
        {
            var payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var payloadLength = payloadBytes.Length;
            
            var buffer = new byte[5 + payloadLength];
            
            buffer[0] = messageType;
            
            buffer[1] = (byte)(payloadLength & 0xFF);
            buffer[2] = (byte)((payloadLength >> 8) & 0xFF);
            buffer[3] = (byte)((payloadLength >> 16) & 0xFF);
            buffer[4] = (byte)((payloadLength >> 24) & 0xFF);
            
            Array.Copy(payloadBytes, 0, buffer, 5, payloadLength);
            
            return buffer;
        }
        
        /// <summary>
        /// Decodes a binary message.
        /// </summary>
        /// <param name="data">Binary message data</param>
        /// <returns>Tuple of message type and JSON payload</returns>
        /// <exception cref="ArgumentException">Thrown when data is invalid</exception>
        public static (byte messageType, string jsonPayload) Decode(byte[] data)
        {
            if (data == null || data.Length < 5)
            {
                throw new ArgumentException($"Invalid message: too short ({data?.Length ?? 0} bytes, minimum 5 required)");
            }
            
            var messageType = data[0];
            var payloadLength = data[1] 
                | (data[2] << 8) 
                | (data[3] << 16) 
                | (data[4] << 24);
            
            if (data.Length < 5 + payloadLength)
            {
                throw new ArgumentException($"Incomplete message: expected {5 + payloadLength} bytes, got {data.Length}");
            }
            
            var jsonPayload = Encoding.UTF8.GetString(data, 5, payloadLength);
            
            return (messageType, jsonPayload);
        }
    }
}
