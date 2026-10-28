using UnityEngine;

/// <summary>Base class for objects the player can interact with from nearby.</summary>
public abstract class Interactable : MonoBehaviour
{
    [TextArea] public string prompt = "Interact";
    public bool canInteract = true;

    public virtual string GetPrompt() => prompt;
    public abstract void Interact(PlayerController player);
}
