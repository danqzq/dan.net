# SyncDataStream

This class handles the currently streaming data. [`SyncObject`](SyncObject.md) components can send and receive data using this class by implementing the [`ISyncData`](ISyncData.md) interface.

## Properties

### `ViewingId` (int)

The ID of the object that is currently receiving the data. Used internally when processing received stream data.

### `SendingId` (int)

The ID of the object that is currently sending the data. Used internally when building outgoing stream data.

## Public Methods

### Transform Synchronization

#### `SendTransform(Vector3? position, Quaternion? rotation)`

Sends transform data (position and/or rotation) for the current object.

**Parameters:**
- `position` (Vector3?): The position to send, or `null` to skip position synchronization.
- `rotation` (Quaternion?): The rotation to send, or `null` to skip rotation synchronization.

#### `ReceiveTransform()`

Receives transform data for the viewing object.

**Returns:** `TransformData` structure containing position and rotation information.

### Custom Data Synchronization

#### `Send(byte[] customData)`

Sends custom binary data for the current object.

**Parameters:**
- `customData` (byte[]): The custom data to send as a byte array.

#### `Receive()`

Receives custom data for the viewing object.

**Returns:** `byte[]` containing the custom data, or `null` if no custom data is available.

## Usage

This class is typically used within implementations of the [`ISyncData`](ISyncData.md) interface. The streaming system automatically manages `SendingId` and `ViewingId` properties.