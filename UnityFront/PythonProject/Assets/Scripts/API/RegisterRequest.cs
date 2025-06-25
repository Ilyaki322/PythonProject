using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RegisterRequest
{
    public IEnumerator Register(string username, string password, string email,
    Action<String> onError, Action<String> onSuccuess)
    {
        string jsonData = JsonUtility.ToJson(new LoginData { username = username, password = password, email = email });

        UnityWebRequest request = new UnityWebRequest("http://localhost:5000/register", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccuess(request.downloadHandler.text);
        }
        else
        {
            onError(request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
        public string email;
    }
}
