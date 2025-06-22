using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System;

public class LoginRequest
{
    public IEnumerator Login(string username, string password, 
        Action<String> onError, Action<String> onSuccuess)
    {
        string jsonData = JsonUtility.ToJson(new LoginData { username = username, password = password });

        UnityWebRequest request = new UnityWebRequest("http://localhost:5000/login", "POST");
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
            onError(request.error);
        }
    }

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }
}
