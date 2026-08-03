# UltraSplit 2.2

## Fixed: empty Layout Editor

UltraSplit 2.1 only loaded an editor profile after **Duplicate and edit** was pressed. Opening the Layout Editor directly therefore displayed empty controls.

UltraSplit 2.2 merges the old Profiles and Layout Editor pages into one:

```text
Profiles & Layouts
```

## How the merged page works

- Selecting any profile immediately loads:
  - its description;
  - visual split preview;
  - zone names;
  - exact pixel widths;
  - game zone;
  - game width and height;
  - validation state.
- The active profile is selected automatically when UltraSplit opens.
- Selecting a profile on Home also selects it in Profiles & Layouts.
- Selecting **Use selected profile** updates Home immediately.

## Built-in profiles

Built-in presets are loaded as fully visible read-only previews.

Use:

```text
Duplicate to edit
```

to create a custom copy. The new copy is selected and made editable automatically.

## Custom profiles

Custom profiles support:

- drag-editing split dividers;
- exact pixel-width editing;
- zone-name editing;
- add split;
- remove split;
- balance;
- game-zone selection;
- exact game width and height;
- reload unsaved changes;
- save custom profile;
- delete custom profile.

## Width safety

Adding and removing splits preserves the total physical monitor width.

A profile cannot be saved unless:

- all zones total exactly the monitor width;
- the game fits inside the selected game zone;
- the game height fits inside the monitor.

## Upgrade

1. Exit UltraSplit 2.1.
2. Extract the complete UltraSplit 2.2 folder.
3. Run `Build UltraSplit 2.2.cmd`.
4. Approve the administrator prompt.

Existing profiles and settings remain in:

```text
%APPDATA%\UltraSplit 2\settings.json
```
