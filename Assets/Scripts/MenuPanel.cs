using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public List<GameObject> prefabs;

    public List<GameObject> items = new List<GameObject>();

    public List<PostItemDto> objectList = new List<PostItemDto>();

    public List<PostItemDto> updatedItems = new List<PostItemDto>();

    public void SetRequestData(string? objId, string prefId, float posX, float posY, float sclX, float sclY, float rotZ, int lZ)
    {

        PostItemDto existingItem = updatedItems.Find(item => item.ObjectId == objId);

        if (existingItem != null)
        {
            existingItem.PrefabId = prefId;
            existingItem.PositionX = posX;
            existingItem.PositionY = posY;
            existingItem.ScaleX = sclX;
            existingItem.ScaleY = sclY;
            existingItem.RotationZ = rotZ;
            existingItem.LayerZ = lZ;
        }
        else
        {
            PostItemDto newItem = new PostItemDto
            {
                ObjectId = objId,
                PrefabId = prefId,
                PositionX = posX,
                PositionY = posY,
                ScaleX = sclX,
                ScaleY = sclY,
                RotationZ = rotZ,
                LayerZ = lZ,
            };

            updatedItems.Add(newItem);
        }

    }

    public void RemoveItem(string id, GameObject item)
    {
        if (!string.IsNullOrEmpty(id) && id.Trim().ToLower().StartsWith("empty"))
        {

            updatedItems.RemoveAll(x => x.ObjectId == id);

            if (item != null)
            {
                GameObject.Destroy(item);
            }

            items.Remove(item);
        } else
        {
            objectList.RemoveAll(x => x.ObjectId == id);

            if (item != null)
            {
                GameObject.Destroy(item);
            }

            items.Remove(item);
        }

    }

    public void CreateGameObjectFromClick(int prefabIndex)
    {
        string newId = $"empty{Guid.NewGuid()}";
        var well = Instantiate(prefabs[prefabIndex], Vector3.zero, Quaternion.identity);
        var draggable = well.GetComponent<Draggable>();
        draggable.StartDragging();
        draggable.menuPanel = this;
        draggable.prefabIndex = prefabIndex.ToString();
        draggable.SetId(newId);
        draggable.removeButton.SetActive(false);
        items.Add(well);
    }

    public void ClearItems()
    {

        foreach (var obj in new List<GameObject>(items))
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        updatedItems.Clear();
        items.Clear();

    }

    public void LoadObjects(PostItemDto[] itemList)
    {
        foreach (var obj in items)
        {
            Destroy(obj);
        }
        items.Clear();


        foreach (var item in itemList)
        {
            var well = Instantiate(prefabs[Convert.ToInt32(item.PrefabId)], new Vector3(item.PositionX, item.PositionY, 0f), Quaternion.Euler(0, 0, item.RotationZ));

            Draggable draggable = well.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.SetId(item.ObjectId);
                draggable.menuPanel = this;
                draggable.prefabIndex = item.PrefabId;
            }

            items.Add(well);

            SetRequestData(
                item.ObjectId,
                item.PrefabId,
                item.PositionX,
                item.PositionY,
                item.ScaleX,
                item.ScaleY,
                item.RotationZ,
                item.LayerZ
            );
        }
    }


    public void GetObjects(PostItemDto[] itemList)
    {

        ClearItems();

        updatedItems.AddRange(itemList);

        foreach (var item in objectList)
        {
            var well = Instantiate(prefabs[Convert.ToInt32(item.PrefabId)], new Vector3(item.PositionX, item.PositionY, 0f), Quaternion.identity);
            Draggable draggable = well.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.SetId(item.ObjectId);
                draggable.prefabIndex = item.PrefabId;
            }
            items.Add(well);
        }
    }

    public void HideMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        this.gameObject.SetActive(true);
    }
}
