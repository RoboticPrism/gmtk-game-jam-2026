using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterUI : MonoBehaviour
{
    [SerializeField]
    private Sprite highFireCount;

    [SerializeField]
    private int highFireThreshold;

    [SerializeField]
    private Sprite mediumFireCount;

    [SerializeField]
    private int mediumFireThreshold;

    [SerializeField]
    private Sprite lowFireCount;

    [SerializeField]
    private int lowFireThreshold;

    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private TextMeshProUGUI counterText;

    // Update is called once per frame
    void Update()
    {
        UpdateCount();
    }

    private void UpdateCount()
    {
        // Update text
        counterText.text = CounterManager.singleton.steps.ToString();

        // Step the background based on how many steps are left
        if(CounterManager.singleton.steps >= highFireThreshold)
        {
            backgroundImage.sprite = highFireCount;
        }
        else if (CounterManager.singleton.steps >= mediumFireThreshold)
        {
            backgroundImage.sprite = mediumFireCount;
        }
        else
        {
            backgroundImage.sprite = lowFireCount;
        }
    }
}
