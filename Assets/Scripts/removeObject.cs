using UnityEngine;

public class removeObject : MonoBehaviour
{
    public Draggable draggable;
    void OnMouseDown()
    {
        draggable.RemoveObject();
    }
}
