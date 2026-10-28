using System.Collections.Generic;
using UnityEngine;

/// <summary>The first-floor puzzle: activate clocks in the order shown by the clue.</summary>
public class ClockPuzzle : MonoBehaviour
{
    public static readonly int[] CorrectSequence = { 2, 0, 1 };
    private readonly List<int> enteredSequence = new List<int>();
    private string message = "The clocks are stopped. A note reads: III, I, II.";
    private float messageTimer = 999f;

    public void PressClock(int clockIndex)
    {
        if (GameState.Instance.ClockPuzzleSolved) return;

        enteredSequence.Add(clockIndex);
        int position = enteredSequence.Count - 1;
        if (enteredSequence[position] != CorrectSequence[position])
        {
            enteredSequence.Clear();
            message = "The ticking resets. Try following the clue.";
            messageTimer = 3f;
            return;
        }

        if (enteredSequence.Count == CorrectSequence.Length)
        {
            GameState.Instance.CompleteClockPuzzle();
            message = "The clocks chime together. The elevator door unlocks.";
            messageTimer = 5f;
            return;
        }

        message = "A clock clicks into place...";
        messageTimer = 2f;
    }

    private void Update()
    {
        if (messageTimer > 0f) messageTimer -= Time.deltaTime;
    }

    private void OnGUI()
    {
        if (messageTimer <= 0f) return;
        GUI.Box(new Rect(20f, 20f, 420f, 55f), message);
    }
}
