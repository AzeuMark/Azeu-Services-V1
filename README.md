# Azeu Services V1
**An Intelligent Energy-Saving & Management System for Pisonet Businesses.**

Azeu Services V1 is a multi-layered solution designed to reduce electricity costs and streamline the management of computer rental (Pisonet) stations. It combines a robust Windows-based client with a real-time Node.js remote monitoring dashboard.

---

## 🚀 Core Features

### 1. Smart Anti-AFK & Auto-Shutdown
*   **Idle Detection:** Automatically detects when a PC is not in use.
*   **Suspicious Activity Detection:** Prevents users from bypassing the timer using macro software, auto-clickers, or weighted keys.
*   **Countdown HUD:** A transparent, top-most widget that informs users of the remaining time before shutdown.
*   **AFK Warning Dialog:** A large, centered alert that appears before the final shutdown to give active users a chance to resume.

### 2. Desktop Curfew System
*   **Automated Schedule:** Set "Closing" and "Opening" times for your business.
*   **Lock or Shutdown:** Choose to either lock the screen with a custom "Closed" image or perform a full system shutdown when curfew hits.
*   **Staff Bypass:** Admins can bypass the curfew on-site using a secure hotkey (`Ctrl + X`) and password.
*   **Remote Bypass:** Unlock a specific PC or all PCs simultaneously from the web dashboard.

### 3. Remote Management (WebSocket)
*   **Real-time Monitoring:** View AFK timers, system uptime, and lock status of every PC in the shop.
*   **Remote Capture:** Take instant screenshots of any PC to monitor user activity.
*   **Remote Power:** Shutdown or Restart PCs individually or via global broadcast.
*   **Communication:** Send pop-up messages to users or navigate their browsers to specific URLs remotely.

### 4. Compliance & Security
*   **No Smoking Dialog:** A customizable full-screen notice that appears upon system startup to enforce shop rules.
*   **Watchdog Service:** A persistent background script that ensures the application cannot be easily closed by tech-savvy users.
*   **Admin Authentication:** All settings and exit functions are protected by an encrypted password dialog.

---

## 🛠 System Architecture
*   **Client:** C# .NET Windows Forms (Win32 Hooks for input monitoring).
*   **Server:** Node.js + Express (Static hosting).
*   **Communication:** WebSocket (ws) for bi-directional real-time data.
*   **Persistence:** JSON-based local storage (`settings.json`).

---

## 📦 Installation

### Part 1: The Remote Server (Node.js)
1. Install [Node.js](https://nodejs.org/).
2. Navigate to your server folder.
3. Install dependencies:
   ```bash
   npm install express ws
   ```
4. Start the server:
   ```bash
   node server.js
   ```
5. Note the `Dashboard URL` and `C# Link` displayed in the console.

### Part 2: The Client (C# App)
1. Launch `AzeuServices_V1.exe`.
2. Authenticate using the default password: `1`.
3. Go to **Remote Settings**.
4. Enter the **WebSocket URL** (e.g., `ws://192.168.1.100:3000`).
5. Ensure the **Security Token** matches the server (default: `azeu_websocket_token`).
6. Click **Save Credentials**.

---

## ⚙️ Configuration Guide

### AFK Settings
*   **Countdown Threshold:** How many minutes of inactivity before shutdown.
*   **Warning Threshold:** How many seconds before shutdown the "AFK Detected" window appears.

### Curfew Settings
*   **Pisonet Close Time:** The time the PC becomes unusable.
*   **Pisonet Open Time:** The time the PC automatically unlocks.
*   **Action:** 
    *   *Shutdown*: Hard shutdown at closing time.
    *   *Show Image*: Display the "Closed" screen.

### Customizing Dialogs (Editors)
The app includes built-in editors for the **No Smoking** and **Curfew** screens. You can customize:
*   Background Colors and Images.
*   Font Sizes and Families.
*   Button Styles and Corner Radii.
*   Display durations.

---

## ⌨️ Shortcuts & Security
*   **Open Settings:** Double-click the tray icon or the Countdown HUD.
*   **Curfew Bypass:** Press `Ctrl + X` while the lock screen is active.
*   **Stealth Mode:** If "Minimize to Tray" is disabled while Anti-AFK is active, the app hides the tray icon to prevent tampering.

---

## 📁 Directory Structure
*   `/images/`: Stores custom background images for the lock screens.
*   `/websocket-cache/`: Temporary storage for screenshot capture and logs.
*   `settings.json`: The core configuration file for all app behaviors.
*   `watchdog.vbs`: The script responsible for keeping the service alive.

---

## ⚠️ Developer's Notice
This application was developed by **Uelmark G. Valdehueza**. 
If you encounter bugs, errors, or require custom features, please contact the developer via the Facebook link provided in the application's "Developer's Notice" panel.

---
*Copyright © 2024 Azeu Services. All Rights Reserved.*
