# Dan.Net

`Dan.Net` is a simple online multiplayer networking solution running on WebSockets, designed for Unity games. Its structure
is inspired by the popular `Photon Unity Networking (PUN)` package.

Latest Version: v1.2

---

https://github.com/user-attachments/assets/496c2715-a3c1-45c6-a524-387b1f41c71c

Try out the example for yourself [here](https://www.danqzq.games)

## Features

- **Client-Server Architecture**: `Dan.Net` is built on a client-server architecture, allowing for a single server to host multiple rooms, each with multiple clients.
- **Event-based Networking**: `Dan.Net` provides an event-based networking system, allowing for easy communication between clients.
- **Stream-based Networking**: `Dan.Net` also provides a stream-based networking system, allowing for real-time data transfer between clients.
- **Cross-Platform**: `Dan.Net` is built on WebSockets, allowing for cross-platform compatibility.
- **Open Source**: `Dan.Net` is open source, allowing for easy modification and integration into your game.

## Dependencies

- If importing manually, add **Newtonsoft Json** as a dependency into your Unity project: `com.unity.nuget.newtonsoft-json` (Add via git URL in Package Manager).
- `Dan.Net` uses the **Hybrid WebSocket** for WebSocket implementation by `jirihybek`: https://github.com/jirihybek/unity-websocket-webgl
- `Dan.Net` also uses a modified version of the **Main Thread Manager** by `Bearded Man Studios, Inc. (Forge Networking)`.

## Installation

To install `Dan.Net` into your Unity project, you can either:

Add the following git URL to your Unity project via Package Manager (RECOMMENDED):

```
https://github.com/danqzq/dan.net.git?path=Assets/DanNet
```

Or simply download the latest release from the [Releases page](https://github.com/danqzq/dan.net/releases)

## Usage

For `Dan.Net` to work, you need to have a server running the `Dan.Net` server software. You can find the server software in the [Releases page](https://github.com/danqzq/dan.net/releases) available for download on Windows, macOS and Linux.
The server listens on port '3000'.
The performance of `Dan.Net` is dependent on the performance of the server, so make sure to scale your server according to your needs.

Upon hosting the server, enter its IP address or domain name in the `DanNetConfig` located in the `Resources` folder of the `Dan.Net` package.
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

- Call `DanNet.Connect()` to connect to the server.
- Call `DanNet.CreateOrJoinRoom(roomName, maxPlayers)` to create or join a room.
- Call `DanNet.JoinRoom(roomName)` to join a room. You would want to do this when the room is created.
- Call `DanNet.LeaveRoom()` to leave the room.
- Call `DanNet.Instantiate(prefabName, position, rotation)` to spawn a networked prefab at the specified position and rotation.



## Networking

There are several ways to use to approach networking with **`Dan.Net`**:

### Event-based Networking

`Dan.Net` provides an event-based networking system, allowing you to easily send and receive messages between clients.
`Dan.Net` events can be sent immediately, buffered, or synchronized across all clients.

To emit any kind of event, you must have a script component which derives from [`MonoBehaviourDanNet`](Docs/MonoBehaviourDanNet.md)

This kind of networking approach is suitable for turn-based games, where delays in sending and receiving messages are acceptable.

### Stream-based Networking

`Dan.Net` also provides a stream-based networking system, for real-time data transfer between clients, operating over a binary stream protocol.

To use the stream-based networking system, you must have a script component which derives from [`MonoBehaviourDanNet`](Docs/MonoBehaviourDanNet.md) and
implements the [`ISyncData`](Docs/ISyncData.md) interface.

This kind of networking approach is suitable for real-time games, where synchronization across all clients is important.
However, it is more network-intensive than the event-based networking system.

### Hybrid Networking

Both of the above systems can be used together, allowing for a hybrid networking system supporting
events and streams for maximum flexibility.

## Classes and Components

For a deeper dive into the classes and components of `Dan.Net`, refer to the Docs below:
- Main Components
  - [`DanNet (Main Class)`](Docs/DanNet.md)
  - [`Room`](Docs/Room.md)
  - [`SyncObject`](Docs/SyncObject.md)
  - [`MonoBehaviourDanNet`](Docs/MonoBehaviourDanNet.md)
  - [`MonoBehaviourDanNetCallbacks`](Docs/MonoBehaviourDanNetCallbacks.md)
  - [`DanNetEvent`](Docs/DanNetEvent.md)
- Stream-based Networking
  - [`ISyncData`](Docs/ISyncData.md)
  - [`SyncDataStream`](Docs/SyncDataStream.md)
  - [`TransformSync`](Docs/TransformSync.md)
- Configuration
  - [`DanNetConfig`](Docs/DanNetConfig.md)
- Internal Classes
  - [`StreamManager`](Docs/StreamManager.md)
  - [`SyncObjectManager`](Docs/SyncObjectManager.md)

## License

`Dan.Net` is licensed under the [MIT License](LICENSE)

## And finally...

For feedback and reviews, please, feel free to raise an issue or [email me](mailto:dan@danqzq.games).

> Hope you enjoy using this little library!
> Made with lots of love 💖 and caffeine ☕👾 - [*Dan*](https://www.danqzq.games)
