
namespace BuriedEchoes.Resources
{
    public class ResourceSeller
    {
        public int SellAll(ResourceInventory inventory)
        {
            if (inventory == null)
                return 0;

            int totalEarned = 0;

            totalEarned += SellResource(
                inventory, ResourceType.Copper, 10);

            totalEarned += SellResource(
                inventory, ResourceType.Iron, 5);
            
            totalEarned += SellResource(
                inventory, ResourceType.Silver, 20);

            totalEarned += SellResource(
                inventory, ResourceType.Gold, 40);

            return totalEarned;
        }

        private int SellResource(
            ResourceInventory inventory,
            ResourceType type,
            int price)
        {
            int amount = inventory.GetAmount(type);

            if (amount <= 0)
                return 0;

            if (!inventory.TryRemove(type, amount))
                return 0;

            return amount * price;
        }

        public int GetTotalValue(ResourceInventory inventory)
        {
            if (inventory == null)
                return 0;

            return inventory.GetAmount(ResourceType.Copper) * 10
                 + inventory.GetAmount(ResourceType.Iron) * 5
                 + inventory.GetAmount(ResourceType.Silver) * 20
                 + inventory.GetAmount(ResourceType.Gold) * 40;
        }
    }
}
