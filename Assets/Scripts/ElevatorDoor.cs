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
            Debug.Log("The elevator door will not open yet.");
            return;
        }

        GameState.Instance.OpenDoor();
        GameState.Instance.AddTrust(1);
        Object.FindFirstObjectByType<PrototypeDirector>().ShowEnding(
            "ENDING PREVIEW\nYou trusted the elevator and found a way forward.");
    }
}
