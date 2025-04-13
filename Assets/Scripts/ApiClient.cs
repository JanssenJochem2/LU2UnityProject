using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public TMP_InputField login_nameInput;
    public TMP_InputField login_passwordInput;
    public TMP_Text loginResponse;

    public TMP_InputField register_nameInput;
    public TMP_InputField register_passwordInput;
    public TMP_Text registerResponse;

    public WorldManager worldManager;

    public static ApiClient apiclient { get; private set; }
    public static string _acces_token = null;

    public string worldId;

    void Awake()
    {
        apiclient = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public string GetToken()
    {
        return _acces_token;
    }

    public void SetWorldId(string id)
    {
        worldId = id;
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

            var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/auth/register", "POST", json_data);

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
        catch (Exception ex)
        {
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

            var request = new PostLoginRequestDto()
            {
                email = username,
                password = password
            };

            var json_data = JsonUtility.ToJson(request);

            var response = await PerformApiCall("https://avansict2211560lu2project.azurewebsites.net/auth/login", "POST", json_data);

            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);

            _acces_token = responseDto.accessToken;

            Debug.Log(_acces_token);
            worldManager.SetAccessToken();
        }
        catch (Exception ex)
        {
            loginResponse.text = $"Gebruikersnaam en wachtwoord zijn ongeldig";
            loginResponse.color = Color.red;
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
