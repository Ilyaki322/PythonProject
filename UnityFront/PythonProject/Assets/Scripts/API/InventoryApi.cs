using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static ItemDetailsDTO;

public class InventoryApi : MonoBehaviour
{
    [SerializeField] ItemDetails[] m_items;

    private readonly string m_url = "http://localhost:5000/inventory";
    private string m_token;

    public void setToken(string token) => m_token = token;

    private void Awake()
    {
        StartCoroutine(UpdateItems());
    }

    public IEnumerator UpdateItems()
    {
        // Convert to DTO
        ItemDetailsDTO[] dtoArray = new ItemDetailsDTO[m_items.Length];
        for (int i = 0; i < m_items.Length; i++)
        {
            dtoArray[i] = new ItemDetailsDTO(m_items[i]);
        }

        ItemDetailsDTOList wrapper = new ItemDetailsDTOList(dtoArray);
        string json = JsonUtility.ToJson(wrapper);

        Debug.Log("Sending JSON:\n" + json);

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
}
