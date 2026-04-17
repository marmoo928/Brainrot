using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float forceMultiplier = 3f;
    public float maxForce = 20f;
    public float minSwipeDistance = 50f; // v pixeloch, aby sa ignorol náhodný klik

    private Rigidbody2D rb;
    private Vector2 swipeStart;
    private bool isSwiping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
            Vector2 swipeDelta = swipeEnd - swipeStart; // v pixeloch

            if (swipeDelta.magnitude < minSwipeDistance) return; // príliš krátky swipe

            // Normalizuj a škáluj silu
            Vector2 direction = swipeDelta.normalized;
            float strength = Mathf.Min(swipeDelta.magnitude * forceMultiplier / 100f, maxForce);
            Vector2 force = direction * strength;

            rb.linearVelocity = Vector2.zero; // reset predchádzajúcej rýchlosti
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}