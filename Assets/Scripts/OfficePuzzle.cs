using System.Collections;
using UnityEngine;

/// <summary>Watch real office objects flash, then repeat increasingly long patterns.</summary>
public class OfficePuzzle : MonoBehaviour
{
    private readonly OfficeButton[] buttons = new OfficeButton[6];
    private MemoryPatternState memory;
    private PrototypeDirector director;
    private Transform warning;
    private SpriteRenderer door;
    private GUIStyle labelStyle;
    private bool showing;
    private bool ready;
    private int flashing = -1;
    private int flashStep;

    public void Configure(PrototypeDirector owner, Transform warningObject)
    {
        director = owner;
        warning = warningObject;
        memory = MemoryPatternState.CreateRandom();
    }

    public void RegisterButton(int index, OfficeButton button) => buttons[index] = button;
    public void RegisterDoor(SpriteRenderer renderer) => door = renderer;

    public void RevealHint()
    {
        if (showing || memory.Solved) return;
        GameState.Instance.InspectOfficeClue();
        StartCoroutine(ShowPattern(0.5f));
    }

    private IEnumerator ShowPattern(float delay)
    {
        showing = true;
        ready = false;
        flashStep = 0;
        memory.ResetAttempt();
        yield return new WaitForSeconds(delay);
        foreach (OfficeButton button in buttons) button.SetSelected(false);
        int[] pattern = memory.CurrentPattern;
        for (int step = 0; step < pattern.Length; step++)
        {
            flashStep = step + 1;
            flashing = pattern[step];
            buttons[flashing].SetFlashing(true);
            director.PlayClockClick();
            yield return new WaitForSeconds(0.8f);
            buttons[flashing].SetFlashing(false);
            flashing = -1;
            yield return new WaitForSeconds(0.3f);
        }
        showing = false;
        ready = true;
        director.ShowMessage("Your turn. Click the objects in the order they flashed.", 3f);
    }

    public void Press(int index)
    {
        if (memory.Solved) return;
        if (showing) return;
        if (!ready)
        {
            director.ShowMessage("Click Watch Pattern first, then repeat the flashes.", 3f);
            return;
        }

        MemoryResult result = memory.Press(index);
        if (result == MemoryResult.Wrong)
        {
            foreach (OfficeButton button in buttons) button.SetSelected(false);
            director.PlayPuzzleFailure();
            director.ShowMessage("Not quite. Retry this round from the start, or watch it again.", 3f);
            return;
        }

        buttons[index].SetSelected(true);
        director.PlayClockClick();
        if (result == MemoryResult.Accepted)
            director.ShowMessage(memory.InputPosition + " / " + memory.CurrentPattern.Length + " remembered.", 2f);
        else if (result == MemoryResult.RoundComplete)
        {
            director.PlayPuzzleSuccess();
            director.ShowMessage("Round complete! Watch the next, longer pattern.", 3f);
            StartCoroutine(ShowPattern(1.2f));
        }
        else
        {
            GameState.Instance.CompleteOfficePuzzle();
            door.color = new Color(0.28f, 0.62f, 0.44f);
            director.PlayPuzzleSuccess();
            director.ShowMessage("All three memory rounds complete. The elevator door unlocks!", 3f);
        }
    }

    private void OnGUI()
    {
        if (memory == null) return;
        if (labelStyle == null)
            labelStyle = new GUIStyle(GUI.skin.box)
                { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };

        string status = memory.Solved ? "Memory challenge complete!"
            : showing ? "WATCH • Round " + (memory.Round + 1) + "/3 • Flash " + flashStep + "/" + memory.CurrentPattern.Length
            : ready ? "YOUR TURN • Round " + (memory.Round + 1) + "/3 • Repeat " + memory.CurrentPattern.Length + " objects"
            : "Watch the objects flash, then repeat the pattern";
        GUI.Box(new Rect(20f, 100f, Screen.width - 220f, 38f), status, labelStyle);
        if (!memory.Solved)
        {
            bool oldEnabled = GUI.enabled;
            GUI.enabled = oldEnabled && !showing;
            if (GUI.Button(new Rect(Screen.width - 185f, 100f, 165f, 38f), ready ? "Watch Again" : "Watch Pattern")) RevealHint();
            GUI.enabled = oldEnabled;
        }

        if (flashing >= 0)
        {
            Bounds bounds = buttons[flashing].VisualBounds;
            Vector3 bottom = Camera.main.WorldToScreenPoint(bounds.min);
            Vector3 top = Camera.main.WorldToScreenPoint(bounds.max);
            Rect frame = new Rect(bottom.x - 8f, Screen.height - top.y - 8f,
                top.x - bottom.x + 16f, top.y - bottom.y + 16f);
            Color old = GUI.color;
            GUI.color = new Color(1f, 0.85f, 0.18f);
            GUI.DrawTexture(new Rect(frame.x, frame.y, frame.width, 4f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(frame.x, frame.yMax - 4f, frame.width, 4f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(frame.x, frame.y, 4f, frame.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(frame.xMax - 4f, frame.y, 4f, frame.height), Texture2D.whiteTexture);
            GUI.color = old;
        }
        if (warning != null) DrawLabel(warning.position + Vector3.up * 0.8f, "WARNING");
        if (door != null) DrawLabel(door.transform.position + Vector3.up * 2.4f, memory.Solved ? "OPEN" : "LOCKED");
    }

    private void DrawLabel(Vector3 position, string text)
    {
        Vector3 point = Camera.main.WorldToScreenPoint(position);
        GUI.Box(new Rect(point.x - 55f, Screen.height - point.y - 16f, 110f, 32f), text, labelStyle);
    }
}
