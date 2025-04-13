using UnityEditor;
using UnityEngine;

public class Draggable : MonoBehaviour 
{
    public bool isDragging = false;

    public MenuPanel menuPanel;

    private ApiClientEnv apiClient;

    public string prefabIndex = "0";

    public Transform trans;

    public string? objectId = null;

    public bool showRemove = false;

    public GameObject removeButton;
    public GameObject moveButton;
    public GameObject rotateButton;

    public bool isSelected;

    public Collider2D col;

    public ScaleAnimationHandler animationHandler;

    private void Start()
    {
        apiClient = FindAnyObjectByType<ApiClientEnv>();
        animationHandler.Shake();
    }

    void Update()
    {
        if (isDragging)
        {
            trans.position = GetMousePosition();
        }

        if (isSelected)
        {
            removeButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(true);
        } else
        {
            removeButton.SetActive(false);
            moveButton.SetActive(false);
            rotateButton.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                foreach (var item in menuPanel.items)
                {
                    Draggable d = item.GetComponent<Draggable>();
                    if (d != null)
                    {
                        d.isSelected = false;
                    }
                }

                isSelected = true;
                animationHandler.OnSelect();
            }
        }

    }

    private Vector3 GetMousePosition()
    {
        int worldWidth = PlayerPrefs.GetInt("worldWidth");
        int worldHeight = PlayerPrefs.GetInt("worldHeight");

        float gridSize = 0.82f;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float snappedX = Mathf.Round(mousePosition.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(mousePosition.y / gridSize) * gridSize;

        bool isOutsideBounds = snappedX < -worldWidth / 2f || snappedX > worldWidth / 2f || snappedY < -worldHeight / 2f || snappedY > worldHeight / 2f;

        AdjustTransparency(isOutsideBounds);

        snappedX = Mathf.Clamp(snappedX, -worldWidth / 2f, worldWidth / 2f);
        snappedY = Mathf.Clamp(snappedY, -worldHeight / 2f, worldHeight / 2f);

        Vector3 targetPosition = new Vector3(snappedX, snappedY, 0);

        return targetPosition;
    }

    private void AdjustTransparency(bool isOutsideBounds)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();

        if (renderer != null)
        {
            Color currentColor = renderer.material.color;

            if (isOutsideBounds)
            {
                renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
            }
            else
            {
                renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            }
        }
    }


    public void SetId(string? id) { 
        objectId = id;
    }

    public void StartDragging()
    {
        isDragging = true;
        removeButton.SetActive(false);
        if (menuPanel != null)
        {
            menuPanel.HideMenu();
        }
    }

    public void RemoveObject()
    {
        apiClient.RemoveWorldData(objectId, this.gameObject);
    }

    public void MoveObject()
    {
        if (isSelected)
        {
            StartDragging();
        }
    }

    void OnMouseUp()
    {
        removeButton.SetActive(true);
        Debug.Log(isSelected);
        float currentZ = gameObject.transform.localRotation.eulerAngles.z;
        if (isSelected || isDragging)
        {
            Debug.Log(currentZ);
            isDragging = false;
            menuPanel.SetRequestData(objectId, prefabIndex, trans.position.x, trans.position.y, 1f, 1f, currentZ, 1);
            if (menuPanel != null)
            {
                menuPanel.ShowMenu();
            }
        }
    }
}