using System;
using UnityEngine;

[System.Serializable]
public class ItemDetailsDTO
{
    public string id;
    public string name;
    public string description;
    public string icon;

    public ItemDetailsDTO(ItemDetails item)
    {
        id = item.Id.ToHexString();
        name = item.Name;
        description = item.Description;

        if (item.Icon != null)
        {
            Texture2D tex = item.Icon.texture;
            byte[] bytes = tex.EncodeToPNG();
            icon = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        else
        {
            icon = null;
        }
    }

    [System.Serializable]
    public class ItemDetailsDTOList
    {
        public ItemDetailsDTO[] array;

        public ItemDetailsDTOList(ItemDetailsDTO[] items)
        {
            array = items;
        }
    }
}