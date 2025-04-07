using UnityEngine;

public class Layout : MonoBehaviour
{
    public class World
    {
        public string WorldId { get; set; }  // Represents the unique ID of the world
        public string PlayerId { get; set; } // Represents the player associated with the world
        public string WorldName { get; set; }  // Represents the name of the world
        public int Width { get; set; }  // Represents the width of the world
        public int Height { get; set; } // Represents the height of the world
    }
}

public class WorldPost
{
    public string PlayerId { get; set; } // Represents the player associated with the world
    public string WorldName { get; set; }  // Represents the name of the world
    public int Width { get; set; }  // Represents the width of the world
    public int Height { get; set; } // Represents the height of the world
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