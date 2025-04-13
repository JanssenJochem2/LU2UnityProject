using UnityEditor;
using UnityEngine;

public class Draggable : MonoBehaviour // IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // The prefab to instantiate
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


    private void Start()
    {
        apiClient = FindAnyObjectByType<ApiClientEnv>();
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

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // Deselect all others using menuPanel.items
                foreach (var item in menuPanel.items)
                {
                    Draggable d = item.GetComponent<Draggable>();
                    if (d != null)
                    {
                        d.isSelected = false;
                    }
                }

                // Select this one
                isSelected = true;
                Debug.Log($"Selected {gameObject.name}");
            }
        }

    }

    private Vector3 GetMousePosition()
    {
        float gridSize = 0.82f;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float snappedX = Mathf.Round(mousePosition.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(mousePosition.y / gridSize) * gridSize;

        Vector3 targetPosition = new Vector3(snappedX, snappedY, 0);

        return targetPosition;
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
        Debug.Log("Removing: " + objectId);
    }

    public void MoveObject()
    {
        if (isSelected)
        {
            StartDragging();
        }
    }

    //void OnMouseDown()
    //{
    //    if (gameObject == Selection.activeGameObject)
    //    {
    //        isSelected = !isSelected;
    //    }
    //}

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