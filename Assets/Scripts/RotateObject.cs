using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public GameObject target;              // The object to rotate
    public float rotationStep = 90f;       // Snap angle
    public float mouseThreshold = 0.5f;    // Mouse movement needed to trigger snap
    public float cooldownTime = 0.2f;      // Delay between snaps

    public bool isRotating = false;       // Is rotation active?
    private bool canRotate = true;         // Can rotate based on cooldown
    private bool ignoreClickOnce = false;  // To avoid immediate toggle-off

    void Update()
    {
        if (isRotating && target != null)
        {
            float mouseDeltaX = Input.GetAxis("Mouse X");

            if (canRotate)
            {
                if (mouseDeltaX > mouseThreshold)
                {
                    SnapRotate(1);
                }
                else if (mouseDeltaX < -mouseThreshold)
                {
                    SnapRotate(-1);
                }
            }

            if (!ignoreClickOnce && Input.GetMouseButtonDown(0))
            {
                isRotating = false;
                Debug.Log("Rotation stopped by screen click.");
            }

            // Reset the ignore flag after 1 frame
            ignoreClickOnce = false;
        }
    }

    void OnMouseUpAsButton()
    {
        isRotating = true;
        ignoreClickOnce = true; // Prevent toggle-off on same click
        Debug.Log("Rotation started.");
    }

    void SnapRotate(int direction)
    {
        canRotate = false;

        Vector3 currentRotation = target.transform.eulerAngles;
        currentRotation.z += rotationStep * direction;
        target.transform.eulerAngles = currentRotation;

        Invoke(nameof(EnableRotation), cooldownTime);
    }

    void EnableRotation()
    {
        canRotate = true;
    }
}
