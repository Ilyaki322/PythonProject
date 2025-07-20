public class Item
{
    public SerializableGuid Id;
    public ItemDetails Details;
    public int Quantity;
    public Item(ItemDetails details, int quantity)
    {
        Id = details.Id;
        Details = details;
        Quantity = quantity;
    }
}