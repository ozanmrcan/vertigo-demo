# Wheel of Fortune

A wheel-of-fortune gambling mini-game made in Unity 2021.3 LTS, based on the Card Game in Critical Strike. Each zone you spin a wheel of rewards and one bomb — keep spinning to grow the pot and risk losing it all, or bank what you have on a safe/super zone.

- **Video:** https://youtu.be/1nwlGnC6QOI
- **APK:** on the [Releases](../../releases) page (portrait, Android 7.0+).

## Screenshots

<table>
  <tr>
    <td align="center">20:9<br><img src="Screenshots/20_9_silver.png" width="220"></td>
    <td align="center">16:9<br><img src="Screenshots/16_9_golden.png" width="220"></td>
    <td align="center">4:3<br><img src="Screenshots/4_3_silver.png" width="220"></td>
  </tr>
</table>

## Running it

- Editor: open in Unity 2021.3.45f2 and play `Assets/_Project/Scenes/Game.unity`.
- Device: install the APK from Releases.

## Code

Game rules live in plain C# under `Scripts/Core` (`GameSession`, `ZoneRules`, …) with no scene or UI dependencies, so they run under EditMode tests. The MonoBehaviours in `Scripts/UI` only display state and forward button clicks; ScriptableObjects in `Scripts/Data` hold the wheels, slices and rules that get edited in the Inspector. `GameManager` news up the session and wires it to the views. UI references are bound by name in `OnValidate`, and button listeners are added in code rather than the Inspector.
