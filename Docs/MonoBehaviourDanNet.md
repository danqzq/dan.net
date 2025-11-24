# MonoBehaviourDanNet

`MonoBehaviourDanNet` is a base class for all Dan.Net script components.
It provides a set of events that you can override to handle network events.

## Properties

### `syncObject` (SyncObject)

The `SyncObject` component attached to the game object. This component is used to send and receive network events.

> [!NOTE]
> The `syncObject` field is automatically assigned when the script component is added to a game object.

> [!WARNING]
> `Awake()` and `OnValidate()` Unity Events are in use in this class, so if you override them, make sure to call the base method to ensure proper functionality.