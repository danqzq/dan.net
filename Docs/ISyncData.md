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

See [`TransformSync`](TransformSync.md) for a complete implementation example of this interface.