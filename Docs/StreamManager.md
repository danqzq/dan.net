# StreamManager (Internal Class)

The `StreamManager` is an internal class that manages the synchronization of data across all clients using the stream-based networking system.

## Overview

The `StreamManager` automatically handles sending and receiving stream data for all [`SyncObject`](SyncObject.md) components that implement the [`ISyncData`](ISyncData.md) interface. It operates at a frequency defined by the `dataSendRate` setting in [`DanNetConfig`](DanNetConfig.md).

## Internal Functionality

### Data Sending

The manager periodically invokes `OnDataSend()` on all [`ISyncData`](ISyncData.md) components belonging to the local player's objects. The collected data is then sent to the server as a single stream packet.

### Data Receiving

When stream data is received from the server, the manager distributes it to all foreign (non-local) [`SyncObject`](SyncObject.md) components by invoking their `OnDataRead()` method.

### Configuration

The send rate is controlled by `dataSendRate` in [`DanNetConfig`](DanNetConfig.md), with a default value of 20 times per second.

> [!NOTE]
> This class is managed internally by DanNet. You don't need to interact with it directly - implement [`ISyncData`](ISyncData.md) on your components instead.

