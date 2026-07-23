using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    public List<ResourceInventorySlot> resourceInventorySlots;

    [SerializeField]
    [Tooltip("The prefab to create when spending resources")]
    private ResourceSpend resourceSpendPrefab;

    [SerializeField]
    [Tooltip("How long between each resource visually being extracted in seconds")]
    private float resourceSpendDelaySeconds;

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

    public void RemoveItemFromInventory(ResourceType resourceType, int amount = 1)
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

    public void SpendResourcesOnTarget(ResourceType resourceType, Transform target, int amount)
    {
        StartCoroutine(SpendResourcesAnimation(resourceType, target, amount)); 
    }

    IEnumerator SpendResourcesAnimation(ResourceType resourceType, Transform target, int amount)
    {
        int amountDispensed = 0;
        while (amountDispensed < amount)
        {
            ResourceSpend resourceSpend = Instantiate(resourceSpendPrefab, transform.position, Quaternion.identity);
            resourceSpend.SetResourceTypeAndTarget(resourceType, target);
            RemoveItemFromInventory(resourceType);
            yield return new WaitForSeconds(resourceSpendDelaySeconds);
            amountDispensed++;
        }
    }
}
