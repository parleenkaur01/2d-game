using UnityEngine;

/// <summary>Explore the inverted office and touch the pictured objects in order.</summary>
public class OfficePuzzle : MonoBehaviour
{
    // Object indices: key, table, pen, chair, notebook, lamp.
    public static readonly int[] PictureOrder = { 0, 1, 2 };

    private int progress;
    private GameObject hintBoard;
    private float hintExpiresAt;
    public const float HintDuration = 3f;
    private readonly OfficeButton[] buttons = new OfficeButton[6];
    private PrototypeDirector director;
    private Transform warning;
    private SpriteRenderer door;
    private GUIStyle labelStyle;

    public void Configure(PrototypeDirector owner, Transform warningObject, GameObject hint)
    {
        director = owner;
        warning = warningObject;
        hintBoard = hint;
    }

    public void RevealHint()
    {
        hintExpiresAt = Time.unscaledTime + HintDuration;
        hintBoard.SetActive(true);
    }

    private void Update()
    {
        if (hintBoard != null && hintBoard.activeSelf && Time.unscaledTime >= hintExpiresAt)
            hintBoard.SetActive(false);
    }

    public void RegisterButton(int index, OfficeButton button) => buttons[index] = button;
    public void RegisterDoor(SpriteRenderer renderer) => door = renderer;

    public void Press(int index)
    {
        if (GameState.Instance.OfficePuzzleSolved) return;

        if (index != PictureOrder[progress])
        {
            progress = 0;
            foreach (OfficeButton button in buttons) button.SetSelected(false);
            director.PlayPuzzleFailure();
            director.ShowMessage("The mechanism resets. Follow the pictured order from the start.", 4f);
            return;
        }
        buttons[index].SetSelected(true);
        progress++;
        director.PlayClockClick();
        if (progress == PictureOrder.Length)
        {
            GameState.Instance.CompleteOfficePuzzle();
            foreach (int correctIndex in PictureOrder) buttons[correctIndex].SetSelected(true);
            door.color = new Color(0.28f, 0.62f, 0.44f);
            director.PlayPuzzleSuccess();
            director.ShowMessage("The mechanism clicks open. The elevator door unlocks.", 6f);
        }
        else
        {
            director.ShowMessage("A soft click. " + progress + " of 3 objects activated.", 2f);
        }
    }

    private void OnGUI()
    {
        if (labelStyle == null)
            labelStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

        if (warning != null) DrawLabel(warning.position + Vector3.up * 0.8f, "WARNING", 110f);
        if (hintBoard != null && hintBoard.activeSelf && Time.unscaledTime < hintExpiresAt)
            GUI.Box(new Rect(Screen.width / 2f - 210f, 100f, 420f, 35f),
                "Click key → table → pen", labelStyle);
        if (door != null)
            DrawLabel(door.transform.position + Vector3.up * 2.4f,
                GameState.Instance.OfficePuzzleSolved ? "OPEN" : "LOCKED", 110f);
    }

    private void DrawLabel(Vector3 worldPosition, string label, float width)
    {
        Vector3 point = Camera.main.WorldToScreenPoint(worldPosition);
        GUI.Box(new Rect(point.x - width / 2f, Screen.height - point.y - 16f,
            width, 32f), label, labelStyle);
    }
}
