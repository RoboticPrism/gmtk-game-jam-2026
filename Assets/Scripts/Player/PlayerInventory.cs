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

    public void AddItemToInventory(ResourceType resourceType, int amount = 1)
    {
        ResourceInventorySlot preexistingSlot = resourceInventorySlots.FirstOrDefault(slot => slot.resourceType == resourceType);
        if(preexistingSlot != null)
        {
            preexistingSlot.amount += amount;
        }
        else
        {
            resourceInventorySlots.Add(new ResourceInventorySlot(resourceType, amount));
        }
    }

    public void RemoveItemFromInventory(ResourceType resourceType, int amount)
    {
        ResourceInventorySlot preexistingSlot = resourceInventorySlots.FirstOrDefault(slot => slot.resourceType == resourceType);
        if(preexistingSlot != null)
        {
            preexistingSlot.amount -= amount;
            if(preexistingSlot.amount < 0)
            {
                Debug.LogError("Went negative on resource! " + resourceType.ToString());
            }
        }
        else
        {
            Debug.LogError("No resource to remove! " + resourceType.ToString());
        }
    }

    public int GetHeldAmount(ResourceType resourceType)
    {
        ResourceInventorySlot preexistingSlot = resourceInventorySlots.FirstOrDefault(slot => slot.resourceType == resourceType);
        if(preexistingSlot != null)
        {
            return preexistingSlot.amount;
        }
        else
        {
            return 0;
        }
    }
}
