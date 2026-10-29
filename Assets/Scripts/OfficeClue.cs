using UnityEngine;

public class OfficeClue : Interactable
{
    private PrototypeDirector director;
    private OfficePuzzle puzzle;

    public void Configure(PrototypeDirector owner, OfficePuzzle officePuzzle)
    {
        director = owner;
        puzzle = officePuzzle;
        prompt = "Examine the torn sketch";
    }

    public override void Interact(PlayerController player)
    {
        GameState.Instance.InspectOfficeClue();
        puzzle.RevealHint();
        director.ShowMessage("Click key → table → pen. Move close to each upside-down object, then click it.", OfficePuzzle.HintDuration);
        director.PlayClueSound();
    }
}
