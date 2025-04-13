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

    private string lastDirection = "";

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

        if (Input.GetKeyDown(KeyCode.RightArrow)) lastDirection = "right";
        if (Input.GetKeyDown(KeyCode.LeftArrow)) lastDirection = "left";
        if (Input.GetKeyDown(KeyCode.UpArrow)) lastDirection = "up";
        if (Input.GetKeyDown(KeyCode.DownArrow)) lastDirection = "down";

        if (movement.x > 0 || movement.x < 0 || movement.y != 0)
        {
            switch (lastDirection)
            {
                case "right": 
                    spriteRenderer.sprite = walkRightSprite;
                    break;

                case "left": 
                    spriteRenderer.sprite = walkLeftSprite;
                    break;

                case "up":
                    spriteRenderer.sprite = walkBackSprite;
                    break;

                case "down":
                    spriteRenderer.sprite = forwardSprite;
                    break;

                default:
                    spriteRenderer.sprite = idleSprite;
                    break;
            }
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0) * moveSpeed * Time.fixedDeltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, newPosition, 0.2f);

    }
}
