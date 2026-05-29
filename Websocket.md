# WebSocket Communication Protocol

This document describes how the real-time communication layer between the **Azeu Services Client (C#)** and the **Azeu Monitoring Server (Node.js)** is structured.

## 1. Overview
The system uses a **Hub-and-Spoke** architecture.
*   **The Server:** Acts as the central hub, managing connections and routing messages.
*   **The Client (PC):** Sends telemetry (telemetry) and receives administrative commands.
*   **The Dashboard (Web):** Displays the state of all PCs and issues commands.

## 2. Connection & Security
All connections must be established via the `ws://` protocol.
*   **Handshake:** Upon connection, a client must send an `IDENTITY` packet immediately.
*   **Security Token:** The server validates the `token` field against the `SECURITY_TOKEN` defined in `server.js`. If they do not match, the connection is terminated instantly.

---

## 3. Client-to-Server Events (C# App)

### `IDENTITY`
Sent once upon successful connection.
```json
{
  "type": "IDENTITY",
  "pc_name": "DESKTOP-ABC123",
  "token": "azeu_websocket_token",
  "status": "Online"
}
```

### `STATUS_UPDATE`
Sent every 1 second via the C# UI Timer.
```json
{
  "type": "STATUS_UPDATE",
  "pc_name": "DESKTOP-ABC123",
  "countdown": "04:59",
  "uptime": "02h 15m 30s",
  "isLocked": false
}
```

### `SCREENSHOT_DATA`
Sent when requested by the dashboard or upon initial connection.
```json
{
  "type": "SCREENSHOT_DATA",
  "pc_name": "DESKTOP-ABC123",
  "image": "data:image/jpeg;base64,...",
  "date": "May 20, 2024",
  "time": "02:30:45 PM"
}
```

---

## 4. Dashboard-to-Server Events (Web UI)

### `DASHBOARD_LOGIN`
Sent by the browser to register as an administrative observer.
```json
{ "type": "DASHBOARD_LOGIN" }
```

### `COMMAND`
Sent when an admin interacts with the dashboard.
```json
{
  "type": "COMMAND",
  "target_pc": "DESKTOP-ABC123", 
  "action": "SHUTDOWN",
  "payload": ""
}
```
*Note: If `target_pc` is set to `"ALL"`, the server broadcasts the command to every connected PC.*

---

## 5. Server-to-Client Events (C# App)

When the server receives a `COMMAND` from the dashboard, it strips the wrapper and sends a raw command object to the C# Client:
```json
{
  "command": "MESSAGE",
  "content": "Please finish your session in 5 minutes."
}
```

### Supported Commands:
| Command | Payload | Description |
| :--- | :--- | :--- |
| `SCREENSHOT` | N/A | Triggers the client to capture and upload a screen. |
| `SHUTDOWN` | N/A | Triggers the system shutdown function. |
| `RESTART` | N/A | Triggers the system restart function. |
| `BYPASS_CURFEW` | N/A | Remotely unlocks the PC if it is currently in Curfew mode. |
| `MESSAGE` | String | Displays a pop-up message on the user's screen. |
| `NAVIGATE` | URL | Forces the client to open a browser to a specific URL. |

---

## 6. Server-to-Dashboard Events (Web UI)

### `PC_LIST`
Sent to dashboards whenever a PC connects or disconnects.
```json
{
  "type": "PC_LIST",
  "pcs": [
    { "name": "PC-01", "countdown": "05:00", "isLocked": false, "fileName": "img.jpg" }
  ]
}
```

---

## 7. Operational Logic

### Connection Persistence
The C# Client uses an asynchronous `ConnectionLoop`. If the WebSocket enters a `Closed` or `Aborted` state, the client waits 10 seconds before attempting a new connection. This ensures the dashboard remains populated even after server reboots.

### Screenshot Handling
To prevent server storage bloat, the Node.js server follows a **One-PC-One-Image** rule. When a new `SCREENSHOT_DATA` packet arrives:
1. The server identifies the `pc_name`.
2. It deletes any existing `.jpg` files belonging to that specific PC name.
3. It saves the new Base64 string as a physical file.
4. It appends a timestamp `?t=...` to the image URL sent to the dashboard to bypass browser caching.

### Broadcast Filtering
The server implements logic to prevent redundant commands. For example, during a **"Bypass All"** broadcast, the server checks the `isLocked` status of each PC and only forwards the `BYPASS_CURFEW` command to machines that are actually locked.
