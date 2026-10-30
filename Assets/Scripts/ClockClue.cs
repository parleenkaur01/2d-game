using UnityEngine;

public class ClockClue : Interactable
{
    private PrototypeDirector director;
    private ClockPuzzle puzzle;

    public void Configure(PrototypeDirector owner, ClockPuzzle wordPuzzle)
    {
        director = owner;
        puzzle = wordPuzzle;
        prompt = "Open the letter board";
    }

    public override void Interact(PlayerController player)
    {
        GameState.Instance.InspectClockClue();
        director.PlayClueSound();
        puzzle.Open(player);
    }
}
