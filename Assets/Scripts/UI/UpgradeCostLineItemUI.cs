using UnityEngine;
using TMPro;

public class UpgradeCostLineItemUI : MonoBehaviour
{
    public SpriteRenderer icon;
    public TextMeshPro text;

    private ResourceType resourceType;
    private int cost;

    [SerializeField]
    private Color goodColor;

    [SerializeField]
    private Color badColor;

    public void Setup(PlayerUpgrades.UpgradeSubcost subcost)
    {
        resourceType = subcost.resourceType;
        cost = subcost.amount;
        icon.sprite = ResourceDictionary.singleton.GetResourceDataByType(subcost.resourceType).resourceArt;
        text.text = "x" + subcost.amount;
    }

    public void Update()
    {
        if(Player.singleton.playerInventory.GetHeldAmount(resourceType) < cost)
        {
            text.color = badColor;
        }
        else
        {
            text.color = goodColor;
        }
    }
}
