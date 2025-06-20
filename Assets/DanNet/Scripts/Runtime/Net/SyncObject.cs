using System.Linq;
using UnityEngine;

namespace Dan.Net
{
    public sealed class SyncObject : MonoBehaviour
    {
        /// <summary>
        /// The unique identifier of the object.
        /// </summary>
        public int ID;
        internal string creatorID;
        
        /// <summary>
        /// Returns true if the object belongs to the local player.
        /// </summary>
        public bool IsMine => DanNet.PlayerID == creatorID;

        internal void Init(int id, string creatorId)
        {
            ID = id;
            creatorID = creatorId;
            
            SyncObjectManager.AddSyncObject(this);
        }
        
        /// <summary>
        /// Calls a DanNetEvent on the object.
        /// </summary>
        /// <param name="method">The DanNetEvent method name</param>
        /// <param name="eventBehaviour"></param>
        /// <param name="args">The arguments required for the method</param>
        public void CallEvent(string method, EventBehaviour eventBehaviour, params object[] args)
        {
            var danNetEvent = new DanNetEvent(method, args, ID);
            DanNet.Send(danNetEvent, eventBehaviour);
        }

        #region Unity Events
        
        private void Awake()
        {
            OnValidate();
            if (FindObjectsByType<SyncObject>(FindObjectsSortMode.None).Any(x => x.ID == ID && x != this))
            {
                Debug.LogError($"Duplicate ID {ID} on {name}");
            }
        }

        private void Start()
        {
            SyncObjectManager.AddSyncObject(this);
        }
        
        private void OnDestroy()
        {
            SyncObjectManager.RemoveSyncObject(this);
        }
        
        private void OnValidate()
        {
            var all = FindObjectsByType<SyncObject>(FindObjectsSortMode.None);
            for (int i = 1; i < all.Length; i++)
            {
                if (all.Any(x => x.ID == i))
                    continue;
                ID = i;
            }
        }

        private void Reset() => OnValidate();

        #endregion
    }
}