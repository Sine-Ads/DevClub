using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Collections;


public class Shaman_Movement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float walkAnimSpeed = 10f;
    public float limbRotateAmount = 45f;

    [Header("Slide Settings")]
    public float slideSpeed = 10f;
    public float slideDistance = 5f;
    public float slideCooldown = 1.0f;

    [Header("Crouch Settings")]
    [Tooltip("How tall the hitbox should be when crouching (Standard is usually 2, try 1 or 1.2)")]
    public float crouchHeight = 1.2f;
    public float crouchWidth = 0.9f;

    [Header("Jump Physics")]
    public float jumpHeight = 3f;
    public float timeToJumpApex = 0.4f;
    public float shortJumpGravityMultiplier = 3f;

    [Header("Body Parts")]
    public GameObject torso;
    public Transform rightLeg;
    public Transform leftLeg;
    public Transform rightHand;
    public Transform leftHand;

    [Header("Visuals")]
    public GameObject slideSpriteObject;
    public GameObject crouchSpriteObject;

    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    private bool isGrounded;
    private bool facingRight = false; // Changed to false since character starts facing left

    // State Variables
    private bool isSliding = false;
    private bool isCrouching = false;
    private bool canSlide = true;
    private float currentSlideDirection;

    // Physics calculation variables
    private float calculatedJumpForce;
    private float standardGravityScale;

    // Original Collider Values
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        // Store original collider settings automatically
        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset;
        }

        if (torso != null) torso.SetActive(true);
        if (slideSpriteObject != null) slideSpriteObject.SetActive(false);
        if (crouchSpriteObject != null) crouchSpriteObject.SetActive(false);

        float gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        calculatedJumpForce = Mathf.Abs(gravity) * timeToJumpApex;
        standardGravityScale = gravity / Physics2D.gravity.y;
        rb.gravityScale = standardGravityScale;
    }

    void Update()
    {
        // sliding logic (Priority over crouch)
        if (isSliding)
        {
            SlideMovement();
            return;
        }

        // Get horizontal input from J (left) and L (right)
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.J))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.L))
            moveInput = 1f;

        // crouching logic
        bool holdingDown = Input.GetKey(KeyCode.K);

        // Only allow crouching if we are grounded AND not trying to walk
        if (holdingDown && isGrounded && moveInput == 0)
        {
            if (!isCrouching) StartCrouch();

            // Stop X movement strictly while crouching
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        else
        {
            // If we release K or start walking, stand up
            if (isCrouching) StopCrouch();
        }

        // slide input
        if (Input.GetKeyDown(KeyCode.K) && isGrounded && moveInput != 0 && canSlide)
        {
            StartCoroutine(PerformSlide(moveInput));
        }

        // normal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // jumping
        if (Input.GetKeyDown(KeyCode.I) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, calculatedJumpForce);
        }

        // gravity (short jump)
        if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.I))
            rb.gravityScale = standardGravityScale * shortJumpGravityMultiplier;
        else
            rb.gravityScale = standardGravityScale;

        // mirroring logic
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        AnimateLimbs(moveInput);
    }

    // Crouch function seperate from the slide function
    void StartCrouch()
    {
        isCrouching = true;

        // Visuals
        torso.SetActive(false);
        if (crouchSpriteObject != null) crouchSpriteObject.SetActive(true);

        // Physics: Shrink Hitbox AND Adjust Offset to keep feet planted
        if (playerCollider != null)
        {
            float heightDifference = originalColliderSize.y - crouchHeight;

            // Logic: Move the center DOWN by half the amount we shrunk
            // This ensures the bottom edge stays exactly where it was
            float newOffsetY = originalColliderOffset.y - (heightDifference / 2);

            playerCollider.size = new Vector2(crouchWidth, crouchHeight);
            playerCollider.offset = new Vector2(originalColliderOffset.x, newOffsetY);
        }
    }

    void StopCrouch()
    {
        isCrouching = false;

        // Visuals
        if (crouchSpriteObject != null) crouchSpriteObject.SetActive(false);
        torso.SetActive(true);

        // Physics: Restore Hitbox
        if (playerCollider != null)
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }
    }

    // This is for slide

    IEnumerator PerformSlide(float direction)
    {
        isSliding = true;
        canSlide = false;
        currentSlideDirection = (direction > 0) ? 1 : -1;

        torso.SetActive(false);
        slideSpriteObject.SetActive(true);

        // Use Crouch Size for Sliding too (optional, feels tighter)
        if (playerCollider != null)
        {
            float heightDifference = originalColliderSize.y - crouchHeight;
            float newOffsetY = originalColliderOffset.y - (heightDifference / 2);
            playerCollider.size = new Vector2(crouchWidth, crouchHeight);
            playerCollider.offset = new Vector2(originalColliderOffset.x, newOffsetY);
        }

        float calculatedDuration = slideDistance / slideSpeed;
        yield return new WaitForSeconds(calculatedDuration);

        slideSpriteObject.SetActive(false);
        torso.SetActive(true);

        // Restore Hitbox
        if (playerCollider != null)
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }

        isSliding = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    void SlideMovement()
    {
        rb.linearVelocity = new Vector2(currentSlideDirection * slideSpeed, rb.linearVelocity.y);
    }

    // animating limbs - FIXED for left-facing character
    void AnimateLimbs(float movement)
    {
        if (movement != 0)
        {
            float angle = Mathf.Sin(Time.time * walkAnimSpeed) * limbRotateAmount;
            rightLeg.localRotation = Quaternion.Euler(0, 0, -angle);
            leftLeg.localRotation = Quaternion.Euler(0, 0, angle);
            rightHand.localRotation = Quaternion.Euler(0, 0, angle);
            leftHand.localRotation = Quaternion.Euler(0, 0, -angle);
        }
        else
        {
            rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, Quaternion.identity, Time.deltaTime * 10);
            leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, Quaternion.identity, Time.deltaTime * 10);
            rightHand.localRotation = Quaternion.Lerp(rightHand.localRotation, Quaternion.identity, Time.deltaTime * 10);
            leftHand.localRotation = Quaternion.Lerp(leftHand.localRotation, Quaternion.identity, Time.deltaTime * 10);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}
