using UnityEngine;

namespace Dan.Net
{
    /// <summary>
    /// A base class for all Dan.Net script components.
    /// It provides a set of events that you can override to handle network events.
    /// </summary>
    public abstract class MonoBehaviourDanNet : MonoBehaviour
    {
        /// <summary>
        /// The SyncObject component attached to the game object. This component is used to send and receive network events.
        /// </summary>
        [HideInInspector] public SyncObject syncObject;
        
        protected virtual void Awake() => syncObject = GetComponent<SyncObject>();

        protected virtual void OnValidate()
        {
            if (GetComponent<SyncObject>() == null)
            {
                Logger.Log("SyncObject component is missing on " + gameObject.name, Logger.LogType.Error);
            }
        }
    }
}