using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    [SerializeField] private float attractionCurveCoeff = 0.5f;
    [SerializeField] private float maxAttractionStrength = 5f;
    [SerializeField] private float attractionLerpSpeed = 5f;
    [SerializeField] private float initialRandomVelocityStrength = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private ItemType type;
    private Transform playerTransform;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerTransform = FindAnyObjectByType<Player>()?.transform;
        type = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);
        rb.linearVelocity = Random.insideUnitCircle * initialRandomVelocityStrength;
        spriteRenderer.sprite = ResourceManager.instance.resourceSprites.GetSpriteForType(type);
    }

    void FixedUpdate()
    {
        Vector2 destination = playerTransform != null ? (Vector2)playerTransform.position : transform.position;
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, destination);
        float attractionStrength = GetAttractionStrengthMult(distance) * maxAttractionStrength;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * attractionStrength, attractionLerpSpeed * Time.fixedDeltaTime);
        if (distance < 0.5f)
        {
            ResourceManager.instance.IncrementResource(type);
            Destroy(gameObject);
        }
    }

    private float GetAttractionStrengthMult(float distance)
    {
        return (Mathf.PI / 2 - Mathf.Atan(distance * attractionCurveCoeff)) / (Mathf.PI / 2);
    }

    public enum ItemType
    {
        Wood,
        Stone,
        Gold
    }
}
