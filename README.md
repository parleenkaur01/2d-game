# The Last Elevator

A small Unity 2D puzzle adventure by **Parleen Kaur**. Escape a mysterious elevator by completing three floors: a word scramble, an upside-down memory challenge, and a spot-the-difference puzzle.

The focus is exploration, observation, and approachable puzzles. There is no combat or time limit.

The player is a miniature explorer character with a walking bob, subtle idle breathing, left/right facing, and alternating footstep sounds. The character appears in the elevator and both explorable rooms.

## Run the game

1. Install **Unity 6000.6.2f1** (Unity 6.6) through Unity Hub.
2. Add this repository's folder as a project and open it.
3. Open [`Assets/Scenes/Main.unity`](Assets/Scenes/Main.unity).
4. Press **Play**, then click inside the **Game** window to give it keyboard focus.

The rooms and interface are created at runtime. An empty scene before pressing Play is expected. When scripts change, stop Play mode, allow Unity to compile, and start again.

## Game flow

### Opening elevator

Start in a dark elevator at B3. Follow the opening instruction: **Move right to the glowing button, then press E or Space.** The elevator moves with motor audio, vibration, changing floor numbers, a bell, and sliding doors.

### Floor 1 — The Clock Room

Open the letter board on the left, or interact with one of the stopped clocks. Solve three word scrambles using each round's riddle:

- Click letter tiles or type to build an answer.
- Check the word with **Enter** or the **Check Word** button.
- Use **Undo**, **Clear**, **Reshuffle**, or a first-letter **Hint** when needed.
- Click an answer slot to remove that letter.

Each solved word starts one clock. Closing the board preserves progress. Restore all three clocks, return to the room, and use the green elevator door. Press the glowing elevator button to continue.

### Floor 2 — The Upside-Down Office

Explore an office containing six realistic, upside-down objects. The memory pattern begins automatically when you enter.

Objects grow briefly and flash inside a yellow frame. The clear status strip is blue while the pattern plays, yellow on **YOUR TURN**, and green after completion. Wait until **YOUR TURN**, then walk near and click the objects in the same order. Three rounds increase from **two flashes** to **three**, then **four**.

- Press **R** to replay the current pattern as often as needed.
- A wrong click resets the current attempt, preserving completed rounds.
- Patterns are randomized for a new run.
- There is no time limit for repeating a pattern.

Complete all three rounds to unlock the elevator and continue to Floor 3.

The Floor 2 screen keeps the puzzle area clear: there is no warning popup, clue board, or large locked label. The exit door changes to green when the memory challenge is complete.

### Floor 3 — Fractured Memories

Compare the **ORIGINAL** picture with its altered copy. Three objects are missing from the altered picture. Find them by clicking their locations in **ORIGINAL**; the instructions do not name the answers.

- Find the differences in any order.
- Correct finds stay marked and cannot count twice.
- Wrong guesses have no penalty, and click targets are forgiving.
- **Hint** zooms toward an unfound object and highlights its area for three seconds without naming it.
- Zoom up to **4×** and pan both pictures together to inspect details.

## Escape and restart

Find all three differences on Floor 3 to see **ESCAPE SUCCESSFUL — You successfully escaped!** The pictures remain visible with the confirmed differences marked. There is no additional exit choice.

While Floor 3 is unfinished, the player is **still trapped**. Keep trying, use hints, or click **Start Again** to return to the opening elevator. Starting again clears progress on every floor. After escaping, **Start Again** or **R** begins a fresh run.

Progress is kept during the current run only; there is no save/load system.

## Controls

| Where | Controls |
| --- | --- |
| Elevator and Clock Room | **A/D** or **Left/Right** to move; **E**, **W**, **Up**, or **Space** to interact nearby |
| Word-scramble board | Click tiles or type letters; **Enter** to check; **Backspace** to undo; **Esc** to return to the room |
| Upside-Down Office | **WASD** or arrow keys to move; click nearby objects or use **E/Space**; press **R** to replay the pattern |
| Picture puzzle | Left-click answers in **ORIGINAL**; scroll or use zoom buttons; hold right mouse and drag to pan |
| Restart | **Start Again** on Floor 3; **R** after a successful escape |

**Esc is not a global pause key.** It closes the Floor 1 letter board. A full pause menu has not been implemented.

## Art and audio

Rooms use simple geometry alongside AI-generated artwork. Sound effects are synthesized at runtime, including player footsteps, elevator movement, button clicks, doors, bells, clock ticking, electrical ambience, and puzzle feedback. A short descending two-note buzz plays for an incorrect word, a wrong memory-pattern choice, or a wrong click in the picture puzzle. There is no continuous background music.

- [Office artwork and generation prompts](Assets/Resources/Office/ARTWORK.txt): key, table, pen, chair, notebook, and lamp cutouts.
- [Player artwork and generation prompt](Assets/Resources/Player/ARTWORK.txt): the transparent miniature explorer sprite.
- [Floor 3 artwork and generation prompts](Assets/Resources/Floor3/ARTWORK.txt): the reference illustration and its targeted removal edit. The game displays only the three edited patches over the original so incidental texture variations do not create extra differences.

## Project structure

| Path | Purpose |
| --- | --- |
| `Assets/Scenes/Main.unity` | Entry scene |
| `Assets/Scripts/PrototypeBootstrap.cs` | Creates the runtime camera and director |
| `Assets/Scripts/PrototypeDirector.cs` | Builds rooms, manages elevator travel, and restarts runs |
| `Assets/Scripts/WordScrambleState.cs` / `ClockPuzzle.cs` | Floor 1 letter rules and interface |
| `Assets/Scripts/MemoryPatternState.cs` / `OfficePuzzle.cs` | Floor 2 patterns, flashes, and progress |
| `Assets/Scripts/PictureDifferencePuzzle.cs` | Floor 3 images, zoom, hints, and answer selection |
| `Assets/Scripts/GameState.cs` | Puzzle progress for the current run |
| `Assets/Resources/` | Runtime-loaded artwork and asset notes |

## Current status

The prototype implements the opening elevator, miniature player with footsteps, all three puzzle floors, escape success, and restart. Development checks have covered Unity compilation, player presentation, word-tile rules, memory-round recovery, image loading and click mapping, completion gating, and resetting progress.

Remaining work includes a full manual playthrough, visual and audio polish, a pause menu, standalone build verification, gameplay recording, and submission documentation. No standalone build is included. Add `Main.unity` to the build's scene list before creating one.
