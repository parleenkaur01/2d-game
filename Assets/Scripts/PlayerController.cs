using UnityEngine;

/// <summary>Small side-view controller for the first prototype slice.</summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    // Interactable controls are mounted on the elevator wall above the player.
    // This range lets the player reach them without requiring vertical movement.
    [SerializeField] private float interactionDistance = 2.8f;
    private Rigidbody2D body;
    private Interactable nearbyInteractable;
    private bool movementEnabled = true;
    private bool officeMovement;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
    }

    private void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"),
            officeMovement ? Input.GetAxisRaw("Vertical") : 0f);
        body.linearVelocity = movementEnabled ? direction.normalized * moveSpeed : Vector2.zero;

        if (!movementEnabled)
        {
            nearbyInteractable = null;
            return;
        }

        FindNearbyInteractable();

        if (officeMovement && Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Collider2D hit in Physics2D.OverlapPointAll(point))
            {
                Interactable target = hit.GetComponent<Interactable>();
                if (target == null || !target.canInteract) continue;
                if (Vector2.Distance(transform.position, target.transform.position) <= interactionDistance)
                    target.Interact(this);
                else
                    FindFirstObjectByType<PrototypeDirector>().ShowMessage("Move closer to that object, then click it.", 3f);
                return;
            }
        }

        if (nearbyInteractable != null && (Input.GetKeyDown(KeyCode.E) ||
            (!officeMovement && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))) ||
            Input.GetKeyDown(KeyCode.Space)))
        {
            nearbyInteractable.Interact(this);
        }
    }

    public void EnableOfficeMovement() => officeMovement = true;

    private void FixedUpdate()
    {
        if (!officeMovement) return;
        Vector2 position = body.position;
        position.x = Mathf.Clamp(position.x, -7.2f, 7.2f);
        position.y = Mathf.Clamp(position.y, -2.2f, 1.4f);
        body.position = position;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled && body != null) body.linearVelocity = Vector2.zero;
    }

    public void SetInteractionDistance(float distance)
    {
        interactionDistance = distance;
    }

    private void FindNearbyInteractable()
    {
        nearbyInteractable = null;
        float closestDistance = interactionDistance;

        foreach (Interactable candidate in FindObjectsOfType<Interactable>())
        {
            if (!candidate.canInteract) continue;
            float distance = Vector2.Distance(transform.position, candidate.transform.position);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                nearbyInteractable = candidate;
            }
        }
    }

    private void OnGUI()
    {
        if (nearbyInteractable == null) return;
        GUI.Box(new Rect(Screen.width / 2f - 160f, Screen.height - 90f, 320f, 45f),
            (officeMovement ? "Click / E / Space: " : "E / W / Space: ") + nearbyInteractable.GetPrompt());
    }
}
