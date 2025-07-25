using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System;

public class LoginRequest
{
    private const string BaseUrl = "http://localhost:5000";

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
            LoginResponse resp = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            onSuccuess(resp.token);
        }

        if (request.result == UnityWebRequest.Result.ConnectionError) onError("Could not connect to game server.");
        else
        {
            ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            onError(error.message);
        }
    }

    public string InitiateGoogleLogin()
    {
        string state = Guid.NewGuid().ToString();
        Application.OpenURL($"{BaseUrl}/login/google?state={state}");
        return state;
    }

    public IEnumerator PollForToken(string state, Func<bool> shouldCancel,
                                Action<string> onError, Action<string> onSuccess)
    {
        while (!shouldCancel())
        {
            using var req = UnityWebRequest.Get($"{BaseUrl}/login/status?state={state}");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                if (!string.IsNullOrEmpty(req.downloadHandler.text))
                {
                    var tr = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
                    onSuccess(tr.token);
                    yield break;
                }
                // else 204: keep waiting
            }
            else if (req.result == UnityWebRequest.Result.ConnectionError ||
                     req.result == UnityWebRequest.Result.ProtocolError)
            {
                onError($"{req.result}: {req.error}");
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
        onError("Login canceled.");
    }

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
        public string token;
    }

    [Serializable]
    private class TokenResponse
    {
        public string token;
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string message;
    }
}
