using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class ApiClientEnv : MonoBehaviour
{
    public MenuPanel menuPanel;

    public string userId;
    public string _acces_token;
    public string worldId;

    public void Start()
    {
        userId = PlayerPrefs.GetString("userId");
        _acces_token = PlayerPrefs.GetString("_acces_token");
        worldId = PlayerPrefs.GetString("worldId");
        Debug.Log(PlayerPrefs.GetString("userId"));
        Debug.Log(PlayerPrefs.GetString("_acces_token"));
        Debug.Log(PlayerPrefs.GetString("worldId"));

        GetWorldData();
    }

    public async void GetWorldData()
    {

        menuPanel.ClearItems();

        var request = new PostItemRequestDto()
        {
            WorldId = worldId
        };

        var json_data = JsonUtility.ToJson(request);

        var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/LoadAllObjects1", "POST", json_data, _acces_token);

        Debug.Log(response);

        var responseDto = JsonConvert.DeserializeObject<PostItemDto[]>(response);

        if (responseDto != null)
        {
            menuPanel.LoadObjects(responseDto);
        }

    }

    public async void RemoveWorld()
    {

        var request = new PostItemRequestDto()
        {
            WorldId = worldId
        };

        var json_data = JsonUtility.ToJson(request);

        var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/RemoveWorld", "POST", json_data, _acces_token);

        SceneManager.LoadScene(1);

    }

    private async Task<string> PerformApiCall(string url, string method, string jsonData = null, string token = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API-aanroep is successvol: " + request.downloadHandler.text);

                return request.downloadHandler.text;
            }
            else
            {
                Debug.Log("Fout bij API-aanroep: " + request.error);
                return null;
            }
        }
    }



    public async void SaveWorldData()
    {
        Debug.Log(worldId);
        foreach (var item in menuPanel.updatedItems)
        {
            Debug.Log(item.ObjectId);

            if (item.ObjectId == null || string.IsNullOrEmpty(item.ObjectId) || item.ObjectId == string.Empty || item.ObjectId.StartsWith("empty"))
            {
                Debug.Log($"ThisAd{item.ObjectId}");

                var request = new PostItemDto2()
                {
                    WorldId = worldId,
                    PrefabId = item.PrefabId,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    ScaleX = item.ScaleX,
                    ScaleY = item.ScaleY,
                    RotationZ = item.RotationZ,
                    LayerZ = item.LayerZ
                };


                var json_data = JsonUtility.ToJson(request);

                var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/AddObject", "POST", json_data, _acces_token);
                Debug.Log("res1: " + response);

                // Deserialize into the wrapper class which contains a list of PostItemDto
                var responseDto = JsonConvert.DeserializeObject<PostResponseDto>(response);

                Debug.Log(responseDto.message);
                if (responseDto.message == "Created")
                {
                    Debug.Log("Saved world");
                }
            }
            else
            {
                var request = new PostItemDto()
                {
                    ObjectId = item.ObjectId,
                    PrefabId = item.PrefabId,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    ScaleX = item.ScaleX,
                    ScaleY = item.ScaleY,
                    RotationZ = item.RotationZ,
                    LayerZ = item.LayerZ
                };

                var json_data = JsonUtility.ToJson(request);

                var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/ReplaceObject", "POST", json_data, _acces_token);

                Debug.Log("res: " + response);

                // Deserialize into the wrapper class which contains a list of PostItemDto
                var responseDto = JsonConvert.DeserializeObject<PostResponseDto>(response);

                Debug.Log(responseDto.message);
                if (responseDto.message == "Replaced")
                {
                    Debug.Log("Replaced world");
                }
            }

        }

        GetWorldData();
    }

    public async void RemoveWorldData(string? id, GameObject item)
    {
        if (id.StartsWith("empty"))
        {
            Debug.Log("removing item");
            menuPanel.RemoveItem(id, item);

        }
        else
        {
            Debug.Log("Removing object");
            var request = new PostremoveItemsDto()
            {
                ObjectId = id
            };

            Debug.Log(request.ObjectId);

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/RemoveObject", "POST", json_data, _acces_token);

            //// Deserialize into the wrapper class which contains a list of PostItemDto
            //var responseDto = JsonConvert.DeserializeObject<PostResponseDto>(response);

            //Debug.Log(responseDto.message);
            //if (responseDto.message == "Removed")
            //{
            //    Debug.Log("Removed object");
            //}
        }

        GetWorldData();

    }
}
