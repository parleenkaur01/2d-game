using UnityEngine;

public class ClockSwitch : Interactable
{
    private ClockPuzzle puzzle;
    private SpriteRenderer face;
    private Transform hand;
    private bool running;

    public void Configure(ClockPuzzle owner, int index)
    {
        puzzle = owner;
        face = GetComponent<SpriteRenderer>();
        prompt = "Open the letter puzzle";
    }

    public void SetSelected(bool selected)
    {
        running = selected;
        if (face != null)
            face.color = selected
                ? new Color(0.55f, 0.85f, 0.58f)
                : new Color(0.55f, 0.65f, 0.85f);
    }

    public void RegisterHand(Transform pivot) => hand = pivot;

    private void Update()
    {
        if (running && hand != null) hand.Rotate(0f, 0f, -75f * Time.deltaTime);
    }

    public override void Interact(PlayerController player)
    {
        if (puzzle != null) puzzle.Open(player);
    }
}
