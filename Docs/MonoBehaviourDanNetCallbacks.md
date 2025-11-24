# MonoBehaviourDanNetCallbacks

`MonoBehaviourDanNetCallbacks` is an abstract base class that extends [`MonoBehaviourDanNet`](MonoBehaviourDanNet.md) and automatically subscribes to DanNet connection and room events.

## Overview

This class simplifies handling DanNet events by automatically subscribing to events in `Awake()` and unsubscribing in `OnDestroy()`. Instead of manually managing event subscriptions, you can simply override the callback methods.

> [!WARNING]
> This class uses `Awake()` and `OnDestroy()` Unity events. If you override them, make sure to call the base methods to ensure proper functionality.

## Virtual Methods

Override these methods to handle network events:

### `OnConnected()`

Called when a connection to the DanNet server is established.

### `OnDisconnected()`

Called when disconnected from the DanNet server.

### `OnRoomCreated(Room room)`

Called when a room is successfully created. The `room` parameter contains information about the created room.

### `OnJoinedRoom(Room room)`

Called when the player successfully joins a room. The `room` parameter contains information about the joined room.

## Example

```csharp
using Dan.Net;
using Dan.Net.Models;

public class NetworkManager : MonoBehaviourDanNetCallbacks
{
    protected override void OnConnected()
    {
        Debug.Log("Connected to DanNet server!");
    }
    
    protected override void OnJoinedRoom(Room room)
    {
        Debug.Log($"Joined room: {room.name}");
    }
    
    protected override void OnDisconnected()
    {
        Debug.Log("Disconnected from server");
    }
}
```