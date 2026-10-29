# The Last Elevator

Unity 2D prototype for the individual 2D game project.

## Current vertical slice

The game starts inside the elevator. Examine the warning on the left, press the glowing button on the right, and ride to **Floor 1: The Clock Room**. Open the letter board on the left or interact with a clock. Solve three word scrambles of increasing length using each round's riddle. Click letter tiles or type to assemble an answer; Enter checks it and Backspace undoes a letter. Undo, clear, reshuffle, and a first-letter hint are also available. Each solved word starts a clock. Progress survives closing the board with Esc. When all three clocks are restored, return through the unlocked elevator door.

Back in the elevator, press the glowing button to ride to **Floor 2: The Upside-Down Office**. The six realistic object images stay upside down. Move in four directions using WASD or arrows. Click Watch Pattern or examine the paper to start a memory challenge. Objects briefly grow and gain a bright yellow frame as they flash. Repeat the pattern by walking near and clicking each object. Three rounds contain two, three, and four flashes, with new patterns each run. Watch Again replays the current round; a wrong click resets only that round's attempt, keeping completed rounds. There is no time limit. Completing all three rounds unlocks the elevator for Floor 3.

**Floor 3: Fractured Memories** is a relaxed image-comparison puzzle. Compare the pictures and find three missing objects by clicking their locations in the ORIGINAL picture. The instructions do not name the answers. Click targets are generous and wrong guesses have no penalty. The Hint button zooms toward an unfound object and highlights it in yellow for three seconds without naming it; hints can be reused freely. Use the mouse wheel or zoom buttons (up to 4×) and hold right mouse to pan both pictures together. Differences can be found in any order; correct finds remain marked and cannot count twice. Finding all three completes the game.

## Completion

Solving Floor 3 means escaping the elevator. After the third difference is found, the pictures stay on screen with **Escape Successful — You successfully escaped!** There is no additional route or ending choice. Until all three differences are found, the player is still trapped and can keep trying with hints or click **Start Again** to restart from the opening elevator. Wrong guesses do not force a restart or remove correct finds. After escaping, click Start Again or press `R` for a fresh run.

Controls:

- `A/D` or left/right arrows: move
- `E`, `W`, up arrow, or space: interact
- On the Floor 1 letter board: type letters or click tiles; Enter checks; Backspace undoes; Esc returns to the room
- In Floor 2: `WASD` or arrows to move in four directions; mouse click, `E`, or space to interact nearby
- `1` or `2`, or mouse: choose an answer to the office warning
- In Floor 3: left mouse marks differences in ORIGINAL; scroll to zoom; hold right mouse and drag to pan
- `R`: replay after completing Floor 3

## Running it

1. Open this folder as a Unity 2D project.
2. Open `Assets/Scenes/Main.unity`.
3. Press Play and click inside the Game window to give it keyboard focus. The prototype builds its rooms at runtime.

The prototype uses generated room geometry, original generated object images, and synthesized sound effects. Further art polish, pause controls, and submission materials remain.
