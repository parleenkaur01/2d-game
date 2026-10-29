using System.Collections.Generic;
using UnityEngine;

/// <summary>Tracks the three-clock sequence and updates its visible feedback.</summary>
public class ClockPuzzle : MonoBehaviour
{
    public static readonly int[] CorrectSequence = { 2, 0, 1 };
    private static readonly string[] ClockNames = { "I", "II", "III" };

    private readonly List<int> enteredSequence = new List<int>();
    private readonly ClockSwitch[] clocks = new ClockSwitch[3];
    private PrototypeDirector director;
    private SpriteRenderer doorRenderer;
    private Transform clueTransform;
    private GUIStyle clockLabelStyle;
    private float tickTimer = 1f;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
    }

    public void RegisterClock(int index, ClockSwitch clock)
    {
        clocks[index] = clock;
    }

    public void RegisterDoor(SpriteRenderer renderer)
    {
        doorRenderer = renderer;
    }

    public void RegisterClue(Transform clue)
    {
        clueTransform = clue;
    }

    public void PressClock(int clockIndex)
    {
        if (GameState.Instance.ClockPuzzleSolved) return;

        if (clockIndex != CorrectSequence[enteredSequence.Count])
        {
            enteredSequence.Clear();
            foreach (ClockSwitch clock in clocks)
                if (clock != null) clock.SetSelected(false);

            director.ShowMessage("Wrong order. The clocks reset. Look for the note on the left.", 4f);
            director.PlayPuzzleFailure();
            return;
        }

        enteredSequence.Add(clockIndex);
        clocks[clockIndex].SetSelected(true);
        director.PlayClockClick();

        if (enteredSequence.Count == CorrectSequence.Length)
        {
            GameState.Instance.CompleteClockPuzzle();
            if (doorRenderer != null)
                doorRenderer.color = new Color(0.25f, 0.62f, 0.47f);

            director.ShowMessage("All three clocks chime. The elevator door is unlocked!", 6f);
            director.PlayPuzzleSuccess();
        }
        else
        {
            director.ShowMessage("Clock " + ClockNames[clockIndex] +
                " is set. " + enteredSequence.Count + " of 3 clocks correct.", 3f);
        }
    }

    private void Update()
    {
        if (GameState.Instance.ClockPuzzleSolved) return;

        tickTimer -= Time.deltaTime;
        if (tickTimer > 0f) return;

        tickTimer = 1f;
        director.PlayClockTick();
    }

    private void OnGUI()
    {
        if (clockLabelStyle == null)
            clockLabelStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

        for (int i = 0; i < clocks.Length; i++)
        {
            if (clocks[i] == null) continue;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(
                clocks[i].transform.position + Vector3.up * 0.95f);
            GUI.Box(new Rect(screenPoint.x - 32f, Screen.height - screenPoint.y - 20f,
                64f, 36f), ClockNames[i], clockLabelStyle);
        }

        if (clueTransform != null)
        {
            Vector3 point = Camera.main.WorldToScreenPoint(clueTransform.position + Vector3.up);
            GUI.Box(new Rect(point.x - 42f, Screen.height - point.y - 20f,
                84f, 32f), "NOTE", clockLabelStyle);
        }

        if (doorRenderer != null)
        {
            Vector3 point = Camera.main.WorldToScreenPoint(
                doorRenderer.transform.position + Vector3.up * 2.4f);
            GUI.Box(new Rect(point.x - 55f, Screen.height - point.y - 20f,
                110f, 32f), GameState.Instance.ClockPuzzleSolved ? "OPEN" : "LOCKED",
                clockLabelStyle);
        }
    }
}
