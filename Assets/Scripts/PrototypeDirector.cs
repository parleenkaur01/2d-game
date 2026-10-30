using System.Collections;
using UnityEngine;

/// <summary>
/// Builds the elevator, three floors, and the final completion screen.
/// Authored scenes and artwork can replace these generated rooms later.
/// </summary>
public class PrototypeDirector : MonoBehaviour
{
    private enum RoomPhase
    {
        Elevator,
        ClockRoom,
        Office,
        Hallway
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
    private PictureDifferencePuzzle picturePuzzle;
    private ClockPuzzle clockPuzzle;

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
        BuildElevatorRoom(0);
    }

    private void Update()
    {
        if (statusMessageTimer > 0f)
            statusMessageTimer -= Time.deltaTime;

        if (phase == RoomPhase.Hallway && GameState.Instance.RunFinished && Input.GetKeyDown(KeyCode.R))
            RestartGame();

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

    private void BuildElevatorRoom(int completedFloor)
    {
        BeginRoom(completedFloor == 0 ? "Opening Elevator" : "Elevator After Floor " + completedFloor);
        phase = RoomPhase.Elevator;
        GameState.Instance.EnterFloor(completedFloor);
        Camera.main.backgroundColor = new Color(0.025f, 0.03f, 0.05f);

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
        floorDisplayText = completedFloor == 0 ? "B3" : completedFloor.ToString();
        CreateBlock("Control Panel", new Vector2(3.25f, -0.2f), new Vector2(0.9f, 2.1f),
            new Color(0.28f, 0.29f, 0.32f), -1, false);

        if (completedFloor < 3)
        {
            GameObject button = CreateBlock("Illuminated Floor Button", new Vector2(3.25f, 0.15f),
                new Vector2(0.38f, 0.38f), new Color(0.9f, 0.65f, 0.12f), 1);
            elevatorButtonTransform = button.transform;
            elevatorButtonRenderer = button.GetComponent<SpriteRenderer>();
            button.AddComponent<ElevatorButton>().Configure(this, completedFloor + 1);
        }
        else
        {
            elevatorButtonTransform = null;
            CreateBlock("Completed Floor Button", new Vector2(3.25f, 0.15f),
                new Vector2(0.38f, 0.38f), new Color(0.22f, 0.45f, 0.33f), 1, false);
        }

        CreatePlayer(new Vector2(completedFloor == 0 ? -1.6f : 0f, -2.2f), 2.8f);
        ShowMessage(completedFloor == 0
            ? "Move right to the glowing button, then press E or Space."
            : completedFloor == 1
                ? "Floor 1 complete. Press the glowing button for Floor 2."
                : "Floor 2 complete. Press the glowing button for Floor 3.", 6f);
    }

    public void BeginFloor(PlayerController player, int destinationFloor)
    {
        if (IsTransitioning || destinationFloor < 1 || destinationFloor > 3) return;

        IsTransitioning = true;
        player.SetMovementEnabled(false);
        StartCoroutine(RideToFloor(destinationFloor));
    }

    private IEnumerator RideToFloor(int destinationFloor)
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

            if (destinationFloor == 1)
            {
                if (progress < 0.28f) floorDisplayText = "↑  B2";
                else if (progress < 0.58f) floorDisplayText = "↑  B1";
                else if (progress < 0.85f) floorDisplayText = "↑  G";
                else floorDisplayText = "↑  1";
            }
            else floorDisplayText = "↑  " + (progress < 0.65f ? destinationFloor - 1 : destinationFloor);

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

        floorDisplayText = destinationFloor.ToString();
        floorDisplayRenderer.color = new Color(0.95f, 0.18f, 0.08f);
        elevatorAudio.PlayBell();
        ShowMessage("DING — FLOOR " + destinationFloor + ". The doors are opening.", 3f);
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

        if (destinationFloor == 1) BuildClockRoom();
        else if (destinationFloor == 2) BuildOfficeRoom();
        else BuildHallway();
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

        GameObject puzzleObject = new GameObject("Clock Puzzle");
        puzzleObject.transform.SetParent(roomRoot.transform);
        ClockPuzzle puzzle = puzzleObject.AddComponent<ClockPuzzle>();
        clockPuzzle = puzzle;
        puzzle.Configure(this);
        puzzle.RegisterClue(clue.transform);
        clue.AddComponent<ClockClue>().Configure(this, puzzle);

        for (int i = 0; i < 3; i++)
        {
            Vector2 clockPosition = new Vector2(-2.35f + i * 2.25f, -0.55f);
            CreateBlock("Clock Rim " + (i + 1), clockPosition,
                new Vector2(1.5f, 1.5f), new Color(0.55f, 0.41f, 0.2f), -1, false);
            GameObject clock = CreateBlock("Clock " + (i + 1), clockPosition,
                new Vector2(1.25f, 1.25f), new Color(0.55f, 0.65f, 0.85f), 0, false);
            GameObject hand = CreateBlock("Clock Hand " + (i + 1), clockPosition + new Vector2(0f, 0.23f),
                new Vector2(0.07f, 0.4f), new Color(0.12f, 0.17f, 0.24f), 1, false);
            Transform handPivot = new GameObject("Clock Hand Pivot").transform;
            handPivot.SetParent(roomRoot.transform, false);
            handPivot.localPosition = clockPosition;
            hand.transform.SetParent(handPivot, true);
            ClockSwitch clockSwitch = clock.AddComponent<ClockSwitch>();
            clockSwitch.Configure(puzzle, i);
            clockSwitch.RegisterHand(handPivot);
            puzzle.RegisterClock(i, clockSwitch);
        }

        CreateBlock("Door Frame", new Vector2(6.15f, -0.8f), new Vector2(2.4f, 4.4f),
            new Color(0.48f, 0.42f, 0.33f), -2, false);
        GameObject door = CreateBlock("Exit Door", new Vector2(6f, -0.8f), new Vector2(2f, 4f),
            new Color(0.22f, 0.25f, 0.38f), -1, false);
        ElevatorDoor elevatorDoor = door.AddComponent<ElevatorDoor>();
        elevatorDoor.Configure(this, 1);
        puzzle.RegisterDoor(door.GetComponent<SpriteRenderer>());

        ShowMessage("The clocks have lost their words. Open the letter board on the left to restore them.", 3f);
    }

    private void BuildOfficeRoom()
    {
        BeginRoom("Upside-Down Office");
        phase = RoomPhase.Office;
        GameState.Instance.EnterFloor(2);
        Camera.main.backgroundColor = new Color(0.035f, 0.07f, 0.06f);
        elevatorAudio.StartOfficeAmbience();

        CreateBlock("Office Back Wall", Vector2.zero, new Vector2(16f, 7f),
            new Color(0.13f, 0.19f, 0.16f), -10, false);
        CreateBlock("Office Ceiling", new Vector2(0f, 2.45f), new Vector2(16f, 0.4f),
            new Color(0.28f, 0.34f, 0.29f), -5, false);
        CreateBlock("Office Floor", new Vector2(0f, -3.2f), new Vector2(16f, 0.5f),
            new Color(0.19f, 0.24f, 0.2f), -5);
        CreateBlock("Fluorescent Light", new Vector2(0f, 2.15f), new Vector2(2.8f, 0.12f),
            new Color(0.69f, 0.82f, 0.62f), -2, false);

        CreatePlayer(new Vector2(-6.1f, -2.2f), 1.8f).EnableOfficeMovement();

        GameObject puzzleObject = new GameObject("Office Button Puzzle");
        puzzleObject.transform.SetParent(roomRoot.transform);
        OfficePuzzle puzzle = puzzleObject.AddComponent<OfficePuzzle>();
        puzzle.Configure(this);

        Vector2[] objectPositions = {
            new Vector2(-1.9f, -1.5f), new Vector2(0.8f, 1f), new Vector2(3.6f, -1.5f),
            new Vector2(-2.8f, 0.4f), new Vector2(0.3f, -1.8f), new Vector2(4.1f, 0.95f)
        };
        float[] objectSizes = { 1.4f, 3.2f, 1.7f, 1.8f, 1.1f, 1.8f };
        for (int i = 0; i < objectPositions.Length; i++)
        {
            Vector2 position = objectPositions[i];
            GameObject button = CreateOfficePicture(roomRoot.transform, i, position, objectSizes[i]);
            OfficeButton officeButton = button.AddComponent<OfficeButton>();
            officeButton.Configure(puzzle, i);
            MakeOfficeClickable(button);
            // Thin items such as the pen get a comfortable mouse target.
            BoxCollider2D clickArea = button.GetComponent<BoxCollider2D>();
            Vector2 clickSize = clickArea.size;
            clickSize.x = Mathf.Max(clickSize.x, 0.65f / button.transform.localScale.x);
            clickArea.size = clickSize;
            puzzle.RegisterButton(i, officeButton);
        }

        puzzle.BeginChallenge();

        CreateBlock("Office Door Frame", new Vector2(6.1f, -0.8f), new Vector2(2.4f, 4.4f),
            new Color(0.41f, 0.48f, 0.35f), -2, false);
        GameObject door = CreateBlock("Office Elevator Door", new Vector2(6f, -0.8f),
            new Vector2(2f, 4f), new Color(0.22f, 0.31f, 0.29f), -1, false);
        door.AddComponent<ElevatorDoor>().Configure(this, 2);
        MakeOfficeClickable(door);
        puzzle.RegisterDoor(door.GetComponent<SpriteRenderer>());

    }

    private void MakeOfficeClickable(GameObject target)
    {
        target.AddComponent<BoxCollider2D>().isTrigger = true;
    }

    private void BuildHallway()
    {
        BeginRoom("Fractured Memories");
        phase = RoomPhase.Hallway;
        GameState.Instance.EnterFloor(3);
        ConfigureCamera();
        Camera.main.backgroundColor = Color.black;
        picturePuzzle = roomRoot.AddComponent<PictureDifferencePuzzle>();
        picturePuzzle.Configure(this);
        statusMessageTimer = 0f;
    }

    public void RestartGame()
    {
        IsTransitioning = false;
        elevatorAudio.StopOfficeAmbience();
        GameState.Instance.ResetRun();
        ConfigureCamera();
        BuildElevatorRoom(0);
    }

    private GameObject CreateOfficePicture(Transform parent, int index, Vector3 position,
        float maximumSize)
    {
        string[] filenames = { "key", "table", "pen", "chair", "notebook", "lamp" };
        Texture2D texture = Resources.Load<Texture2D>("Office/" + filenames[index]);
        GameObject picture = new GameObject("Upside-Down " + filenames[index]);
        picture.transform.SetParent(parent, false);
        picture.transform.localPosition = position;
        picture.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        SpriteRenderer renderer = picture.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        renderer.sortingOrder = 3;
        picture.transform.localScale = Vector3.one *
            (maximumSize * 100f / Mathf.Max(texture.width, texture.height));
        return picture;
    }

    public void ReturnFromFloor(PlayerController player, int floorNumber, Transform door)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;
        player.SetMovementEnabled(false);
        StartCoroutine(LeaveFloor(floorNumber, door));
    }

    private IEnumerator LeaveFloor(int floorNumber, Transform doorTransform)
    {
        ShowMessage("The unlocked elevator door opens. You step back inside.", 3f);
        elevatorAudio.PlayDoors();

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
        elevatorAudio.StopOfficeAmbience();
        BuildElevatorRoom(floorNumber);
        IsTransitioning = false;
    }

    private void BeginRoom(string roomName)
    {
        if (roomRoot != null)
        {
            // Stop the previous room's camera and input logic immediately.
            roomRoot.SetActive(false);
            Destroy(roomRoot);
        }
        roomRoot = new GameObject(roomName);
    }

    private PlayerController CreatePlayer(Vector2 position, float interactionRange)
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.transform.SetParent(roomRoot.transform, false);
        playerObject.transform.localPosition = position;
        playerObject.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 1.5f);
        PlayerController controller = playerObject.AddComponent<PlayerController>();
        controller.SetInteractionDistance(interactionRange);
        playerObject.AddComponent<PlayerAvatar>().Configure();
        return controller;
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
    public void PlayFootstep() => elevatorAudio.PlayFootstep();

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

        if (phase == RoomPhase.Hallway)
        {
            picturePuzzle.DrawGUI();
            return;
        }

        if (phase == RoomPhase.ClockRoom && clockPuzzle != null && clockPuzzle.IsOpen)
        {
            clockPuzzle.DrawGUI();
            return;
        }

        GUI.Label(new Rect(0f, 15f, Screen.width, 40f), "THE LAST ELEVATOR", titleStyle);

        string subtitle = phase == RoomPhase.Elevator
            ? (GameState.Instance != null && GameState.Instance.OfficeDoorOpened
                ? "Elevator — Floor 2 Complete"
                : GameState.Instance != null && GameState.Instance.DoorOpened
                    ? "Elevator — Floor 1 Complete" : "Inside the Elevator")
            : phase == RoomPhase.ClockRoom ? "Floor 1: The Clock Room"
                : phase == RoomPhase.Office ? "Floor 2: The Upside-Down Office"
                    : "Floor 3: Fractured Memories";
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
            GameState.Instance.OfficeDoorOpened
                ? "Press the glowing button to continue to Floor 3"
                : phase == RoomPhase.Office
                    ? "WASD / arrows: move   Mouse click / E / Space: interact"
                    : phase == RoomPhase.Hallway
                        ? "A/D / left-right: move   Mouse click / E / Space: examine"
                    : "A/D or arrows: move   E/W/Space: interact", bodyStyle);

    }
}
