using System.Collections;
using UnityEngine;

public class OfficePuzzle : MonoBehaviour
{
    private readonly OfficeButton[] buttons = new OfficeButton[6];
    private MemoryPatternState memory;
    private PrototypeDirector director;
    private SpriteRenderer door;
    private GUIStyle labelStyle;
    private bool showing;
    private bool ready;
    private int flashing = -1;
    private int flashStep;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        memory = MemoryPatternState.CreateRandom();
    }

    public void BeginChallenge()
    {
        StartCoroutine(ShowPattern(0.9f));
    }

    public void RegisterButton(int index, OfficeButton button) => buttons[index] = button;
    public void RegisterDoor(SpriteRenderer renderer) => door = renderer;

    public void RevealHint()
    {
        if (showing || memory.Solved) return;
        StartCoroutine(ShowPattern(0.5f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RevealHint();
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
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

        string status = memory.Solved ? "COMPLETE — The elevator is open"
            : showing ? "WATCH CLOSELY — Round " + (memory.Round + 1) + "/3 — Flash " + flashStep + "/" + memory.CurrentPattern.Length
            : ready ? "YOUR TURN — Repeat " + memory.CurrentPattern.Length + " objects in order   •   R: Replay"
            : "WATCH THE OBJECTS — Then repeat them in the same order";
        labelStyle.normal.textColor = memory.Solved
            ? new Color(0.45f, 1f, 0.62f)
            : ready ? new Color(1f, 0.88f, 0.28f)
            : new Color(0.62f, 0.9f, 1f);

        Rect statusArea = new Rect(12f, 88f, Screen.width - 24f, 38f);
        Color oldColor = GUI.color;
        GUI.color = new Color(0.015f, 0.035f, 0.03f, 0.88f);
        GUI.DrawTexture(statusArea, Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(statusArea, status, labelStyle);
        GUI.color = oldColor;

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
        if (memory.Solved && door != null)
            door.color = new Color(0.28f, 0.62f, 0.44f);
    }
}
