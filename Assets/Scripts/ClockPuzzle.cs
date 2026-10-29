using UnityEngine;

/// <summary>Three word scrambles restore the clocks and unlock the elevator.</summary>
public class ClockPuzzle : MonoBehaviour
{
    private WordScrambleState words;
    private readonly ClockSwitch[] clocks = new ClockSwitch[3];
    private PrototypeDirector director;
    private SpriteRenderer doorRenderer;
    private Transform clueTransform;
    private PlayerController activePlayer;
    private GUIStyle labelStyle;
    private GUIStyle tileStyle;
    private GUIStyle bodyStyle;
    private float tickTimer = 1f;
    private float messageUntil;
    private string message = "";
    private int openedFrame;
    public bool IsOpen => activePlayer != null;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        words = new WordScrambleState();
    }
    public void RegisterClock(int index, ClockSwitch clock) => clocks[index] = clock;
    public void RegisterDoor(SpriteRenderer renderer) => doorRenderer = renderer;
    public void RegisterClue(Transform clue) => clueTransform = clue;

    public void Open(PlayerController player)
    {
        activePlayer = player;
        openedFrame = Time.frameCount;
        player.SetMovementEnabled(false);
        GameState.Instance.InspectClockClue();
    }

    private void Close()
    {
        if (activePlayer != null) activePlayer.SetMovementEnabled(true);
        activePlayer = null;
    }

    private void Update()
    {
        if (!words.Solved)
        {
            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0f) { tickTimer = 1f; director.PlayClockTick(); }
        }
        if (!IsOpen || openedFrame == Time.frameCount) return;
        if (Input.GetKeyDown(KeyCode.Escape)) { Close(); return; }
        if (words.Solved)
        {
            if (Input.GetKeyDown(KeyCode.Return)) Close();
            return;
        }
        foreach (char letter in Input.inputString)
        {
            if (letter == '\b') words.Undo();
            else if (letter == '\n' || letter == '\r') { Submit(); return; }
            else if (char.IsLetter(letter) && words.ChooseLetter(letter)) director.PlayClockClick();
        }
    }

    private void Submit()
    {
        if (!words.Submit())
        {
            Say("Not quite. Use the clue and rearrange your letters.");
            director.PlayPuzzleFailure();
            return;
        }
        clocks[words.SolvedCount - 1].SetSelected(true);
        director.PlayPuzzleSuccess();
        if (words.Solved)
        {
            GameState.Instance.CompleteClockPuzzle();
            doorRenderer.color = new Color(0.25f, 0.62f, 0.47f);
        }
        else Say("Correct! One clock starts ticking. Try the next word.");
    }

    private void Say(string text) { message = text; messageUntil = Time.unscaledTime + 3f; }

    public void DrawGUI()
    {
        EnsureStyles();
        Color oldColor = GUI.color;
        GUI.color = new Color(0.02f, 0.03f, 0.06f, 0.96f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = oldColor;
        Matrix4x4 oldMatrix = GUI.matrix;
        float scale = Mathf.Min((Screen.width - 20f) / 720f, (Screen.height - 20f) / 440f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 720f * scale) / 2f,
            (Screen.height - 440f * scale) / 2f), Quaternion.identity, Vector3.one * scale);
        GUI.Box(new Rect(0f, 0f, 720f, 440f), "");
        GUI.Label(new Rect(15f, 12f, 690f, 40f), "THE CLOCKMAKER'S LOST WORDS", labelStyle);
        GUI.Label(new Rect(15f, 55f, 690f, 30f), "Clocks restored: " + words.SolvedCount + " / 3", bodyStyle);
        if (words.Solved)
        {
            GUI.Label(new Rect(60f, 125f, 600f, 110f),
                "All three clocks are running!\nThe elevator door is unlocked.", labelStyle);
            GUI.Label(new Rect(60f, 250f, 600f, 55f), "Return to the room and use the green door on the right.", bodyStyle);
        }
        else
        {
            GUI.Label(new Rect(30f, 95f, 660f, 40f), words.Clue, bodyStyle);
            float tileWidth = 54f;
            float rowLeft = (720f - words.Tiles.Length * 62f + 8f) / 2f;
            string answer = words.Answer;
            for (int i = 0; i < words.Tiles.Length; i++)
            {
                if (GUI.Button(new Rect(rowLeft + i * 62f, 150f, tileWidth, 52f),
                    i < answer.Length ? answer[i].ToString() : "_", tileStyle)) words.RemoveAt(i);
                bool oldEnabled = GUI.enabled;
                GUI.enabled = oldEnabled && !words.IsUsed(i);
                if (GUI.Button(new Rect(rowLeft + i * 62f, 222f, tileWidth, 52f), words.Tiles[i].ToString(), tileStyle))
                {
                    words.Choose(i);
                    director.PlayClockClick();
                }
                GUI.enabled = oldEnabled;
            }
            if (GUI.Button(new Rect(55f, 300f, 100f, 32f), "Undo")) words.Undo();
            if (GUI.Button(new Rect(165f, 300f, 100f, 32f), "Clear")) words.Clear();
            if (GUI.Button(new Rect(275f, 300f, 100f, 32f), "Reshuffle")) words.Shuffle();
            if (GUI.Button(new Rect(385f, 300f, 100f, 32f), "Hint")) Say("The word starts with " + words.FirstLetter + ".");
            if (GUI.Button(new Rect(495f, 300f, 170f, 32f), "Check word  [Enter]")) Submit();
            GUI.Label(new Rect(25f, 342f, 670f, 40f),
                Time.unscaledTime < messageUntil ? message : "Click letters or type. Backspace undoes a letter.", bodyStyle);
        }
        if (GUI.Button(new Rect(250f, 395f, 220f, 30f), "Back to room  [Esc]")) Close();
        GUI.matrix = oldMatrix;
    }

    private void EnsureStyles()
    {
        if (labelStyle != null) return;
        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        tileStyle = new GUIStyle(GUI.skin.button) { fontSize = 27, alignment = TextAnchor.MiddleCenter };
        bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter, wordWrap = true };
    }

    private void OnGUI()
    {
        if (IsOpen) return;
        EnsureStyles();
        for (int i = 0; i < clocks.Length; i++)
            if (clocks[i] != null) DrawLabel(clocks[i].transform.position + Vector3.up * 0.95f,
                i < words.SolvedCount ? "RUNNING" : "WORD " + (i + 1), 115f);
        if (clueTransform != null) DrawLabel(clueTransform.position + Vector3.up, "LETTER BOARD", 150f);
        if (doorRenderer != null) DrawLabel(doorRenderer.transform.position + Vector3.up * 2.4f,
            words.Solved ? "OPEN" : "LOCKED", 115f);
    }

    private void DrawLabel(Vector3 position, string text, float width)
    {
        Vector3 point = Camera.main.WorldToScreenPoint(position);
        GUI.Box(new Rect(point.x - width / 2f, Screen.height - point.y - 16f, width, 32f), text);
    }
}
