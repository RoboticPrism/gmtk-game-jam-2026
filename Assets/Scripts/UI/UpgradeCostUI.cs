using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UpgradeCostUI : MonoBehaviour
{
    [SerializeField]
    private UpgradeCostLineItemUI upgradeCostLineItemUIPrefab;

    [SerializeField]
    private TextMeshPro productText;

    [SerializeField]
    private float yOffsetIncrement;

    [SerializeField]
    private float yOffsetStart;

    [SerializeField]
    private Transform lineItemArea;

    private List<UpgradeCostLineItemUI> upgradeCostLineItemUIInstances = new List<UpgradeCostLineItemUI>();

    public void Setup(string title, Cost cost)
    {
        productText.text = title;
        float yOffset = yOffsetStart;
        foreach(Subcost subcost in cost.subcosts)
        {
            UpgradeCostLineItemUI newLineItem = Instantiate(upgradeCostLineItemUIPrefab, lineItemArea);
            newLineItem.transform.localPosition += Vector3.down * yOffset;
            newLineItem.Setup(subcost);
            upgradeCostLineItemUIInstances.Add(newLineItem);
            yOffset += yOffsetIncrement;
        }
    }
}
