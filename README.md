# The Last Elevator

Unity 2D prototype for the individual 2D game project.

## Current vertical slice

The playable slice starts inside the elevator. Examine the warning on the left, press the glowing button on the right, and ride to **Floor 1: The Clock Room**. Read the note on the left wall, set the labeled clocks in order `III, I, II`, then use the unlocked elevator door.

Back in the elevator, press the glowing button to ride to **Floor 2: The Upside-Down Office**. The furniture and realistic object images stay upside down. Move in four directions using WASD or arrows. Walk near the torn paper on the left and click it for the hint: **Click key → table → pen**. Walk near each pictured object and click it in that order. A chair, notebook, and lamp provide extra choices; clicking one resets the attempt, just like clicking a required object out of order. The hint disappears after three seconds; click the paper again to reread it. The warning popup also closes after three seconds, restoring movement without making a choice automatically. Choose whether to trust it using the mouse or the `1`/`2` keys, or reopen it later if unanswered. Then return through the unlocked elevator door. Press `R` after Floor 2 to replay.

Controls:

- `A/D` or left/right arrows: move
- `E`, `W`, up arrow, or space: interact
- In Floor 2: `WASD` or arrows to move in four directions; mouse click, `E`, or space to interact nearby
- `1` or `2`, or mouse: choose an answer to the office warning
- `R`: replay after finishing Floor 2

## Running it

1. Open this folder as a Unity 2D project.
2. Open `Assets/Scenes/Main.unity`.
3. Press Play and click inside the Game window to give it keyboard focus. The prototype builds its rooms at runtime.

The visuals and simple sound effects are generated in code for this prototype. Authored art, Floor 3, pause controls, and the three final endings will follow in later milestones.
