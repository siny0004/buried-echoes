
using System.Collections.Generic;

namespace BuriedEchoes.Resources
{
    public class ResourceInventory
    {
        private readonly Dictionary<ResourceType, int> resources = new();
        private static readonly int[] capacities = { 5, 8, 12, 18, 25 };
        private static readonly int[] upgradeCosts = { 100, 300, 750, 1500 };

        public int InventoryLevel { get; private set; } = 1;

        public int Capacity => capacities[InventoryLevel - 1];

        public bool CanUpgradeInventory => InventoryLevel < capacities.Length;

        public int NextInventoryUpgradeCost =>
            CanUpgradeInventory ? upgradeCosts[InventoryLevel - 1] : 0;

        public bool TryUpgradeInventory()
        {
            if (!CanUpgradeInventory)
                return false;

            InventoryLevel++;

            return true;
        }

        public int TotalItems
        {
            get
            {
                int total = 0;

                foreach (int amount in resources.Values)
                {
                    total += amount;
                }

                return total;
            }
        }

        public bool IsFull => TotalItems >= Capacity;

        public bool Add(ResourceType type, int amount)
        {
            if (amount <= 0)
                return false;

            if (TotalItems + amount > Capacity)
                return false;

            resources.TryGetValue(type, out int currentAmount);
            resources[type] = currentAmount + amount;

            return true;
        }

        public int GetAmount(ResourceType type)
        {
            return resources.TryGetValue(type, out int amount)
                ? amount
                : 0;
        }

        public bool TryRemove(ResourceType type, int amount)
        {
            if (amount <= 0)
                return false;

            int currentAmount = GetAmount(type);

            if (currentAmount < amount)
                return false;

            resources[type] = currentAmount - amount;
            return true;
        }
    }
}
