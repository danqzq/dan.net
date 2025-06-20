using System;
using Dan.Net.Models;

namespace Dan.Net
{
    public abstract class MonoBehaviourDanNetCallbacks : MonoBehaviourDanNet
    {
        protected override void Awake()
        {
            base.Awake();
            DanNet.OnConnected += OnConnected;
            DanNet.OnDisconnected += OnDisconnected;
            DanNet.OnJoinedRoom += OnJoinedRoom;
            DanNet.OnRoomCreated += OnRoomCreated;
        }

        protected virtual void OnDestroy()
        {
            DanNet.OnConnected -= OnConnected;
            DanNet.OnDisconnected -= OnDisconnected;
            DanNet.OnJoinedRoom -= OnJoinedRoom;
            DanNet.OnRoomCreated -= OnRoomCreated;
        }

        protected virtual void OnConnected()
        {
            
        }
        
        protected virtual void OnRoomCreated(Room room)
        {
            
        }
        
        protected virtual void OnJoinedRoom(Room room)
        {
            
        }
        
        protected virtual void OnDisconnected()
        {
            
        }
    }
}