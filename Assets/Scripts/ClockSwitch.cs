using UnityEngine;

public class ClockSwitch : Interactable
{
    private int clockIndex;
    private ClockPuzzle puzzle;

    public void Configure(ClockPuzzle owner, int index)
    {
        puzzle = owner;
        clockIndex = index;
        prompt = "Press clock " + (index + 1);
    }

    public override void Interact(PlayerController player)
    {
        if (puzzle != null) puzzle.PressClock(clockIndex);
    }
}
