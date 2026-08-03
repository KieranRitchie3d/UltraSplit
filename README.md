# UltraSplit 2.2

UltraSplit is a Windows utility for running games inside exact, resolution-safe areas of an ultrawide or super-ultrawide monitor.

It can automatically detect remembered games, remove the normal Windows frame, apply a saved layout profile and manage the taskbar while gaming.

---

## Supported monitor layouts

UltraSplit includes presets designed for:

- `5120×1440` super-ultrawide monitors
- `3440×1440` ultrawide monitors

Built-in profiles use standard gaming resolutions to prevent stretching, unusual aspect ratios or incorrect scaling.

### Included 5120×1440 profiles

| Profile | Layout |
|---|---|
| Native 32:9 | `5120×1440` |
| Ultrawide centre | `840 \| 3440×1440 \| 840` |
| QHD centre | `1280 \| 2560×1440 \| 1280` |
| Full HD centre | `1600 \| 1920×1080 \| 1600` |
| Ultrawide 1080p | `1280 \| 2560×1080 \| 1280` |
| Dual QHD | `2560×1440 \| 2560×1440` |

### Included 3440×1440 profiles

| Profile | Layout |
|---|---|
| Native ultrawide | `3440×1440` |
| QHD centre | `440 \| 2560×1440 \| 440` |
| Full HD centre | `760 \| 1920×1080 \| 760` |
| Ultrawide 1080p | `440 \| 2560×1080 \| 440` |
| Dual desktop | `1720 \| 1720` |

---

# Installation

## 1. Unblock the ZIP

Before extracting:

1. Right-click `UltraSplit-2.2.zip`.
2. Select **Properties**.
3. Tick **Unblock** if the option appears.
4. Press **Apply**.
5. Extract the complete folder.

Do not run the build script directly from inside the ZIP.

## 2. Build UltraSplit

Run:

```text
Build UltraSplit 2.2.cmd
```

The script creates:

```text
UltraSplit2.exe
```

UltraSplit requests administrator permission when launched. Approve the Windows UAC prompt.

Administrator mode is required because many games and launchers run with elevated permissions.

## 3. Start UltraSplit later

After building, launch either:

```text
UltraSplit2.exe
```

or:

```text
Run UltraSplit 2.2.cmd
```

---

# Interface overview

UltraSplit contains four pages:

1. **Home**
2. **Profiles & Layouts**
3. **Game Detection**
4. **Settings**

---

# Home

The Home page is used to select your monitor, choose a profile and apply it to a selected application.

## Target monitor

Choose the physical monitor UltraSplit should manage.

For the Philips Evnia 49M2C8900, select the display showing:

```text
5120×1440
```

Only profiles compatible with the selected monitor resolution are shown.

## Active profile

Choose the layout you want to use.

For a centred `3440×1440` game on a `5120×1440` monitor, select:

```text
3440×1440 Ultrawide Centre
```

This creates the following target:

```text
840×1440 desktop | 3440×1440 game | 840×1440 desktop
```

## Apply profile to selected app

This button applies the active profile to the application selected on the **Game Detection** page.

Before changing anything, UltraSplit shows a confirmation containing:

- process name;
- complete window title;
- selected profile;
- target X and Y position;
- target width and height.

Review this information carefully before pressing **Yes**.

UltraSplit applies the profile once. It does not permanently keep the application above other windows, and other applications can overlap it normally.

## Restore Everything

This restores all applications changed during the current UltraSplit session.

It restores:

- window title bars;
- resize frames;
- menus;
- DWM window-frame rendering;
- original size;
- original position;
- maximised state;
- original taskbar behaviour.

---

# Profiles & Layouts

This page combines profile selection and custom layout editing.

## Profile library

Select any profile from the list to immediately view:

- its visual split layout;
- zone names;
- exact pixel widths;
- game zone;
- game resolution;
- description;
- validation state.

## Built-in profiles

Built-in profiles are read-only to prevent accidental corruption.

They can be viewed and activated normally.

To modify one, select it and press:

```text
Duplicate to edit
```

UltraSplit creates an editable custom copy.

## Use selected profile

Makes the selected profile active.

The selected profile will then appear on the Home page.

## Duplicate to edit

Creates an editable copy of the selected profile.

Use this when you want to:

- change split widths;
- rename zones;
- change the game resolution;
- add or remove zones;
- create a new saved layout.

## Delete

Deletes the selected custom profile.

Built-in profiles cannot be deleted.

---

# Custom layout editor

## Visual split editor

The large preview shows the full width of the monitor.

Each block represents one horizontal zone.

The green block is the selected game zone.

For editable custom profiles, divider lines can be dragged left or right.

Dragging a divider changes the widths of the two neighbouring zones while preserving the total monitor width.

## Exact pixel editing

The zone table contains:

- **Zone name**
- **Width in pixels**

You can type exact values directly.

For a `5120×1440` monitor, all zone widths must total exactly:

```text
5120
```

Example:

```text
840 + 3440 + 840 = 5120
```

UltraSplit will not save an invalid profile.

## Game zone

Choose which horizontal zone contains the game.

The game resolution must fit inside that zone.

Example:

```text
Zone width: 3440
Game width: 3440
```

Valid.

```text
Zone width: 2560
Game width: 3440
```

Invalid.

## Game width and height

Enter the exact game-window resolution.

Recommended standard resolutions include:

```text
5120×1440
3440×1440
2560×1440
2560×1080
1920×1080
```

Avoid unusual game resolutions unless the game specifically supports them.

## Add split

Adds another horizontal zone.

UltraSplit takes space from the largest existing zone so the total width does not exceed the monitor width.

A profile supports up to six zones.

## Remove split

Removes the currently selected zone.

Its pixels are transferred to an adjacent zone so no monitor space is lost.

## Balance

Makes all zones approximately equal width.

Any remainder pixels are placed into the final zone so the total remains exact.

## Reload

Discards unsaved editor changes and reloads the stored version of the profile.

## Save custom profile

Saves the currently edited custom profile.

A profile can only be saved when:

- all zones total exactly the monitor width;
- the game width fits inside the selected game zone;
- the game height does not exceed the monitor height;
- the profile has a name.

---

# Game Detection

The Game Detection page controls which running application UltraSplit modifies.

## Select an application

1. Open the game.
2. Set it to **Windowed**, **Borderless** or **Borderless Windowed**, depending on which mode behaves correctly.
3. Open UltraSplit.
4. Go to **Game Detection**.
5. Press **Refresh**.
6. Select the exact game window from the dropdown.

UltraSplit intentionally does not automatically select the first application. This prevents accidentally modifying Discord, ChatGPT, a browser or another desktop application.

## Remember selected as game

Stores the selected process as a game.

Remembered games can be detected even when they are running in normal Windowed or Borderless mode.

This is the most reliable way to distinguish games from ordinary desktop applications.

## Force active profile now

Immediately applies the active profile to the selected application.

A confirmation window appears before any changes are made.

Use this when:

- the game was already open before UltraSplit;
- automatic detection did not trigger;
- the game recreated its window;
- you changed profile while the game was running.

## Forget selected game

Removes a process from the remembered-games list.

The application will no longer be automatically detected in Windowed or Borderless mode.

## Automatic detection options

### Detect remembered games

Automatically detects processes previously added using **Remember selected as game**.

This works with:

- Windowed games;
- Borderless games;
- Borderless Windowed games.

### Detect unknown fullscreen applications

Detects applications that genuinely cover the complete physical monitor and use fullscreen-style window settings.

Disable this option when you only want remembered games to be modified.

### Detection delay

Controls how long UltraSplit waits before applying a profile.

Recommended value:

```text
400 ms
```

Increase it if a game repeatedly changes its window during startup.

---

# Recommended game setup

## Games with working Windowed mode

Use:

```text
Display Mode: Windowed
```

UltraSplit removes the title bar and resize frame, then applies the exact profile resolution.

This is useful when the game's built-in Borderless mode always uses the full `5120×1440` desktop.

## Games with working Borderless mode

Use:

```text
Display Mode: Borderless Windowed
```

UltraSplit can resize the borderless window into the selected game area.

## Exclusive fullscreen

True exclusive fullscreen cannot usually be resized by a normal Windows desktop utility.

Use Windowed or Borderless mode instead.

---

# Settings

## Remove Windows frame

Removes:

- title bar;
- borders;
- resize frame;
- window menu;
- DWM non-client frame.

Keep this enabled for a clean game window.

## Apply profile once

UltraSplit applies the profile once when:

- automatic detection triggers;
- **Force active profile now** is pressed;
- **Apply profile to selected app** is pressed.

It does not continuously force the game's size, position or z-order.

Other applications can overlap the game normally.

## Taskbar overlay mode

When enabled:

- the taskbar hides while a captured game has focus;
- the taskbar appears when another application receives focus;
- side applications can use the full `1440 px` height;
- Windows no longer reserves a taskbar-sized gap at the bottom.

## Notifications

Shows confirmation and error pop-ups when actions are performed.

Important errors are still shown even when normal notifications are disabled.

## Run on Windows startup

Tick:

```text
Run UltraSplit automatically when I sign in to Windows
```

Then press **Save all settings**.

Because UltraSplit always runs as administrator, Windows may show a UAC prompt when it starts.

---

# FancyZones setup

UltraSplit positions the game. FancyZones can be used for side applications.

For the recommended `3440×1440` centre profile on a `5120×1440` monitor, use:

```text
840 | 3440 | 840
```

FancyZones settings:

```text
Space around zones: 0 px
Distance between zones: 0 px
```

Use FancyZones for Discord, Spotify, browsers, ChatGPT and other desktop applications.

Do not use FancyZones to reposition the game after UltraSplit has applied its profile.

---

# Closing UltraSplit

Clicking the UltraSplit window's **X**:

1. restores every tracked application;
2. restores title bars and window frames;
3. restores original size and position;
4. restores the original taskbar state;
5. exits UltraSplit.

Minimising UltraSplit sends it to the system tray.

To exit from the tray:

1. Right-click the UltraSplit tray icon.
2. Select **Exit**.

---

# Hotkeys

```text
Ctrl + Alt + Enter
```

Applies the active profile to the currently selected application.

```text
Ctrl + Alt + Shift + R
```

Restores all modified windows and the original taskbar state.

---

# Troubleshooting

## Layout editor is empty

Select a profile from the Profile Library.

UltraSplit 2.2 loads the selected profile immediately.

## I cannot edit a profile

Built-in profiles are read-only.

Press:

```text
Duplicate to edit
```

Then edit the custom copy.

## The profile will not save

Check the validation message.

Common causes:

- zone widths do not total the monitor width;
- game width exceeds the selected game zone;
- game height exceeds the monitor height;
- profile name is empty.

## The wrong application was selected

UltraSplit displays a confirmation before applying a profile.

Press **No**, return to Game Detection and select the correct process/window.

## The title bar remains

Make sure UltraSplit is running as administrator.

Some games also recreate their window after changing display settings. Press **Refresh**, select the new live game window and apply the profile again.

## The taskbar leaves a gap at the bottom

Enable UltraSplit taskbar overlay mode.

Also set FancyZones spacing to zero and reapply the FancyZones layout.

## Restore Everything reports no tracked windows

UltraSplit can only restore windows modified during the current running session.

If an older UltraSplit version changed an application and was then closed incorrectly, fully exit and reopen that application once.

## The game has black bars

Check that:

- the selected profile uses a resolution supported by the game;
- the game's aspect-ratio setting matches the profile;
- the game is in Windowed mode when its Borderless mode forces full-monitor rendering;
- the selected game zone is exactly the required width.

Recommended profile:

```text
840 | 3440×1440 | 840
```

## The application cannot be modified

The application may be running at a higher privilege level.

UltraSplit 2.2 automatically requests administrator permission. Close and reopen UltraSplit if the UAC prompt was cancelled.

---

# Settings and profiles location

UltraSplit stores its settings and custom profiles at:

```text
%APPDATA%\UltraSplit 2\settings.json
```

Back up this file to preserve custom profiles.

Deleting it resets UltraSplit to its default settings and built-in profiles.

---

# Important limitations

UltraSplit modifies Windows application windows. It does not create genuine physical or virtual monitors.

Therefore:

- side areas do not receive separate Windows taskbars;
- exclusive fullscreen cannot normally be resized;
- game compatibility can vary;
- some anti-cheat or protected games may block external window changes;
- HDR, G-SYNC and Adaptive Sync remain properties of the complete physical monitor.

UltraSplit is designed to provide practical, resolution-safe window layouts on one ultrawide display without installing a display driver.
