# Potion Collector

A hyper-casual 3D game for Android where players collect randomly spawning potions to increase their score. The game is built with Unity and integrates Firebase for backend services and Addressables for dynamic asset management.

## Game Overview and Interaction Flow

- **Concept:** Potions appear at random locations on a grid.
- **Objective:** The player must tap on the potions to collect them before they disappear.
- **Scoring:** Each collected potion increases the player's score.
- **Endgame:** The game session ends, and the final score is saved to a global leaderboard.

The core interaction is simple and intuitive: players watch for potions and tap them to score points.

## Build Instructions

1.  **Unity Version:** This project was developed using **Unity 2022.3.32f1**. Please ensure you have this version installed.
2.  **Platform:** The target platform is **Android**.
3.  **Open Project:** Open the project in the Unity Hub.
4.  **Set Build Target:**
    *   Go to `File > Build Settings`.
    *   Select `Android` from the platform list and click `Switch Platform`.
5.  **Build APK:**
    *   In the `Build Settings` window, click `Build`.
    *   Choose a location to save the `.apk` file.

## Firebase Setup Guide

The project uses Firebase for anonymous authentication, Realtime Database (for scores and session data), and Analytics (for user properties).

1.  **Create Firebase Project:** Go to the [Firebase Console](https://console.firebase.google.com/) and create a new project.
2.  **Add Unity App:**
    *   Inside your project, add a new Unity application.
    *   Follow the setup steps to register the app. You will need to provide a package name (e.g., `com.yourcompany.potioncollector`).
3.  **Download Config Files:**
    *   Download the `google-services.json` file for Android.
    *   Download the `google-services-desktop.json` file for Unity Editor testing.
4.  **Place Config Files:**
    *   Place both `google-services.json` and `google-services-desktop.json` into the `Assets/StreamingAssets/` directory of your Unity project, replacing the placeholder files.
5.  **Realtime Database Rules:** For the leaderboard to work, ensure your Realtime Database rules allow public read access. For example:
    ```json
    {
      "rules": {
        ".read": "true",
        ".write": "true"
      }
    }
    ```

The `FirebaseManager.cs` script handles anonymous login, data synchronization, and analytics automatically.

## Addressables Usage Notes

The game uses the Addressables system to load potion data and prefabs asynchronously, allowing for dynamic updates without a new build.

-   **Asset Management:** The `AddressablesManager.cs` singleton manages loading and releasing addressable assets.
-   **Hosting:** The project is configured to fetch Addressables from a remote URL. You can use a service like [bunny.net](https://bunny.net/) or any other CDN.
    1.  Set up your hosting provider.
    2.  In Unity, go to `Window > Asset Management > Addressables > Profiles`.
    3.  Select the `Remote` profile and set the `RemoteLoadPath` to your CDN URL.
-   **Building Addressables:**
    1.  Go to `Window > Asset Management > Addressables > Groups`.
    2.  Click on `Build > New Build > Default Build Script`.
    3.  Upload the contents of the generated `ServerData` directory to your CDN.

## Event System Description

The project uses a custom, decoupled event system to manage game flow and communication between different managers.

-   **Core:** The `EventBusService.cs` (located in `Assets/ScriptSmith/`) is a static class that allows any part of the application to subscribe to, unsubscribe from, and invoke events.
-   **Usage:**
    *   `EventBusService.Subscribe<EventType>(Callback);`
    *   `EventBusService.UnSubscribe<EventType>(Callback);`
    *   `EventBusService.InvokeEvent(new EventType());`
-   **Event Definitions:** All game events are defined as classes in `Assets/Game/Scripts/GameEvents.cs`. Key events include:
    *   `GameStartedEvent`: Fired when the game session begins.
    *   `GameEndedEvent`: Fired when the game session ends, carrying the final score.
    *   `PotionSpawnedEvent`: Fired when a new potion appears.
    *   `PotionCollectedEvent`: Fired when a player taps a potion, carrying its type and value.
    *   `ScoreUpdatedEvent`: Fired to notify the UI of a score change.
    *   `LeaderboardLoadedEvent`: Carries the top scores fetched from Firebase.
    *   `FirebaseSyncStartedEvent` / `FirebaseSyncCompletedEvent`: Used to track the status of Firebase operations.

## Custom Potion Inspector

To streamline the management of potion data, a custom editor is provided for the `PotionScriptable` object.

-   **Script:** `Assets/Game/Scripts/Editor/PotionScriptableEditor.cs`
-   **Functionality:** When you select a `PotionScriptable` asset in the Unity Editor, the inspector provides a user-friendly interface for editing its properties:
    *   **Icon:** An object field for assigning the potion's sprite.
    *   **Name & Type:** Text fields for identification.
    *   **Score & Potency:** Sliders and integer fields for gameplay values.
    *   **Description:** A multi-line text area for a detailed description.

This custom inspector makes it easy to configure different potion types without needing to modify any code.
