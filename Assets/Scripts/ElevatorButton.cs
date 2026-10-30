using UnityEngine;

public class ElevatorButton : Interactable
{
    private PrototypeDirector director;
    private int destinationFloor;

    public void Configure(PrototypeDirector owner, int floor)
    {
        director = owner;
        destinationFloor = floor;
        prompt = "Go to Floor " + floor;
    }

    public override void Interact(PlayerController player)
    {
        if (director == null || director.IsTransitioning) return;

        canInteract = false;
        director.PlayElevatorButtonSound();
        director.BeginFloor(player, destinationFloor);
    }
}
