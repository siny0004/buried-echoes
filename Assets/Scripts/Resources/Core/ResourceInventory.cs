
using System.Collections.Generic;

namespace BuriedEchoes.Resources
{
    public class ResourceInventory
    {
        private readonly Dictionary<ResourceType, int> resources = new();

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0)
                return;

            resources.TryGetValue(type, out int currentAmount);
            resources[type] = currentAmount + amount;
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
