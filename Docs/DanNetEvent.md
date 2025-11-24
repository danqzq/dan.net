# DanNetEvent

This is an attribute class that can be used to define a network event.
It is used in conjunction with the [`MonoBehaviourDanNet`](MonoBehaviourDanNet.md) class to send and receive network events.

When applied on a method, the method will be invoked when the event is received.

## Example

```csharp

using DanNet;

public class MyNetworkEvents : MonoBehaviourDanNet
{
    [DanNetEvent]
    private void OnPlayerMove(int xDirection, int yDirection)
    {
        // Handle player movement
    }
}
```

In the example above, the `OnPlayerMove` method will be invoked when the `OnPlayerMove` event is received.

The method can have any number of parameters, but they must be default-constructible types.

In order to call the event, use the SendEvent method, which is a part of the [`SyncObject`](SyncObject.md) class.

When inheriting a script component from [`MonoBehaviourDanNet`](MonoBehaviourDanNet.md), it is assumed that the object
contains a [`SyncObject`](SyncObject.md) component. The [`MonoBehaviourDanNet`](MonoBehaviourDanNet.md) class provides a `syncObject` field that can be used to send events.

```csharp
syncObject.CallEvent("OnPlayerMove", EventBehaviour.Normal, xDirection, yDirection);
```

A recommended approach is to make use of `nameof` to avoid hardcoding event names.

```csharp
syncObject.CallEvent(nameof(OnPlayerMove), EventBehaviour.Normal, xDirection, yDirection);
```

Find more about `CallEvent` in the [`SyncObject`](SyncObject.md) documentation.