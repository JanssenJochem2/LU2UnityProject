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

    }

    private Vector3 GetMousePosition()
    {
        float gridSize = 0.64f;
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
        StartDragging();
    }

    void OnMouseUp()
    {
        removeButton.SetActive(true);
        Debug.Log(objectId);
        if (isDragging)
        {
            isDragging = !isDragging;
            menuPanel.SetRequestData(objectId, prefabIndex, trans.position.x, trans.position.y, 1f, 1f, 0f, 1);
            if (menuPanel != null)
            {
                menuPanel.ShowMenu();
            }
        }
    }
}