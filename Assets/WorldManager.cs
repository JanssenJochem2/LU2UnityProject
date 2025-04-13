using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static Layout;

public class WorldManager : MonoBehaviour
{
    public TMP_InputField worldName_input;
    public TMP_InputField worldHeight_input;
    public TMP_InputField worldWidth_input;
    public TMP_Text addWorldResponse;

    public GameObject canvas;
    public GameObject worldList;
    public GameObject buttonPrefab;
    public GameObject loadWorldsButton;

    public GameObject authPanel;
    public GameObject worldPanel;

    private List<World> worlds = new List<World>();
    private List<GameObject> loadedWorlds = new List<GameObject>();

    public ApiClient apiClient;

    private string _acces_token = ApiClient._acces_token;
    public string worldId;

    public void Start()
    {
        SetAccessToken();
        if (!string.IsNullOrEmpty(_acces_token))
        {
            authPanel.SetActive(false);
            worldPanel.SetActive(true);
        }
        else
        {
            worldPanel.SetActive(false);
            authPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(_acces_token))
        {
            authPanel.SetActive(true);
            worldPanel.SetActive(false);
        }
        else
        {
            worldPanel.SetActive(true);
            authPanel.SetActive(false);
        }

        if (worlds.Count != 0)
        {
            worldList.SetActive(true);
            loadWorldsButton.SetActive(false);
        }
        else
        {
            worldList.SetActive(false);
            loadWorldsButton.SetActive(true);
        }
    }

    public void SetAccessToken()
    {
        _acces_token = ApiClient._acces_token;
    }

    public async void GetWorlds()
    {
        worlds.Clear();

        if (worlds == null)
        {
            worlds = new List<World>();
        }

        var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/GetWorlds", "POST", null, _acces_token);

        var responseDto = JsonConvert.DeserializeObject<World[]>(response);

        if (responseDto != null)
        {
            worlds.AddRange(responseDto);
        }

        CreateButtons();
    }

    void CreateButtons()
    {
        foreach (GameObject btn in loadedWorlds)
        {
            Destroy(btn);
        }
        loadedWorlds.Clear();


        for (int i = 0; i < worlds.Count(); i++)
        {
            World thisWorld = worlds[i];
            GameObject buttonObject = Instantiate(buttonPrefab, canvas.transform);

            buttonObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100 - i * 100);

            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {

                buttonText.text = "Wereld - " + thisWorld.WorldName + thisWorld.Width + thisWorld.Height;
            }

            UnityEngine.UI.Button button = buttonObject.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => OnWorldClick(thisWorld));

            loadedWorlds.Add(buttonObject);
        }
    }

    void OnWorldClick(World world)
    {
        PlayerPrefs.SetInt("worldWidth", world.Width);
        PlayerPrefs.SetInt("worldHeight", world.Height);
        PlayerPrefs.SetString("worldId", world.WorldId.ToString());
        SceneManager.LoadScene(1);
    }

    public async void AddWorlds()
    {

        try
        {
            var request = new WorldPost()
            {
                WorldName = worldName_input.text.ToString(),
                Width = Convert.ToInt32(worldWidth_input.text),
                Height = Convert.ToInt32(worldHeight_input.text)
            };

            var json_data = JsonConvert.SerializeObject(request);

            var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/Object/AddWorlds", "POST", json_data, _acces_token);

            var responseDto = JsonConvert.DeserializeObject<World>(response);

            GetWorlds();

        }
        catch (Exception ex)
        {
            addWorldResponse.text = "Error bij het toevoegen van wereld, probeer het opnieuw";
            addWorldResponse.color = Color.red;
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
