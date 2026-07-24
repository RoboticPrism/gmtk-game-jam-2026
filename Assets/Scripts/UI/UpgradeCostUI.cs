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

    [SerializeField]
    private Transform background;

    [SerializeField]
    private float initialBackgroundScale = 0.4f;
    
    [SerializeField]
    private float backgroundScaleIncrement = 0.1f;
    
    [SerializeField]
    private float initialBackgroundOffset = -0.3f;
    
    [SerializeField]
    private float backgroundOffsetIncrement = -0.1f;

    [SerializeField]
    private Transform hoverArea;

    [SerializeField]
    private float hoverOffset;

    [SerializeField]
    private float hoverSpeed;

    [SerializeField]
    private bool hoverDown = false;

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
        background.localPosition += Vector3.up * (initialBackgroundOffset + ((cost.subcosts.Count - 1) * backgroundOffsetIncrement));
        background.localScale += Vector3.up * (initialBackgroundScale + ((cost.subcosts.Count - 1) * backgroundScaleIncrement));
    }

    public void Update()
    {
        if(hoverDown)
        {
            hoverArea.transform.localPosition += Vector3.down * hoverSpeed * Time.deltaTime;
            if(hoverArea.transform.localPosition.y < 0)
            {
                hoverDown = false;
            }
        }   
        else
        {
            hoverArea.transform.localPosition += Vector3.up * hoverSpeed * Time.deltaTime;
            if(hoverArea.transform.localPosition.y > hoverOffset)
            {
                hoverDown = true;
            }
        }
    }
}
