# DanNetConfig

This class is a configuration class for **Dan.Net**. It is a `ScriptableObject` that can be created in the Unity Editor and saved as an asset.

## Properties

### Connection Settings

#### `serverUrl` (string)

The URL of the Dan.Net server. Default is `"localhost:3000"`.

#### `isSecure` (bool)

Determines whether to use a secure connection (wss/https) or not (ws/http). Default is `false`.

### Stream Settings

#### `dataSendRate` (float)

The rate at which data is sent to the server in seconds. Default is `20`.

## Unity Editor Tools

The DanNetConfig provides editor menu items:
- **Tools/DanNet/Show Config**: Opens the existing DanNetConfig asset in the inspector.
- **Tools/DanNet/Create Config**: Creates a new DanNetConfig asset in `Assets/Resources/DanNetConfig.asset` if one doesn't exist.
