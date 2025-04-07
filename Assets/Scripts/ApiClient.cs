using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using static Layout;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class ApiClient : MonoBehaviour
{
    public TMP_InputField login_nameInput;
    public TMP_InputField login_passwordInput;
    public TMP_Text loginResponse;

    public TMP_InputField register_nameInput;
    public TMP_InputField register_passwordInput;
    public TMP_Text registerResponse;

    public TMP_InputField worldName_input;
    public TMP_InputField worldHeight_input;
    public TMP_InputField worldWidth_input;
    public TMP_Text addWorldResponse;

    public GameObject authPanel;
    public GameObject worldPanel;

    public GameObject canvas;
    public GameObject worldList;
    public GameObject buttonPrefab;
    public GameObject loadWorldsButton;

    public string _acces_token;

    private string userId;

    public string worldId;

    //public World[] worlds;
    private List<World> worlds = new List<World>();

    public void Start()
    {
        string userName = PlayerPrefs.GetString("userName");
        _acces_token = PlayerPrefs.GetString("_acces_token");
        if (userName != null)
        {
            GetUser(userName);
        }
       
    }


    private void Update()
    {
        if (string.IsNullOrEmpty(userId))
        {
            authPanel.SetActive(true);
            worldPanel.SetActive(false);
        } else
        {
            worldPanel.SetActive(true);
            authPanel.SetActive(false);
        }

        if (worlds.Count != 0)
        {
            worldList.SetActive(true);
            loadWorldsButton.SetActive(false);
        } else
        {
            worldList.SetActive(false);
            loadWorldsButton.SetActive(true);
        }
    }

    public void SetWorldId(string id)
    {
        worldId = id;
    }

    public void SetUserId(string id)
    {
        userId = id;
    }

    public bool checkInputValid(string email, string password)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        return Regex.IsMatch(email, emailPattern);
    }


    public async void Register()
    {

        try
        {
            string username = register_nameInput.text;
            string password = register_passwordInput.text;

            var request = new PostRegisterRequestDto()
            {
                email = username,
                password = password
            };

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://localhost:7239/auth/register", "POST", json_data);

            Debug.Log(response);

            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);

            if (checkInputValid(username, password))
            {
                registerResponse.text = "Account is aangemaakt, u kunt inloggen!";
                registerResponse.color = Color.blue;
            }
            else
            {
                registerResponse.text = "Ongeldige waarde: Er bestaat nog geen gebruiker met deze gebruikersnaam.  De gebruikersnaam is uniek.oWachtwoord moet minimaal 10 karakters lang zijn.oWachtwoord moet minstens 1 lowercase, uppercase, cijfer en niet-alphanumeriek karakter bevatten";
                registerResponse.color = Color.red;

            }
        }
        catch (Exception ex) {

            registerResponse.text = $"Fout bij het aanmaken, probeer het opnieuw";
            registerResponse.color = Color.red;
        }
    }

    public async void Login()
    {
        try
        {
            string username = login_nameInput.text;
            string password = login_passwordInput.text;

            Debug.Log(username + " / " + password);

            var request = new PostLoginRequestDto()
            {
                email = username,
                password = password
            };

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://localhost:7239/auth/login", "POST", json_data);

            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);

            Debug.Log(responseDto.accessToken);

            _acces_token = responseDto.accessToken;

            PlayerPrefs.SetString("_acces_token", responseDto.accessToken.ToString());
            PlayerPrefs.SetString("userName", username.ToString());

            GetUser(username);
        }
        catch (Exception ex) {
            loginResponse.text = $"Gebruikersnaam en wachtwoord zijn ongeldig";
            loginResponse.color = Color.red;
        }
    }

    public async void GetUser(string useranme)
    {

        try
        {
            var request = new PostUserRequestDto()
            {
                Username = useranme,
            };

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://localhost:7239/Object/GetUser", "POST", json_data, _acces_token);

            SetUserId(response.ToString());
            PlayerPrefs.SetString("UserId", response.ToString());
        }
        catch (Exception ex) { 
            Debug.Log(ex); 
        }
    }

    public async void GetWorlds()
    {
        worlds.Clear();

        if (worlds == null)
        {
            worlds = new List<World>();
        }

        var request = new PostGetWorldRequestDto()
        {
            playerId = userId,
        };

        Debug.Log(userId);

        var json_data = JsonUtility.ToJson(request);

        var response = await PerformApiCall("https://localhost:7239/Object/GetWorlds", "POST", json_data, _acces_token);

        var responseDto = JsonConvert.DeserializeObject<World[]>(response);

        if (responseDto != null)
        {
            worlds.AddRange(responseDto);
        }

        Debug.Log(worlds.Count());

        CreateButtons();

    }

    void CreateButtons()
    {
        for (int i = 0; i < worlds.Count(); i++)
        {
            World thisWorld = worlds[i];
            GameObject buttonObject = Instantiate(buttonPrefab, canvas.transform);

            buttonObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100 - i * 100);

            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "Wereld - " + thisWorld.WorldName;
            }

            UnityEngine.UI.Button button = buttonObject.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => OnWorldClick(thisWorld.WorldId));
        }
    }

    void OnWorldClick(string worldId)
    {
        Debug.Log("Button " + worldId + " clicked!");
        PlayerPrefs.SetString("worldId", worldId.ToString());
        SceneManager.LoadScene(2);
    }

    public async void AddWorlds()
    {

        try
        {
            var request = new WorldPost()
            {
                PlayerId = userId,
                WorldName = worldName_input.text.ToString(),
                Width = Convert.ToInt32(worldWidth_input.text),
                Height = Convert.ToInt32(worldHeight_input.text)
            };

            var json_data = JsonConvert.SerializeObject(request);

            var response = await PerformApiCall("https://localhost:7239/Object/AddWorlds", "POST", json_data, _acces_token);
            Debug.Log(response);

            var responseDto = JsonConvert.DeserializeObject<World>(response);


            if (responseDto == null)
            {
                Debug.Log(responseDto);
            }

            GetWorlds();


        } catch (Exception ex)
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
