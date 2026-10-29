using UnityEngine;

public class OfficeButton : Interactable
{
    private OfficePuzzle puzzle;
    private int index;
    private SpriteRenderer face;

    public void Configure(OfficePuzzle owner, int buttonIndex)
    {
        puzzle = owner;
        index = buttonIndex;
        face = GetComponent<SpriteRenderer>();
        prompt = "Use this object";
    }

    public void SetSelected(bool selected)
    {
        if (face != null)
            face.color = selected
                ? new Color(0.65f, 1f, 0.65f)
                : Color.white;
    }

    public override void Interact(PlayerController player)
    {
        puzzle.Press(index);
    }
}
