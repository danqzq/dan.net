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
        
        /// <summary>
        /// Cached ISyncData components with their assigned indices
        /// </summary>
        private ISyncData[] _syncDataComponents;
        
        /// <summary>
        /// Gets the ISyncData components on this object, caching them on first access
        /// </summary>
        public ISyncData[] GetSyncDataComponents()
        {
            _syncDataComponents ??= GetComponents<ISyncData>();
            return _syncDataComponents;
        }
        
        /// <summary>
        /// Gets the index of a specific ISyncData component
        /// </summary>
        public int GetSyncDataIndex(ISyncData syncData)
        {
            var components = GetSyncDataComponents();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == syncData)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void Init(int id, string creatorId)
        {
            ID = id;
            creatorID = creatorId;
            _syncDataComponents = null; // Reset cache
            
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
                Logger.Log($"Duplicate ID {ID} on {name}", Logger.LogType.Warning);
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
                {
                    continue;
                }
                ID = i;
            }
        }

        private void Reset() => OnValidate();

        #endregion
    }
}