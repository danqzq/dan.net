# ISyncData

`ISyncData` is an interface that provides methods for reading and sending synchronized data. This interface is used by the streaming system to synchronize data across all clients at a high frequency.

> [!NOTE]
> This component requires the `SyncObject` component to be attached to the same GameObject.

## Methods for Implementation

### `OnDataRead(in SyncDataStream stream)`

This method is called when synchronized data is received from the network. The `stream` parameter contains the data sent from other clients.

**Parameters:**
- `stream` ([SyncDataStream](SyncDataStream.md)): The stream containing received data. Passed by reference (`in`) for performance.

### `OnDataSend(in SyncDataStream stream)`

This method is called when it's time to send synchronized data to the network. Use the `stream` parameter to send your data.

**Parameters:**
- `stream` ([SyncDataStream](SyncDataStream.md)): The stream to send data through. Passed by reference (`in`) for performance.

## Usage Example

```cs
[RequireComponent(typeof(SyncObject))]
public class HealthSyncComponent : MonoBehaviourDanNet, ISyncData
{
    [SerializeField] private int _health;
    
    // This is executed when synchronized data is received
    public void OnDataRead(in SyncDataStream stream)
    {
        var data = this.Receive(in stream, syncObject);
        if (data != null && data.Length >= 4) // int requires 4 bytes
        {
            _health = System.BitConverter.ToInt32(data, 0);
        }
    }
    
    // This is executed when synchronized data is to be sent
    public void OnDataSend(in SyncDataStream stream)
    {
        var data = System.BitConverter.GetBytes(_health);
        this.Send(in stream, data, syncObject);
    }
}
```

For more implementation details, see [`TransformSync`](TransformSync.md) for a complete implementation example of this interface.