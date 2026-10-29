using UnityEngine;

public enum DifferenceResult { Found, AlreadyFound, Miss }

/// <summary>Tracks unique finds with forgiving click targets and no guessing penalty.</summary>
public class PictureDifferenceState
{
    private readonly Rect[] answers;
    private readonly bool[] found;
    public int FoundCount { get; private set; }
    public int Mistakes { get; private set; }
    public bool Solved => FoundCount == answers.Length;
    public bool IsFound(int index) => found[index];

    public PictureDifferenceState(Rect[] regions)
    {
        answers = regions;
        found = new bool[regions.Length];
    }

    public DifferenceResult Select(Vector2 point)
    {
        for (int i = 0; i < answers.Length; i++)
        {
            Rect target = answers[i];
            const float padding = 0.025f;
            target = Rect.MinMaxRect(target.xMin - padding, target.yMin - padding,
                target.xMax + padding, target.yMax + padding);
            if (!target.Contains(point)) continue;
            if (found[i]) return DifferenceResult.AlreadyFound;
            found[i] = true;
            FoundCount++;
            return DifferenceResult.Found;
        }
        Mistakes++;
        return DifferenceResult.Miss;
    }
}

/// <summary>Two-image comparison. Answer coordinates use the original image's top-left origin.</summary>
public class PictureDifferencePuzzle : MonoBehaviour
{
    // Calibrated to the final reference artwork, not the screen's resolution.
    public static readonly Rect[] AnswerRegions = {
        new Rect(188f / 1536f, 778f / 1024f, 104f / 1536f, 54f / 1024f),
        new Rect(1364f / 1536f, 306f / 1024f, 65f / 1536f, 52f / 1024f),
        new Rect(1384f / 1536f, 707f / 1024f, 64f / 1536f, 101f / 1024f)
    };
    private PictureDifferenceState progress;
    private PrototypeDirector director;
    private Texture2D reference;
    private Texture2D changed;
    private float zoom = 1f;
    private Vector2 focus = new Vector2(0.5f, 0.5f);
    private int hintIndex = -1;
    private float hintUntil;
    private string message = "You are still trapped. Find all three differences to escape. Use Hint if you need help.";
    private float messageUntil = 6f;
    private GUIStyle titleStyle;
    private GUIStyle textStyle;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        reference = Resources.Load<Texture2D>("Floor3/reference");
        changed = Resources.Load<Texture2D>("Floor3/changed");
        progress = new PictureDifferenceState(AnswerRegions);
        GameState.Instance.InspectHallwayClue();
        messageUntil = Time.unscaledTime + 6f;
    }

    public static Vector2 ScreenToImage(Vector2 point, Rect panel, Vector2 center, float scale)
    {
        return center + new Vector2((point.x - panel.x) / panel.width - 0.5f,
            (point.y - panel.y) / panel.height - 0.5f) / scale;
    }

    public void DrawGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 23, alignment = TextAnchor.MiddleCenter };
            textStyle = new GUIStyle(GUI.skin.label)
                { fontSize = 15, wordWrap = true, alignment = TextAnchor.MiddleCenter };
        }
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.blackTexture);
        GUI.Label(new Rect(10f, 8f, Screen.width - 20f, 36f),
            progress.Solved ? "ESCAPE SUCCESSFUL" : "FLOOR 3 — FRACTURED MEMORIES", titleStyle);
        GUI.Label(new Rect(10f, 43f, Screen.width - 20f, 30f),
            progress.Solved ? "You successfully escaped! All three floors completed."
                : "Find 3 differences to escape • Click in ORIGINAL • " + progress.FoundCount + " / 3 found", textStyle);

        if (reference == null || changed == null)
        {
            GUI.Label(new Rect(20f, 100f, Screen.width - 40f, 60f), "The comparison photographs could not be loaded.", textStyle);
            return;
        }

        float availableHeight = Mathf.Max(60f, Screen.height - 220f);
        float panelWidth = (Screen.width - 54f) / 2f;
        float panelHeight = Mathf.Min(availableHeight, panelWidth * reference.height / reference.width);
        panelWidth = panelHeight * reference.width / reference.height;
        float left = (Screen.width - panelWidth * 2f - 18f) / 2f;
        Rect first = new Rect(left, 98f, panelWidth, panelHeight);
        Rect second = new Rect(first.xMax + 18f, 98f, panelWidth, panelHeight);
        GUI.Label(new Rect(first.x, 74f, first.width, 24f), "ORIGINAL — click here", textStyle);
        GUI.Label(new Rect(second.x, 74f, second.width, 24f), "ALTERED — three objects missing", textStyle);

        HandleNavigation(first, second);
        DrawPicture(reference, first);
        DrawPicture(reference, second);
        DrawAlteredPatches(second);
        DrawFound(first);
        DrawFound(second);

        Event current = Event.current;
        if (!progress.Solved && current.type == EventType.MouseDown && current.button == 0)
        {
            if (first.Contains(current.mousePosition))
            {
                Select(ScreenToImage(current.mousePosition, first, focus, zoom));
                current.Use();
            }
            else if (second.Contains(current.mousePosition))
            {
                Say("Mark the missing object in ORIGINAL, on the left.");
                current.Use();
            }
        }

        float controlsY = first.yMax + 8f;
        if (!progress.Solved)
        {
            if (GUI.Button(new Rect(Screen.width / 2f - 220f, controlsY, 85f, 28f), "Zoom −")) ChangeZoom(zoom - 1f);
            GUI.Label(new Rect(Screen.width / 2f - 130f, controlsY, 50f, 28f), zoom + "×", textStyle);
            if (GUI.Button(new Rect(Screen.width / 2f - 75f, controlsY, 85f, 28f), "Zoom +")) ChangeZoom(zoom + 1f);
            if (GUI.Button(new Rect(Screen.width / 2f + 15f, controlsY, 70f, 28f), "Reset"))
            {
                focus = new Vector2(0.5f, 0.5f);
                zoom = 1f;
            }
            if (GUI.Button(new Rect(Screen.width / 2f + 90f, controlsY, 130f, 28f), "Hint")) ShowHint();
        }
        GUI.Label(new Rect(10f, controlsY + 31f, Screen.width - 20f, 24f),
            progress.Solved ? "Start Again or press R to play a new escape"
                : "Scroll to zoom • Hold right mouse and drag to pan both pictures", textStyle);

        string status = progress.Solved ? "The elevator doors open to freedom. You successfully escaped!"
            : Time.unscaledTime < messageUntil ? message
                : "You are still trapped. Keep looking, use Hint, or Start Again.";
        GUI.Label(new Rect(15f, Screen.height - 48f, Screen.width - (progress.Solved ? 30f : 200f), 42f), status, textStyle);
        if (!progress.Solved && GUI.Button(new Rect(Screen.width - 175f, Screen.height - 47f, 155f, 34f), "Start Again"))
        {
            director.RestartGame();
            return;
        }
        if (progress.Solved && GUI.Button(new Rect(Screen.width / 2f - 140f, controlsY, 280f, 30f), "Start Again"))
            director.RestartGame();
    }

    private void DrawPicture(Texture2D texture, Rect panel)
    {
        Rect uv = new Rect(focus.x - 0.5f / zoom, 1f - focus.y - 0.5f / zoom, 1f / zoom, 1f / zoom);
        GUI.DrawTextureWithTexCoords(panel, texture, uv);
    }

    private void DrawFound(Rect panel)
    {
        GUI.BeginGroup(panel);
        for (int i = 0; i < AnswerRegions.Length; i++)
        {
            bool hinted = i == hintIndex && Time.unscaledTime < hintUntil;
            if (!progress.IsFound(i) && !hinted) continue;
            Rect answer = AnswerRegions[i];
            Rect box = new Rect(((answer.x - focus.x) * zoom + 0.5f) * panel.width,
                ((answer.y - focus.y) * zoom + 0.5f) * panel.height,
                answer.width * zoom * panel.width, answer.height * zoom * panel.height);
            Color old = GUI.color;
            GUI.color = progress.IsFound(i) ? new Color(0.4f, 1f, 0.65f) : new Color(1f, 0.85f, 0.2f);
            GUI.DrawTexture(new Rect(box.x, box.y, box.width, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(box.x, box.yMax - 2f, box.width, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(box.x, box.y, 2f, box.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(box.xMax - 2f, box.y, 2f, box.height), Texture2D.whiteTexture);
            GUI.color = old;
        }
        GUI.EndGroup();
    }

    private void DrawAlteredPatches(Rect panel)
    {
        // Use the generated removal areas only. Every other displayed pixel comes
        // from the original, preventing incidental generative texture drift from
        // becoming additional, unclickable differences.
        GUI.BeginGroup(panel);
        foreach (Rect region in AnswerRegions)
        {
            Rect destination = new Rect(((region.x - focus.x) * zoom + 0.5f) * panel.width,
                ((region.y - focus.y) * zoom + 0.5f) * panel.height,
                region.width * zoom * panel.width, region.height * zoom * panel.height);
            Rect uv = new Rect(region.x, 1f - region.yMax, region.width, region.height);
            GUI.DrawTextureWithTexCoords(destination, changed, uv);
        }
        GUI.EndGroup();
    }

    private void HandleNavigation(Rect first, Rect second)
    {
        Event current = Event.current;
        Rect panel = first.Contains(current.mousePosition) ? first : second;
        if (!panel.Contains(current.mousePosition)) return;
        if (current.type == EventType.ScrollWheel)
        {
            focus = ScreenToImage(current.mousePosition, panel, focus, zoom);
            ChangeZoom(zoom + (current.delta.y < 0f ? 1f : -1f));
            current.Use();
        }
        else if (current.type == EventType.MouseDrag && current.button == 1)
        {
            focus -= new Vector2(current.delta.x / panel.width, current.delta.y / panel.height) / zoom;
            ClampFocus();
            current.Use();
        }
    }

    private void ChangeZoom(float value)
    {
        zoom = Mathf.Clamp(value, 1f, 4f);
        ClampFocus();
    }

    private void ClampFocus()
    {
        float margin = 0.5f / zoom;
        focus.x = Mathf.Clamp(focus.x, margin, 1f - margin);
        focus.y = Mathf.Clamp(focus.y, margin, 1f - margin);
    }

    private void Select(Vector2 point)
    {
        DifferenceResult result = progress.Select(point);
        if (result == DifferenceResult.Found)
        {
            GameState.Instance.RecordHallwayChange();
            director.PlayClueSound();
            Say("Difference found. " + progress.FoundCount + " / 3. Keep going to escape!");
            if (hintIndex >= 0 && progress.IsFound(hintIndex)) hintIndex = -1;
            if (progress.Solved)
            {
                GameState.Instance.CompleteRun();
                director.PlayPuzzleSuccess();
            }
        }
        else if (result == DifferenceResult.AlreadyFound) Say("You already found this difference.");
        else if (result == DifferenceResult.Miss)
        {
            Say("Not quite—you are still trapped. Try another spot or Start Again. Your finds are saved.");
        }
    }

    private void ShowHint()
    {
        for (int i = 0; i < AnswerRegions.Length; i++)
        {
            if (progress.IsFound(i)) continue;
            hintIndex = i;
            hintUntil = Time.unscaledTime + 3f;
            focus = AnswerRegions[i].center;
            ChangeZoom(2f);
            Say("Look inside the yellow box. Click the missing object in ORIGINAL.");
            return;
        }
    }

    private void Say(string text)
    {
        message = text;
        messageUntil = Time.unscaledTime + 3f;
    }
}
