using UnityEngine;

/// <summary>The illuminated button that starts the opening elevator ride.</summary>
public class ElevatorButton : Interactable
{
    private PrototypeDirector director;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        prompt = "Press the illuminated button";
    }

    public override void Interact(PlayerController player)
    {
        if (director == null || director.IsTransitioning) return;

        canInteract = false;
        director.PlayElevatorButtonSound();
        director.BeginFirstFloor(player);
    }
}
