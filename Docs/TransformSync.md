# TransformSync

The `TransformSync` class is a component that synchronizes the position and rotation of a `Transform` across all clients using the streaming system.

> [!NOTE]
> This component requires the [`SyncObject`](SyncObject.md) component to be attached to the same GameObject. It implements the [`ISyncData`](ISyncData.md) interface.

## Properties

### `TargetPosition` (Vector3)

The target position that the transform is interpolating towards (for remote objects).

### `TargetRotation` (Quaternion)

The target rotation that the transform is interpolating towards (for remote objects).

## Inspector Settings

### Update Mode

Determines when the position and rotation of the Transform are updated:

- `Normal`: Updates in the `Update()` method (default).
- `Fixed`: Updates in the `FixedUpdate()` method.
- `Late`: Updates in the `LateUpdate()` method.

### Sync Settings

#### `Sync Position` (bool)

Determines whether the position of the Transform is synchronized across all clients. Default is `true`.

#### `Sync Rotation` (bool)

Determines whether the rotation of the Transform is synchronized across all clients. Default is `true`.

### Smoothing Settings

#### `Smoothing` (bool)

Determines whether the position and rotation of the Transform are smoothly interpolated when synchronized across all clients. Default is `true`.

> [!TIP]
> Smoothing helps reduce jittery movement by interpolating between received positions/rotations.

#### `Lag Compensation Factor` (float)

Controls how much lag compensation is applied when synchronizing the Transform. Higher values result in more aggressive interpolation. Default is `10.0`.

## Optimization

The component only sends data when changes exceed certain thresholds:
- **Position Threshold:** 0.01 units
- **Rotation Threshold:** 1 degree

This reduces network traffic by avoiding unnecessary updates for tiny movements.

## Behavior

- **Local Objects:** If `IsMine` is `true`, the component does not apply received data, allowing local control.
- **Remote Objects:** Remote transforms smoothly interpolate to received positions and rotations based on the smoothing settings.