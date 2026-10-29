using System.Collections;
using UnityEngine;

/// <summary>
/// Builds the opening elevator and the first playable floor from simple shapes.
/// Authored scenes and artwork can replace these generated rooms later.
/// </summary>
public class PrototypeDirector : MonoBehaviour
{
    private enum RoomPhase
    {
        Elevator,
        ClockRoom
    }

    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle floorDisplayStyle;
    private GameObject roomRoot;
    private Sprite whiteSprite;
    private ElevatorAudio elevatorAudio;
    private Transform leftElevatorDoor;
    private Transform rightElevatorDoor;
    private Transform elevatorButtonTransform;
    private GameObject elevatorDoorSeam;
    private SpriteRenderer elevatorButtonRenderer;
    private SpriteRenderer elevatorLightRenderer;
    private SpriteRenderer floorDisplayRenderer;
    private RoomPhase phase;
    private string statusMessage;
    private string floorDisplayText = "B3";
    private float statusMessageTimer;

    public bool IsTransitioning { get; private set; }

    private void Start()
    {
        if (GameState.Instance == null)
            new GameObject("Game State").AddComponent<GameState>();
        GameState.Instance.ResetRun();

        ConfigureCamera();
        elevatorAudio = gameObject.AddComponent<ElevatorAudio>();
        if (FindObjectOfType<AudioListener>() == null)
            Camera.main.gameObject.AddComponent<AudioListener>();
        BuildElevatorRoom(false);
    }

    private void Update()
    {
        if (statusMessageTimer > 0f)
            statusMessageTimer -= Time.deltaTime;

        if (phase == RoomPhase.Elevator && GameState.Instance != null &&
            GameState.Instance.DoorOpened && !IsTransitioning && Input.GetKeyDown(KeyCode.R))
        {
            GameState.Instance.ResetRun();
            BuildElevatorRoom(false);
        }

        if (phase == RoomPhase.Elevator && !IsTransitioning && elevatorButtonTransform != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.12f;
            elevatorButtonTransform.localScale = new Vector3(0.38f, 0.38f, 1f) * pulse;
            float glow = 0.72f + Mathf.Abs(Mathf.Sin(Time.time * 4f)) * 0.28f;
            elevatorButtonRenderer.color = new Color(1f, 0.55f * glow, 0.05f);
            elevatorLightRenderer.color = new Color(0.7f * glow, 0.75f * glow, 0.82f * glow);
        }
    }

    private void ConfigureCamera()
    {
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5f;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0.025f, 0.03f, 0.05f);
    }

    private void BuildElevatorRoom(bool returning)
    {
        BeginRoom(returning ? "Elevator After Floor 1" : "Opening Elevator");
        phase = RoomPhase.Elevator;
        GameState.Instance.EnterFloor(returning ? 1 : 0);

        CreateBlock("Elevator Back Wall", Vector2.zero, new Vector2(9f, 7f),
            new Color(0.07f, 0.08f, 0.11f), -10, false);
        GameObject ceilingLight = CreateBlock("Flickering Ceiling Light", new Vector2(0f, 2.3f),
            new Vector2(2.4f, 0.18f), new Color(0.7f, 0.75f, 0.82f), 0, false);
        elevatorLightRenderer = ceilingLight.GetComponent<SpriteRenderer>();
        CreateBlock("Elevator Floor", new Vector2(0f, -3.2f), new Vector2(9f, 0.45f),
            new Color(0.18f, 0.19f, 0.22f), -4);
        CreateBlock("Left Wall", new Vector2(-4.35f, 0f), new Vector2(0.35f, 7f),
            new Color(0.23f, 0.24f, 0.28f), -3);
        CreateBlock("Right Wall", new Vector2(4.35f, 0f), new Vector2(0.35f, 7f),
            new Color(0.23f, 0.24f, 0.28f), -3);

        // This blue opening is hidden while the doors are closed and becomes
        // visible as they slide apart at the destination.
        CreateBlock("Arrival Opening", new Vector2(0f, -0.1f), new Vector2(4.6f, 5.7f),
            new Color(0.035f, 0.08f, 0.14f), -7, false);
        leftElevatorDoor = CreateBlock("Left Door", new Vector2(-1.18f, -0.1f),
            new Vector2(2.28f, 5.7f), new Color(0.16f, 0.18f, 0.22f), -5, false).transform;
        rightElevatorDoor = CreateBlock("Right Door", new Vector2(1.18f, -0.1f),
            new Vector2(2.28f, 5.7f), new Color(0.14f, 0.16f, 0.2f), -5, false).transform;
        elevatorDoorSeam = CreateBlock("Door Seam", new Vector2(0f, -0.1f),
            new Vector2(0.07f, 5.7f), new Color(0.01f, 0.01f, 0.015f), -2, false);

        GameObject floorDisplay = CreateBlock("Floor Display", new Vector2(0f, 2.75f),
            new Vector2(1.35f, 0.5f), new Color(0.3f, 0.06f, 0.05f), -1, false);
        floorDisplayRenderer = floorDisplay.GetComponent<SpriteRenderer>();
        floorDisplayText = returning ? "1" : "B3";
        CreateBlock("Control Panel", new Vector2(3.25f, -0.2f), new Vector2(0.9f, 2.1f),
            new Color(0.28f, 0.29f, 0.32f), -1, false);

        if (!returning)
        {
            GameObject button = CreateBlock("Illuminated Floor Button", new Vector2(3.25f, 0.15f),
                new Vector2(0.38f, 0.38f), new Color(0.9f, 0.65f, 0.12f), 1);
            elevatorButtonTransform = button.transform;
            elevatorButtonRenderer = button.GetComponent<SpriteRenderer>();
            button.AddComponent<ElevatorButton>().Configure(this);
        }
        else
        {
            elevatorButtonTransform = null;
            CreateBlock("Floor One Button", new Vector2(3.25f, 0.15f),
                new Vector2(0.38f, 0.38f), new Color(0.22f, 0.45f, 0.33f), 1, false);
        }

        GameObject warning = CreateBlock("Scratched Warning", new Vector2(-3.1f, 0.1f),
            new Vector2(0.85f, 0.62f), new Color(0.72f, 0.68f, 0.52f), 1);
        warning.AddComponent<ElevatorWarning>().Configure(this);

        CreatePlayer(new Vector2(returning ? 0f : -1.6f, -2.2f), 2.8f);
        ShowMessage(returning
            ? "Floor 1 complete. Press R to replay while the next floor is being built."
            : "The elevator is silent. One button is still glowing.", 6f);
    }

    public void BeginFirstFloor(PlayerController player)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;
        player.SetMovementEnabled(false);
        StartCoroutine(RideToFirstFloor());
    }

    private IEnumerator RideToFirstFloor()
    {
        ShowMessage("The doors lock. The elevator begins moving upward...", 10f);
        elevatorAudio.PlayMotor();

        Vector3 restingPosition = roomRoot.transform.position;
        float elapsed = 0f;
        const float travelTime = 4.5f;
        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / travelTime;

            if (progress < 0.28f) floorDisplayText = "↑  B2";
            else if (progress < 0.58f) floorDisplayText = "↑  B1";
            else if (progress < 0.85f) floorDisplayText = "↑  G";
            else floorDisplayText = "↑  1";

            float horizontalShake = Mathf.Sin(elapsed * 27f) * 0.025f;
            float verticalRumble = Mathf.Sin(elapsed * 39f) * 0.07f;
            roomRoot.transform.position = restingPosition +
                new Vector3(horizontalShake, verticalRumble, 0f);

            float pulse = 0.55f + Mathf.Abs(Mathf.Sin(elapsed * 5f)) * 0.45f;
            floorDisplayRenderer.color = new Color(0.65f * pulse, 0.08f, 0.04f);
            float flicker = 0.35f + Mathf.Abs(Mathf.Sin(elapsed * 21f)) * 0.65f;
            elevatorLightRenderer.color = new Color(0.7f * flicker, 0.75f * flicker, 0.82f * flicker);
            yield return null;
        }

        roomRoot.transform.position = restingPosition + Vector3.up * 0.16f;
        yield return new WaitForSeconds(0.12f);
        roomRoot.transform.position = restingPosition + Vector3.down * 0.08f;
        yield return new WaitForSeconds(0.12f);
        roomRoot.transform.position = restingPosition;

        floorDisplayText = "1";
        floorDisplayRenderer.color = new Color(0.95f, 0.18f, 0.08f);
        elevatorAudio.PlayBell();
        ShowMessage("DING — FLOOR 1. The doors are opening.", 3f);
        yield return new WaitForSeconds(0.6f);

        Vector3 leftClosed = leftElevatorDoor.localPosition;
        Vector3 rightClosed = rightElevatorDoor.localPosition;
        Vector3 leftOpen = leftClosed + Vector3.left * 2.35f;
        Vector3 rightOpen = rightClosed + Vector3.right * 2.35f;
        elevatorAudio.PlayDoors();
        elevatorDoorSeam.SetActive(false);

        elapsed = 0f;
        const float doorTime = 1.25f;
        while (elapsed < doorTime)
        {
            elapsed += Time.deltaTime;
            float amount = Mathf.SmoothStep(0f, 1f, elapsed / doorTime);
            leftElevatorDoor.localPosition = Vector3.Lerp(leftClosed, leftOpen, amount);
            rightElevatorDoor.localPosition = Vector3.Lerp(rightClosed, rightOpen, amount);
            yield return null;
        }

        yield return new WaitForSeconds(0.65f);

        BuildClockRoom();
        IsTransitioning = false;
    }

    private void BuildClockRoom()
    {
        BeginRoom("Clock Room");
        phase = RoomPhase.ClockRoom;
        floorDisplayText = string.Empty;
        GameState.Instance.EnterFloor(1);
        Camera.main.backgroundColor = new Color(0.04f, 0.05f, 0.09f);

        CreateBlock("Back Wall", Vector2.zero, new Vector2(16f, 7f),
            new Color(0.08f, 0.1f, 0.16f), -10, false);
        CreateBlock("Upper Wall Trim", new Vector2(0f, 2.4f), new Vector2(16f, 0.16f),
            new Color(0.25f, 0.28f, 0.37f), -4, false);
        CreateBlock("Floor", new Vector2(0f, -3.2f), new Vector2(16f, 0.5f),
            new Color(0.12f, 0.14f, 0.2f), -4);
        CreateBlock("Clock Room Lamp", new Vector2(0f, 2.15f), new Vector2(2f, 0.15f),
            new Color(0.6f, 0.65f, 0.7f), -2, false);

        CreatePlayer(new Vector2(-6.1f, -2.2f), 2.15f);

        CreateBlock("Note Board", new Vector2(-6.15f, -0.65f), new Vector2(1.55f, 1.3f),
            new Color(0.25f, 0.2f, 0.17f), -1, false);
        GameObject clue = CreateBlock("Clock Clue Note", new Vector2(-6.15f, -0.65f),
            new Vector2(1.2f, 0.95f), new Color(0.79f, 0.75f, 0.58f), 1, false);
        clue.AddComponent<ClockClue>().Configure(this);

        GameObject puzzleObject = new GameObject("Clock Puzzle");
        puzzleObject.transform.SetParent(roomRoot.transform);
        ClockPuzzle puzzle = puzzleObject.AddComponent<ClockPuzzle>();
        puzzle.Configure(this);
        puzzle.RegisterClue(clue.transform);

        for (int i = 0; i < 3; i++)
        {
            Vector2 clockPosition = new Vector2(-2.35f + i * 2.25f, -0.55f);
            CreateBlock("Clock Rim " + (i + 1), clockPosition,
                new Vector2(1.5f, 1.5f), new Color(0.55f, 0.41f, 0.2f), -1, false);
            GameObject clock = CreateBlock("Clock " + (i + 1), clockPosition,
                new Vector2(1.25f, 1.25f), new Color(0.55f, 0.65f, 0.85f), 0, false);
            CreateBlock("Clock Hand " + (i + 1), clockPosition + new Vector2(0f, 0.23f),
                new Vector2(0.07f, 0.4f), new Color(0.12f, 0.17f, 0.24f), 1, false);
            ClockSwitch clockSwitch = clock.AddComponent<ClockSwitch>();
            clockSwitch.Configure(puzzle, i);
            puzzle.RegisterClock(i, clockSwitch);
        }

        CreateBlock("Door Frame", new Vector2(6.15f, -0.8f), new Vector2(2.4f, 4.4f),
            new Color(0.48f, 0.42f, 0.33f), -2, false);
        GameObject door = CreateBlock("Exit Door", new Vector2(6f, -0.8f), new Vector2(2f, 4f),
            new Color(0.22f, 0.25f, 0.38f), -1, false);
        ElevatorDoor elevatorDoor = door.AddComponent<ElevatorDoor>();
        elevatorDoor.prompt = "Try the elevator";
        puzzle.RegisterDoor(door.GetComponent<SpriteRenderer>());

        ShowMessage("Three stopped clocks. Read the note on the left to learn their order.", 7f);
    }

    public void ReturnFromClockRoom(PlayerController player)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;
        player.SetMovementEnabled(false);
        StartCoroutine(LeaveClockRoom());
    }

    private IEnumerator LeaveClockRoom()
    {
        ShowMessage("The unlocked elevator door opens. You step back inside.", 3f);
        elevatorAudio.PlayDoors();

        ElevatorDoor door = FindObjectOfType<ElevatorDoor>();
        Transform doorTransform = door.transform;
        Vector3 closedScale = doorTransform.localScale;
        float elapsed = 0f;
        while (elapsed < 1.1f)
        {
            elapsed += Time.deltaTime;
            float amount = Mathf.SmoothStep(0f, 1f, elapsed / 1.1f);
            doorTransform.localScale = Vector3.Lerp(closedScale,
                new Vector3(0.08f, closedScale.y, closedScale.z), amount);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        BuildElevatorRoom(true);
        IsTransitioning = false;
    }

    private void BeginRoom(string roomName)
    {
        if (roomRoot != null) Destroy(roomRoot);
        roomRoot = new GameObject(roomName);
    }

    private void CreatePlayer(Vector2 position, float interactionRange)
    {
        GameObject playerObject = CreateBlock("Player", position, new Vector2(0.8f, 1.5f),
            new Color(0.8f, 0.75f, 0.55f), 10);
        playerObject.AddComponent<PlayerController>().SetInteractionDistance(interactionRange);
    }

    private GameObject CreateBlock(string objectName, Vector2 position, Vector2 size,
        Color color, int sortingOrder, bool addCollider = true)
    {
        GameObject block = new GameObject(objectName);
        block.transform.SetParent(roomRoot.transform);
        block.transform.position = position;
        block.transform.localScale = size;

        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = GetWhiteSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        if (addCollider) block.AddComponent<BoxCollider2D>();
        return block;
    }

    private Sprite GetWhiteSprite()
    {
        if (whiteSprite != null) return whiteSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        whiteSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }

    public void ShowMessage(string message, float duration = 4f)
    {
        statusMessage = message;
        statusMessageTimer = duration;
    }

    public void PlayElevatorButtonSound()
    {
        elevatorAudio?.PlayButton();
    }

    public void PlayClockClick() => elevatorAudio.PlayClockClick();
    public void PlayPuzzleFailure() => elevatorAudio.PlayPuzzleFailure();
    public void PlayPuzzleSuccess() => elevatorAudio.PlayPuzzleSuccess();
    public void PlayClueSound() => elevatorAudio.PlayClue();
    public void PlayClockTick() => elevatorAudio.PlayClockTick();

    private void OnGUI()
    {
        if (titleStyle == null)
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter
            };
        if (bodyStyle == null)
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
        if (floorDisplayStyle == null)
            floorDisplayStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.35f, 0.15f) }
            };

        GUI.Label(new Rect(0f, 15f, Screen.width, 40f), "THE LAST ELEVATOR", titleStyle);

        string subtitle = phase == RoomPhase.Elevator
            ? (GameState.Instance != null && GameState.Instance.DoorOpened
                ? "Elevator — Floor 1 Complete" : "Inside the Elevator")
            : "Floor 1: The Clock Room";
        GUI.Label(new Rect(0f, 55f, Screen.width, 35f), subtitle, bodyStyle);

        if (phase == RoomPhase.Elevator)
            GUI.Label(new Rect(Screen.width / 2f - 90f, 90f, 180f, 45f),
                floorDisplayText, floorDisplayStyle);

        if (statusMessageTimer > 0f && !string.IsNullOrEmpty(statusMessage))
        {
            GUI.Box(new Rect(Screen.width / 2f - 250f, Screen.height - 165f, 500f, 60f), "");
            GUI.Label(new Rect(Screen.width / 2f - 225f, Screen.height - 157f, 450f, 45f),
                statusMessage, bodyStyle);
        }

        GUI.Label(new Rect(0f, Screen.height - 45f, Screen.width, 30f),
            phase == RoomPhase.Elevator && GameState.Instance != null &&
            GameState.Instance.DoorOpened
                ? "Floor 1 complete   R: replay from the beginning"
                : "A/D or arrows: move   E/W/Space: interact", bodyStyle);
    }
}
