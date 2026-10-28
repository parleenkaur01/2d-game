using UnityEngine;

/// <summary>Stores the small amount of state that must survive room transitions.</summary>
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool ClockPuzzleSolved { get; private set; }
    public bool DoorOpened { get; private set; }
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
    public void AddTrust(int amount) => TrustScore += amount;

    public void ResetRun()
    {
        ClockPuzzleSolved = false;
        DoorOpened = false;
        TrustScore = 0;
    }
}
