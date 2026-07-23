using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField]
    private Image resourceImage;

    [SerializeField]
    private TextMeshProUGUI amountText;

    public ResourceType resourceType;

    public void SetType(ResourceType resourceType)
    {
        this.resourceType = resourceType;
        resourceImage.sprite = ResourceDictionary.singleton.GetResourceDataByType(resourceType).resourceArt;
    }

    // Update is called once per frame
    void Update()
    {
        amountText.text = Player.singleton.playerInventory.GetHeldAmount(resourceType).ToString();
    }
}
