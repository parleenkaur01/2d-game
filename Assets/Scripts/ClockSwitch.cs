using UnityEngine;

public class ClockSwitch : Interactable
{
    private int clockIndex;
    private ClockPuzzle puzzle;
    private SpriteRenderer face;
    private static readonly string[] ClockNames = { "I", "II", "III" };

    public void Configure(ClockPuzzle owner, int index)
    {
        puzzle = owner;
        clockIndex = index;
        face = GetComponent<SpriteRenderer>();
        prompt = "Set clock " + ClockNames[index];
    }

    public void SetSelected(bool selected)
    {
        if (face != null)
            face.color = selected
                ? new Color(0.55f, 0.85f, 0.58f)
                : new Color(0.55f, 0.65f, 0.85f);
    }

    public override void Interact(PlayerController player)
    {
        if (puzzle != null) puzzle.PressClock(clockIndex);
    }
}
