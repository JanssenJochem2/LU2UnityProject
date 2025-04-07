using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer; 

    public Sprite walkRightSprite;
    public Sprite walkLeftSprite; 
    public Sprite walkBackSprite; 
    public Sprite idleSprite;      
    public Sprite forwardSprite;
    public Transform transform;

    public Vector2 movement;

    private string lastDirection = ""; // Variable to store the last direction pressed

    public float minX = -150f, maxX = 150f;
    public float minY = -125f, maxY = 125f;


    public void Start()
    {
        transform.position = Vector3.zero;
    }


    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Update the last direction based on key presses
        if (Input.GetKeyDown(KeyCode.RightArrow)) lastDirection = "right";
        if (Input.GetKeyDown(KeyCode.LeftArrow)) lastDirection = "left";
        if (Input.GetKeyDown(KeyCode.UpArrow)) lastDirection = "up";
        if (Input.GetKeyDown(KeyCode.DownArrow)) lastDirection = "down";

        // If there's movement, update sprite based on last direction pressed
        if (movement.x > 0 || movement.x < 0 || movement.y != 0)
        {
            switch (lastDirection)
            {
                case "right": // Moving to the right
                    spriteRenderer.sprite = walkRightSprite;
                    break;

                case "left": // Moving to the left
                    spriteRenderer.sprite = walkLeftSprite;
                    break;

                case "up": // Moving up (backward)
                    spriteRenderer.sprite = walkBackSprite;
                    break;

                case "down": // Moving down (forward)
                    spriteRenderer.sprite = forwardSprite;
                    break;

                default:
                    spriteRenderer.sprite = idleSprite;
                    break;
            }
        }
        else
        {
            // If no movement is detected, set to idle sprite
            spriteRenderer.sprite = idleSprite;
        }
    }

    void FixedUpdate()
    {
        // Calculate the new position based on movement
        Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0) * moveSpeed * Time.fixedDeltaTime;

        // Restrict movement within the bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX); // Restrict X axis
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY); // Restrict Y axis

        // Apply the new position
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.2f);

    }
}
