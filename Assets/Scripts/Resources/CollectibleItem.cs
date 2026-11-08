
using UnityEngine;

namespace BuriedEchoes.Resources
{
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType = ResourceType.Gold;
        [SerializeField, Min(1)] private int amount = 1;
        public ResourceType ResourceType => resourceType;
        private bool collected;

        public bool TryCollect(ResourceInventory inventory)
        {
            if (collected || inventory == null)
                return false;

            // Try to add the mineral to the inventory.
            if (!inventory.Add(resourceType, amount))
                return false;

            collected = true;

            Destroy(gameObject);
            return true;
        }
    }
}
