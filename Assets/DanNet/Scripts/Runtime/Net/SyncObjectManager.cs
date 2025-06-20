using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dan.Net
{
    public sealed class SyncObjectManager : MonoBehaviour
    {
        private static Dictionary<int, SyncObject> _syncObjectDictionary;

        public static SyncObject GetSyncObjectByID(int id)
        {
            return _syncObjectDictionary.GetValueOrDefault(id);
        }

        public static IReadOnlyList<SyncObject> GetSyncObjectsOfCreator(string creatorID)
        {
            return _syncObjectDictionary.Values.Where(syncObject => syncObject.creatorID == creatorID).ToList();
        }
        
        public static IEnumerable<SyncObject> GetForeignSyncObjects()
        {
            return _syncObjectDictionary.Values.Where(syncObject => !syncObject.IsMine);
        }
        
        public static IEnumerable<SyncObject> GetMySyncObjects()
        {
            return _syncObjectDictionary.Values.Where(syncObject => syncObject.IsMine);
        }

        private void Awake()
        {
            _syncObjectDictionary = new Dictionary<int, SyncObject>();
        }

        internal static void AddSyncObject(SyncObject syncObject)
        {
            if (_syncObjectDictionary.ContainsValue(syncObject))
                return;
            _syncObjectDictionary[syncObject.ID] = syncObject;
        }
        
        internal static void RemoveSyncObject(SyncObject syncObject)
        {
            _syncObjectDictionary.Remove(syncObject.ID);
        }
        
        internal static void ClearSyncObjects()
        {
            _syncObjectDictionary.Clear();
        }
        
        internal static void FetchAllSyncObjects()
        {
            var syncObjects = FindObjectsByType<SyncObject>(FindObjectsSortMode.None);
            _syncObjectDictionary = new Dictionary<int, SyncObject>(syncObjects.Length);
            foreach (var syncObject in syncObjects)
            {
                if (syncObject != null)
                {
                    _syncObjectDictionary.TryAdd(syncObject.ID, syncObject);
                }
            }
        }
    }
}