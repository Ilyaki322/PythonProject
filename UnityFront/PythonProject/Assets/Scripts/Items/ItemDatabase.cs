using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField]
    private List<ItemDetails> itemDetailsList;

    private Dictionary<string, ItemDetails> itemDetailsById;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildItemDictionary();
    }

    private void BuildItemDictionary()
    {
        itemDetailsById = new Dictionary<string, ItemDetails>();
        foreach (var item in itemDetailsList)
        {
            if (item != null && !itemDetailsById.ContainsKey(item.Id.ToHexString()))
            {
                itemDetailsById.Add(item.Id.ToHexString(), item);
            }
            else
            {
                Debug.LogWarning($"Duplicate or null item in ItemDatabase: {item?.Name}");
            }
        }
    }

    public ItemDetails GetItemDetailsById(SerializableGuid id)
    {
        if (itemDetailsById.TryGetValue(id.ToHexString(), out var details))
        {
            return details;
        }

        Debug.LogWarning($"Item with ID {id} not found in ItemDatabase.");
        return null;
    }

    public ItemDetails GetItemDetailsById(string id)
    {
        if (itemDetailsById.TryGetValue(id, out var details))
        {
            return details;
        }

        Debug.LogWarning($"Item with ID {id} not found in ItemDatabase.");
        return null;
    }
}
