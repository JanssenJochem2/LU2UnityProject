using UnityEngine;

public class toggleDrag : MonoBehaviour
{
    public Draggable draggable;

    private void OnMouseUpAsButton()
    {
        draggable.StartDragging();

    }
}
