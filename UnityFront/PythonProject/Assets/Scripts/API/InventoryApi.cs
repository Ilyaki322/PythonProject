using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using static ItemDetailsDTO;

public class InventoryApi : MonoBehaviour
{
    [SerializeField] ItemDetails[] m_items;

    private readonly string m_url = "http://localhost:5000/inventory";
    private string m_token;
    private int m_charID;

    public void setToken(string token) => m_token = token;

    public void setCharID(int id) => m_charID = id;

    private void Awake()
    {
        StartCoroutine(UpdateItems());
    }

    public IEnumerator UpdateItems()
    {
        ItemDetailsDTO[] dtoArray = new ItemDetailsDTO[m_items.Length];
        for (int i = 0; i < m_items.Length; i++)
        {
            dtoArray[i] = new ItemDetailsDTO(m_items[i]);
        }

        ItemDetailsDTOList wrapper = new ItemDetailsDTOList(dtoArray);
        string json = JsonUtility.ToJson(wrapper);

        UnityWebRequest req = new UnityWebRequest(m_url + "/update", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful.");
        }
        else
        {
            Debug.LogError("Upload failed: " + req.error);
        }
    }

    public IEnumerator GetItems(Action<string> onSuccess)
    {
        UnityWebRequest req = new UnityWebRequest(m_url + "/get", "GET");
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", "Bearer " + m_token);
        req.SetRequestHeader("CharID", m_charID.ToString());

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            onSuccess(req.downloadHandler.text);
        }
        else
        {
            Debug.LogError(req.downloadHandler.text);
        }
    }

    public ItemDetails getByID(string id)
    {
        return m_items.Where(item => item.Id == SerializableGuid.FromHexString(id)).FirstOrDefault();
    }
}
