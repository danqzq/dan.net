# SyncObjectManager (Internal Class)

The `SyncObjectManager` class is responsible for managing all [`SyncObject`](SyncObject.md) instances in the scene.

## Overview

This internal class maintains a registry of all [`SyncObject`](SyncObject.md) components in the current scene and provides methods to query and filter these objects.

## Internal Methods

### `GetSyncObjectByID(int id)`

Retrieves a [`SyncObject`](SyncObject.md) by its unique ID.

**Returns:** The [`SyncObject`](SyncObject.md) with the specified ID, or `null` if not found.

### `GetSyncObjectsOfCreator(string creatorID)`

Retrieves all [`SyncObject`](SyncObject.md) instances created by a specific player.

**Returns:** A read-only list of [`SyncObject`](SyncObject.md) components belonging to the specified creator.

### `GetForeignSyncObjects()`

Retrieves all [`SyncObject`](SyncObject.md) instances that do not belong to the local player.

**Returns:** An enumerable collection of foreign [`SyncObject`](SyncObject.md) components.

### `GetMySyncObjects()`

Retrieves all [`SyncObject`](SyncObject.md) instances that belong to the local player.

**Returns:** An enumerable collection of local [`SyncObject`](SyncObject.md) components.

### `AddSyncObject(SyncObject syncObject)`

Adds a [`SyncObject`](SyncObject.md) to the internal registry. Called automatically when a [`SyncObject`](SyncObject.md) starts.

### `RemoveSyncObject(SyncObject syncObject)`

Removes a [`SyncObject`](SyncObject.md) from the internal registry. Called automatically when a [`SyncObject`](SyncObject.md) is destroyed.

### `ClearSyncObjects()`

Clears all [`SyncObject`](SyncObject.md) references from the registry.

### `FetchAllSyncObjects()`

Scans the scene for all [`SyncObject`](SyncObject.md) components and rebuilds the internal registry.

> [!NOTE]
> This class is managed internally by DanNet. You typically don't need to interact with it directly.