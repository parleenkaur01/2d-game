using UnityEngine;

/// <summary>Small side-view controller for the first prototype slice.</summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionDistance = 1.6f;
    private Rigidbody2D body;
    private Interactable nearbyInteractable;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, 0f);
        FindNearbyInteractable();

        if (nearbyInteractable != null && (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.Space)))
        {
            nearbyInteractable.Interact(this);
        }
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
            "E / W / Space: " + nearbyInteractable.GetPrompt());
    }
}
