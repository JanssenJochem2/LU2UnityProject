using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public GameObject target;
    public float rotationStep = 90f;
    public float mouseThreshold = 0.5f;
    public float cooldownTime = 0.2f;

    public bool isRotating = false;
    private bool canRotate = true;
    private bool ignoreClickOnce = false; 

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
            }
            ignoreClickOnce = false;
        }
    }

    void OnMouseUpAsButton()
    {
        isRotating = true;
        ignoreClickOnce = true;
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
