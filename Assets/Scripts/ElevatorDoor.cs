using UnityEngine;

public class ElevatorDoor : Interactable
{
    public override string GetPrompt()
    {
        return GameState.Instance != null && GameState.Instance.ClockPuzzleSolved
            ? "Return to the elevator" : "The elevator is locked";
    }

    public override void Interact(PlayerController player)
    {
        if (!GameState.Instance.ClockPuzzleSolved)
        {
            FindObjectOfType<PrototypeDirector>().ShowMessage(
                "The elevator door is locked. Set all three clocks first.", 4f);
            return;
        }

        canInteract = false;
        GameState.Instance.OpenDoor();
        FindObjectOfType<PrototypeDirector>().ReturnFromClockRoom(player);
    }
}
