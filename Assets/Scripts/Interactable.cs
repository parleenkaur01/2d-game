using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [TextArea] public string prompt = "Interact";
    public bool canInteract = true;

    public virtual string GetPrompt() => prompt;
    public abstract void Interact(PlayerController player);
}
