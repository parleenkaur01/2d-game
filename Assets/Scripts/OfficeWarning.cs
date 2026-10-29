using UnityEngine;

public class OfficeWarning : Interactable
{
    private PrototypeDirector director;

    public void Configure(PrototypeDirector owner)
    {
        director = owner;
        prompt = "Read the warning";
    }

    public override void Interact(PlayerController player)
    {
        director.PresentOfficeWarning(player);
    }
}
