using UnityEngine;

/// <summary>Stores the small amount of state that must survive room transitions.</summary>
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool ClockPuzzleSolved { get; private set; }
    public bool DoorOpened { get; private set; }
    public bool OfficePuzzleSolved { get; private set; }
    public bool OfficeDoorOpened { get; private set; }
    public bool InspectedClockClue { get; private set; }
    public bool InspectedHallwayClue { get; private set; }
    public int HallwayChangesFound { get; private set; }
    public bool HallwaySolved => HallwayChangesFound == 3;
    public bool RunFinished { get; private set; }
    public int CurrentFloor { get; private set; }

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
    public void CompleteOfficePuzzle() => OfficePuzzleSolved = true;
    public void OpenOfficeDoor() => OfficeDoorOpened = true;
    public void InspectClockClue() => InspectedClockClue = true;
    public void EnterFloor(int floorNumber) => CurrentFloor = floorNumber;
    public void InspectHallwayClue() => InspectedHallwayClue = true;
    public void RecordHallwayChange() => HallwayChangesFound = Mathf.Min(3, HallwayChangesFound + 1);

    public void CompleteRun() => RunFinished = HallwaySolved;

    public void ResetRun()
    {
        ClockPuzzleSolved = false;
        DoorOpened = false;
        OfficePuzzleSolved = false;
        OfficeDoorOpened = false;
        InspectedClockClue = false;
        InspectedHallwayClue = false;
        HallwayChangesFound = 0;
        RunFinished = false;
        CurrentFloor = 0;
    }
}
