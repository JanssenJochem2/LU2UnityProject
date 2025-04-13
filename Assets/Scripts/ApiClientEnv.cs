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
    public string worldId;

    private string _acces_token = ApiClient._acces_token;

    public void Start()
    {
        worldId = PlayerPrefs.GetString("worldId");
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

        var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/LoadAllObjects", "POST", json_data, _acces_token);

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

        SceneManager.LoadScene(0);

    }

    public async void SaveWorldData()
    {
        foreach (var item in menuPanel.updatedItems)
        {

            if (item.ObjectId == null || string.IsNullOrEmpty(item.ObjectId) || item.ObjectId == string.Empty || item.ObjectId.StartsWith("empty"))
            {

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

                var responseDto = JsonConvert.DeserializeObject<PostResponseDto>(response);

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

            }

        }

        GetWorldData();
    }

    public async void RemoveWorldData(string? id, GameObject item)
    {
        if (!string.IsNullOrEmpty(id) && id.Trim().ToLower().StartsWith("empty"))
        {
            menuPanel.RemoveItem(id, item);

        }
        else
        {
            var request = new PostremoveItemsDto()
            {
                ObjectId = id
            };

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/RemoveObject", "POST", json_data, _acces_token);

            menuPanel.RemoveItem(id, item);

        }
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
}
