using UnityEngine;

/// <summary>The inspectable note that reveals the first-floor solution.</summary>
public class ClockClue : Interactable
{
    private PrototypeDirector director;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        prompt = "Read the clock note";
    }

    public override void Interact(PlayerController player)
    {
        GameState.Instance.InspectClockClue();
        director.ShowMessage("The note reads: SET THE CLOCKS IN THIS ORDER — III, I, II.", 8f);
        director.PlayClueSound();
    }
}
