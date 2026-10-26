
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

            collected = true;
            inventory.Add(resourceType, amount);

            Debug.Log($"Collected {amount} {resourceType}!");

            Destroy(gameObject);
            return true;
        }
    }
}
