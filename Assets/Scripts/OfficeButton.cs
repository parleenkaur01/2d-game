using UnityEngine;

public class OfficeButton : Interactable
{
    private OfficePuzzle puzzle;
    private int index;
    private SpriteRenderer face;
    private Vector3 restingScale;
    private bool selected;

    public void Configure(OfficePuzzle owner, int buttonIndex)
    {
        puzzle = owner;
        index = buttonIndex;
        face = GetComponent<SpriteRenderer>();
        restingScale = transform.localScale;
        prompt = "Use this object";
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        if (face != null)
            face.color = selected
                ? new Color(0.65f, 1f, 0.65f)
                : Color.white;
    }

    public void SetFlashing(bool flashing)
    {
        transform.localScale = restingScale * (flashing ? 1.15f : 1f);
        if (face != null) face.color = flashing ? new Color(1f, 0.94f, 0.55f)
            : selected ? new Color(0.65f, 1f, 0.65f) : Color.white;
    }

    public Bounds VisualBounds => face.bounds;

    public override void Interact(PlayerController player)
    {
        puzzle.Press(index);
    }
}
