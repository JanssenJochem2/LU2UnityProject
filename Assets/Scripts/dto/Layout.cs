using UnityEngine;

public class Layout : MonoBehaviour
{
    public class World
    {
        public string WorldId { get; set; }
        public string PlayerId { get; set; }
        public string WorldName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

public class WorldPost
{
    public string PlayerId { get; set; }
    public string WorldName { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class PostItemRequestDto
{
    public string WorldId;
}

[System.Serializable]
public class PostItemDto
{
    public string? ObjectId;
    public string WorldId;
    public string PrefabId;
    public float PositionX;
    public float PositionY;
    public float ScaleX;
    public float ScaleY;
    public float RotationZ;
    public int LayerZ;
}

[System.Serializable]
public class PostItemDto2
{
    public string WorldId;
    public string PrefabId;
    public float PositionX;
    public float PositionY;
    public float ScaleX;
    public float ScaleY;
    public float RotationZ;
    public int LayerZ;
}

[System.Serializable]
public class PostResponseDto
{
    public string message;
}


public class PostremoveItemsDto
{
    public string ObjectId;
}