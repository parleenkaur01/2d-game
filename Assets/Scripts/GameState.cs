using UnityEngine;

/// <summary>Stores the small amount of state that must survive room transitions.</summary>
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool ClockPuzzleSolved { get; private set; }
    public bool DoorOpened { get; private set; }
    public bool InspectedElevatorWarning { get; private set; }
    public bool InspectedClockClue { get; private set; }
    public int CurrentFloor { get; private set; }
    public int TrustScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CompleteClockPuzzle() => ClockPuzzleSolved = true;
    public void OpenDoor() => DoorOpened = true;
    public void InspectElevatorWarning() => InspectedElevatorWarning = true;
    public void InspectClockClue() => InspectedClockClue = true;
    public void EnterFloor(int floorNumber) => CurrentFloor = floorNumber;
    public void AddTrust(int amount) => TrustScore += amount;

    public void ResetRun()
    {
        ClockPuzzleSolved = false;
        DoorOpened = false;
        InspectedElevatorWarning = false;
        InspectedClockClue = false;
        CurrentFloor = 0;
        TrustScore = 0;
    }
}
