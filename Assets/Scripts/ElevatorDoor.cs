using UnityEngine;

public class ElevatorDoor : Interactable
{
    private PrototypeDirector director;
    private int floorNumber;

    public void Configure(PrototypeDirector owner, int floor)
    {
        director = owner;
        floorNumber = floor;
    }

    private bool IsUnlocked => GameState.Instance != null &&
        (floorNumber == 1 ? GameState.Instance.ClockPuzzleSolved : GameState.Instance.OfficePuzzleSolved);

    public override string GetPrompt()
    {
        return IsUnlocked
            ? "Return to the elevator" : "The elevator is locked";
    }

    public override void Interact(PlayerController player)
    {
        if (!IsUnlocked)
        {
            director.ShowMessage(floorNumber == 1
                ? "The door is locked. Solve the three word scrambles to restore the clocks."
                : "The door is locked. Complete the three memory rounds first.", 4f);
            return;
        }

        canInteract = false;
        if (floorNumber == 1) GameState.Instance.OpenDoor();
        else GameState.Instance.OpenOfficeDoor();
        director.ReturnFromFloor(player, floorNumber, transform);
    }
}
