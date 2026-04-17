using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float forceMultiplier = -2f;
    public float maxForce = 20f;
    public float minSwipeDistance = 50f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the sprite rotates to align with gravity. Set to 0 for instant snap.")]
    public float rotationSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 swipeStart;
    private bool isSwiping = false;

    public int health = 100;

    public PowerUp currentItem;
    public float itemDuration = 5f;
    public bool isInvincible = false;

    private float damageCooldown = 0.5f;
    private float lastDamageTime = -999f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = true;

        // Grab the EnvironmentBehaviour from the parent and subscribe
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.AddListener(OnGravityChanged);
        else
            Debug.LogWarning("[PlayerController] No EnvironmentBehaviour found in parent.");
    }

    void OnDestroy()
    {
        // Clean up the listener to avoid stale references
        EnvironmentBehaviour env = GetComponentInParent<EnvironmentBehaviour>();
        if (env != null)
            env.onGravityChanged.RemoveListener(OnGravityChanged);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipeDelta = swipeEnd - swipeStart;

            if (swipeDelta.magnitude < minSwipeDistance) return;

            Vector2 direction = swipeDelta.normalized;
            float strength = Mathf.Min(swipeDelta.magnitude * forceMultiplier / 100f, maxForce);
            Vector2 force = direction * strength;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    private void OnGravityChanged(Vector2 newGravityDirection)
    {
        float targetAngle = Vector2.SignedAngle(Vector2.down, newGravityDirection);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        if (rotationSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(RotateToRoutine(targetRotation));
        }
    }

    private System.Collections.IEnumerator RotateToRoutine(Quaternion target)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        transform.rotation = target;
    }


    public void Heal(int amount)
    {
        health += amount;
    }

    public void AddScore(int amount)
    {
        Debug.Log("Score +" + amount);
    }

    public void ApplyPowerUp(PowerUp powerUp)
    {
        currentItem = powerUp;

        StopAllCoroutines();
        StartCoroutine(PowerUpRoutine());
    }

    private System.Collections.IEnumerator PowerUpRoutine()
    {
        yield return new WaitForSeconds(itemDuration);
        currentItem = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();

        if (collectible != null)
        {
            collectible.OnCollect(this);
        }
    }

    public void TakeDamage(int amount)
    {
       if (isInvincible) return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        health -= amount;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died");

        // Disable movement
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Optional: play animation, reload scene, etc.
        gameObject.SetActive(false);
    }

}