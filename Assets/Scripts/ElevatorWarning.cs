using UnityEngine;

/// <summary>A warning inside the elevator that introduces examining clues.</summary>
public class ElevatorWarning : Interactable
{
    private PrototypeDirector director;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        prompt = "Examine the scratched note";
    }

    public override void Interact(PlayerController player)
    {
        if (GameState.Instance != null)
            GameState.Instance.InspectElevatorWarning();

        director?.ShowMessage(
            "The note reads: IF THE FLOOR NUMBER DISAPPEARS, DO NOT TRUST THE DOORS.",
            5f);
    }
}
