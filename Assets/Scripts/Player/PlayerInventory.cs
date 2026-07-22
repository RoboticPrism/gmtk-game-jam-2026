using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    public List<ResourceInventorySlot> resourceInventorySlots;

    [System.Serializable]
    public class ResourceInventorySlot
    {
        public ResourceType resourceType;
        public int amount;

        public ResourceInventorySlot(ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }
    }

    public void AddItemToInventory(ResourceType resourceType)
    {
        ResourceInventorySlot preexistingSlot = resourceInventorySlots.FirstOrDefault(slot => slot.resourceType == resourceType);
        if(preexistingSlot != null)
        {
            preexistingSlot.amount += 1;
        }
        else
        {
            resourceInventorySlots.Add(new ResourceInventorySlot(resourceType, 1));
        }
    }
}
