using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer character;
    private Transform visual;
    private float artworkScale;
    private float stepPhase;
    private readonly Vector3 footPosition = new Vector3(0f, -0.75f, 0f);

    public void Configure()
    {
        body = GetComponent<Rigidbody2D>();
        Texture2D texture = Resources.Load<Texture2D>("Player/avatar");
        visual = new GameObject("Mini Character").transform;
        visual.SetParent(transform, false);
        visual.localPosition = footPosition;
        character = visual.gameObject.AddComponent<SpriteRenderer>();
        character.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.05f), 100f, 0, SpriteMeshType.FullRect);
        character.sortingOrder = 10;
        artworkScale = 1.65f * 100f / texture.height;
        visual.localScale = Vector3.one * artworkScale;
    }

    private void LateUpdate()
    {
        if (character == null || body == null) return;
        Vector2 velocity = body.linearVelocity;
        if (Mathf.Abs(velocity.x) > 0.05f) character.flipX = velocity.x < 0f;

        bool walking = velocity.sqrMagnitude > 0.01f;
        if (walking) stepPhase += Time.deltaTime * 12f;
        else stepPhase = 0f;

        float bounce = walking ? Mathf.Abs(Mathf.Sin(stepPhase)) * 0.045f : 0f;
        float breathing = walking ? 1f : 1f + Mathf.Sin(Time.time * 2f) * 0.012f;
        visual.localPosition = footPosition + Vector3.up * bounce;
        visual.localRotation = Quaternion.Euler(0f, 0f, walking ? Mathf.Sin(stepPhase) * 1.5f : 0f);
        visual.localScale = new Vector3(artworkScale, artworkScale * breathing, artworkScale);
    }
}
