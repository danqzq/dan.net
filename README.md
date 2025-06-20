# Dan.Net

## Preview

https://github.com/user-attachments/assets/496c2715-a3c1-45c6-a524-387b1f41c71c

## Overview

Dan.Net is a simple online multiplayer networking solution running on WebSockets, designed for Unity games. Its structure
is inspired by the popular Photon Unity Networking (PUN) package.

## Features

- **Client-Server Architecture**: Dan.Net is built on a client-server architecture, allowing for a single server to host multiple rooms, each with multiple clients.
- **Event-based Networking**: Dan.Net provides an event-based networking system, allowing for easy communication between clients.
- **Stream-based Networking**: Dan.Net also provides a stream-based networking system, allowing for real-time data transfer between clients.
- **Cross-Platform**: Dan.Net is built on WebSockets, allowing for cross-platform compatibility.
- **Open Source**: Dan.Net is open source, allowing for easy modification and integration into your game.

## Installation

`TODO: Add installation instructions`

To install Dan.Net, simply download the latest release from the [releases page]()

## Usage

For Dan.Net to work, you need to have a server running the Dan.Net server software. You can find the server software in the [Dan.Net Server Download Link](https://www.danqzq.games/dan-net-server.exe).
The server listens on port '3000'.
The performance of Dan.Net is dependent on the performance of the server, so make sure to scale your server according to your needs.

Upon hosting the server, enter its IP address or domain name in the `DanNetConfig` located in the `Resources` folder of the Dan.Net package.
Additionally, you may access it through the toolbar by navigating to `Tools > DanNet > Config`.

Before you begin implementing actual online multiplayer mechanics, you need to set up a scene to have a friendly user interface for your players
to be able to connect to the server and create/join rooms. Create a script for handling networking, and make use of the static C# events such as:

```csharp
private void Awake()
{
    DanNet.OnConnected += OnConnected;
    DanNet.OnDisconnected += OnDisconnected;
    DanNet.OnJoinedRoom += OnJoinedRoom;
    DanNet.OnRoomCreated += OnRoomCreated;
}

private void OnDestroy()
{
    DanNet.OnConnected -= OnConnected;
    DanNet.OnDisconnected -= OnDisconnected;
    DanNet.OnJoinedRoom -= OnJoinedRoom;
    DanNet.OnRoomCreated -= OnRoomCreated;
}

// This is called when the user successfully connects to the server
private void OnConnected() { }

// This is called when the user creates a room
private void OnRoomCreated(Room room) { }

// This is called when the user joins a room
private void OnJoinedRoom(Room room) { }

// This is called when the user disconnects from the server
private void OnDisconnected() { }
```

Call `DanNet.Connect()` to connect to the server.

Call `DanNet.CreateOrJoinRoom(roomName, maxPlayers)` to create or join a room.

Call `DanNet.JoinRoom(roomName)` to join a room. You would want to do this when the room is created.

Call `DanNet.LeaveRoom()` to leave the room.

Call `DanNet.Instantiate(prefabName, position, rotation)` to spawn a networked prefab at the specified position and rotation.



## Networking

There are several ways to use to approach networking with **Dan.Net**:

### Event-based Networking

Dan.Net provides an event-based networking system, allowing you to easily send and receive messages between clients.
Dan.Net events can be sent immediately, buffered, or synchronized across all clients.

To emit any kind of event, you must have a script component which derives from [`MonoBehaviourDanNet`](Assets/DanNet/Scripts/Runtime/Net/MonoBehaviourDanNet.cs)

This kind of networking approach is suitable for turn-based games, where delays in sending and receiving messages are acceptable.

### Stream-based Networking

Dan.Net also provides a stream-based networking system, for real-time data transfer between clients.

To use the stream-based networking system, you must have a script component which derives from [`MonoBehaviourDanNet`](Assets/DanNet/Scripts/Runtime/Net/MonoBehaviourDanNet.cs) and
implements the [`ISyncData`](Assets/DanNet/Scripts/Runtime/Net/ISyncData.cs) interface.

This kind of networking approach is suitable for real-time games, where synchronization across all clients is important.
However, it is more network-intensive than the event-based networking system.

### Hybrid Networking

Both of the above systems can be used together, allowing for a hybrid networking system supporting
events and streams for maximum flexibility.

## Classes and Components

For a deeper dive into the classes and components of Dan.Net, refer to the links below:
- Main Components
  - [`DanNet (Main Class)`](Assets/DanNet/Scripts/Runtime/Net/DanNet.cs)
  - [`Room`](Assets/DanNet/Scripts/Runtime/Net/Models/Room.cs)
  - [`SyncObject`](Assets/DanNet/Scripts/Runtime/Net/SyncObject.cs)
  - [`MonoBehaviourDanNet`](Assets/DanNet/Scripts/Runtime/Net/MonoBehaviourDanNet.cs)
  - [`MonoBehaviourDanNetCallbacks`](Assets/DanNet/Scripts/Runtime/Net/MonoBehaviourDanNetCallbacks.cs)
  - [`DanNetEvent`](Assets/DanNet/Scripts/Runtime/Net/DanNetEvent.cs)
- Stream-based Networking
  - [`ISyncData`](Assets/DanNet/Scripts/Runtime/Net/ISyncData.cs)
  - [`SyncDataStream`](Assets/DanNet/Scripts/Runtime/Net/SyncDataStream.cs)
  - [`TransformSync`](Assets/DanNet/Scripts/Runtime/Net/TransformSync.cs)
- Configuration
  - [`DanNetConfig`](Assets/DanNet/Scripts/Runtime/Net/DanNetConfig.cs)
- Internal Classes
  - [`StreamManager`](Assets/DanNet/Scripts/Runtime/Net/StreamManager.cs)
  - [`SyncObjectManager`](Assets/DanNet/Scripts/Runtime/Net/SyncObjectManager.cs)

## Dependencies

Dan.Net uses the Hybrid WebSocket for WebSocket implementation by jirihybek: https://github.com/jirihybek/unity-websocket-webgl

Dan.Net also uses a modified version of the Main Thread Manager by Bearded Man Studios, Inc. (Forge Networking).

## License

Dan.Net is licensed under the [MIT License](LICENSE)
