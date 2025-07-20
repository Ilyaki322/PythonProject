using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterApi : MonoBehaviour
{

    private string m_url = "http://localhost:5000/characters";
    private string m_token;

    public void setToken(string token) => m_token = token;
    public void Logout() => m_token = string.Empty;
    public IEnumerator AddCharacter(CharacterDTO character, Action<bool> onSuccess)
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
            onSuccess(true);
        }
        else
        {
            onSuccess(false);
            Debug.LogError("Error adding character: " + request.error);
        }
    }

    public IEnumerator DeleteCharacter(int characterId, Action<bool> onSuccess)
    {
        UnityWebRequest request = UnityWebRequest.Delete(m_url + "/delete_character");
        request.SetRequestHeader("Authorization", "Bearer " + m_token);

        string jsonData = JsonUtility.ToJson(new CharacterID { character_id = characterId });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Character deleted successfully.");
            onSuccess(true);
        }
        else
        {
            Debug.LogError("Error deleting character: " + request.error);
            onSuccess(false);
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
