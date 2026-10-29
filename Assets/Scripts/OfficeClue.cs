using UnityEngine;

public class OfficeClue : Interactable
{
    private PrototypeDirector director;
    private OfficePuzzle puzzle;

    public void Configure(PrototypeDirector owner, OfficePuzzle officePuzzle)
    {
        director = owner;
        puzzle = officePuzzle;
        prompt = "Start / replay the memory pattern";
    }

    public override void Interact(PlayerController player)
    {
        GameState.Instance.InspectOfficeClue();
        puzzle.RevealHint();
        director.ShowMessage("Watch the highlighted objects, then click them in the same order.", 3f);
        director.PlayClueSound();
    }
}
