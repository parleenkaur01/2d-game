using UnityEngine;

/// <summary>
/// Builds a playable placeholder scene in code until authored art and scenes are added.
/// </summary>
public class PrototypeDirector : MonoBehaviour
{
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private string endingMessage;

    private void Start()
    {
        if (GameState.Instance == null)
            new GameObject("Game State").AddComponent<GameState>();

        BuildPrototypeRoom();
    }

    private void BuildPrototypeRoom()
    {
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5f;
        Camera.main.backgroundColor = new Color(0.04f, 0.05f, 0.09f);

        CreateBlock("Floor", new Vector2(0f, -3.2f), new Vector2(16f, 0.5f), new Color(0.12f, 0.14f, 0.2f));
        GameObject backWall = CreateBlock("Back Wall", new Vector2(0f, 1.5f), new Vector2(16f, 7f), new Color(0.08f, 0.1f, 0.16f));
        Destroy(backWall.GetComponent<BoxCollider2D>());
        CreateBlock("Elevator Door", new Vector2(6f, -0.8f), new Vector2(2f, 4f), new Color(0.18f, 0.2f, 0.3f));

        GameObject playerObject = CreateBlock("Player", new Vector2(-5f, -2.2f), new Vector2(0.8f, 1.5f), new Color(0.8f, 0.75f, 0.55f));
        playerObject.AddComponent<PlayerController>();

        GameObject puzzleObject = new GameObject("Clock Puzzle");
        ClockPuzzle puzzle = puzzleObject.AddComponent<ClockPuzzle>();

        for (int i = 0; i < 3; i++)
        {
            GameObject clock = CreateBlock("Clock " + (i + 1), new Vector2(-1.5f + i * 2f, 0f),
                new Vector2(1.2f, 1.2f), new Color(0.55f, 0.65f, 0.85f));
            ClockSwitch clockSwitch = clock.AddComponent<ClockSwitch>();
            clockSwitch.Configure(puzzle, i);
        }

        GameObject door = CreateBlock("Exit Door", new Vector2(6f, -0.8f), new Vector2(2f, 4f), new Color(0.22f, 0.25f, 0.38f));
        ElevatorDoor elevatorDoor = door.AddComponent<ElevatorDoor>();
        elevatorDoor.prompt = "Try the elevator";
    }

    private GameObject CreateBlock(string objectName, Vector2 position, Vector2 size, Color color)
    {
        GameObject block = new GameObject(objectName);
        block.transform.position = position;
        block.transform.localScale = size;
        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateWhiteSprite();
        renderer.color = color;
        block.AddComponent<BoxCollider2D>();
        return block;
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    public void ShowEnding(string message) => endingMessage = message;

    private void OnGUI()
    {
        if (titleStyle == null)
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
        if (bodyStyle == null)
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };

        GUI.Label(new Rect(0f, 15f, Screen.width, 40f), "THE LAST ELEVATOR", titleStyle);
        GUI.Label(new Rect(0f, 55f, Screen.width, 35f), "Floor 1: The Clock Room", bodyStyle);
        GUI.Label(new Rect(0f, Screen.height - 45f, Screen.width, 30f), "A/D or arrows: move   E/W/Space: interact   Esc: pause", bodyStyle);

        if (!string.IsNullOrEmpty(endingMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 220f, Screen.height / 2f - 75f, 440f, 150f), "");
            GUI.Label(new Rect(Screen.width / 2f - 190f, Screen.height / 2f - 45f, 380f, 90f), endingMessage, bodyStyle);
        }
    }
}
