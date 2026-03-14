using UnityEngine;

public class PlayerParralax : MonoBehaviour
{
    [Header("Targets")]
    public Transform player1;
    public Transform player2;

    [Header("Settings")]
    [Tooltip("Negative values = Far Background. Positive values = Foreground.")]
    public float moveModifier;

    private Vector3 startPosition;
    private Vector3 startMidpointPosition;

    void Start()
    {
        startPosition = transform.position;

        if (player1 != null && player2 != null)
        {
            // Calculate starting midpoint
            startMidpointPosition = (player1.position + player2.position) / 2f;
        }
    }

    void Update()
    {
        if (player1 == null || player2 == null) return;

        // 1. Find the current midpoint between both players
        Vector3 currentMidpoint = (player1.position + player2.position) / 2f;

        // 2. Calculate how far the midpoint has moved
        float distX = currentMidpoint.x - startMidpointPosition.x;
        float distY = currentMidpoint.y - startMidpointPosition.y;

        // 3. Apply parallax
        float moveX = distX * moveModifier;
        float moveY = distY * moveModifier;

        transform.position = new Vector3(
            startPosition.x + moveX,
            startPosition.y + moveY,
            startPosition.z
        );
    }
}