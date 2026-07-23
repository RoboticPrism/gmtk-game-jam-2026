using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The inventory slot ui prefab to instantiate")]
    private InventorySlotUI inventorySlotUIPrefab;

    [SerializeField]
    private Transform inventorySlotArea;

    List<InventorySlotUI> inventorySlotUIs = new List<InventorySlotUI>();

    public void Start()
    {
        foreach(ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            InventorySlotUI slotUI = Instantiate(inventorySlotUIPrefab, inventorySlotArea);
            slotUI.SetType(resourceType);
            inventorySlotUIs.Add(slotUI);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only show inventory we have
        foreach(InventorySlotUI slotUI in inventorySlotUIs)
        {
            slotUI.gameObject.SetActive(Player.singleton.playerInventory.GetHeldAmount(slotUI.resourceType) > 0);
        }
    }
}
