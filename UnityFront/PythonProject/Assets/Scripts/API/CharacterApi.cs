using System;
using System.Collections;
using System.Text;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterApi : MonoBehaviour
{

    private string m_url = "http://localhost:5000/characters";
    private string m_token;

    public void setToken(string token) => m_token = token;

    public IEnumerator AddCharacter(CharacterDTO character)
    {
        string json = JsonUtility.ToJson(character);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(m_url + "/add", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + m_token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Character added: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error adding character: " + request.error);
        }
    }

    public IEnumerator GetCharacters(Action<string> onSuccess)
    {
        UnityWebRequest request = UnityWebRequest.Get(m_url + "/");
        request.SetRequestHeader("Authorization", "Bearer " + m_token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error getting characters: " + request.error);
        }
    }
}
