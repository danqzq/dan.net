# SyncObject

The `SyncObject` class is a component that can be attached to a prefab to synchronize its properties across all clients.

> [!NOTE]
> This component is required for any GameObject that needs to be networked. It must be attached to prefabs that will be instantiated using `DanNet.Instantiate()`.

## Properties

### `ID` (int)

The unique identifier of the object. This ID is automatically assigned and validated in the editor to ensure uniqueness.

### `IsMine` (bool)

Returns `true` if the object belongs to the local player (i.e., the local player is the creator of this object).

## Public Methods

### `CallEvent(string eventName, EventBehaviour eventBehaviour, params object[] args)` (void)

Calls a [`DanNetEvent`](DanNetEvent.md) on the object.

`EventBehaviour` is an enum with the following values:

- `Normal`: The event is sent to all clients. The event will be first executed locally and then sent to other clients.
- `ServerSync`: The event is sent to the server, which will then broadcast it to all clients.
- `Buffered`: The event is sent to all clients. The event will be first executed locally and then sent to other clients. The event will be buffered and replayed for new clients that join the room.