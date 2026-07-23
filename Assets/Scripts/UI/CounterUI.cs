using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [SerializeField]
    private GameObject container;

    public void Start()
    {
        container.SetActive(false);
    }

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

    public void PlayIntro(Pyre pyre)
    {
        StartCoroutine(PlayIntroAnimation(pyre));
    }

    IEnumerator PlayIntroAnimation(Pyre pyre)
    {
        // Start over pyre
        transform.position = Camera.main.WorldToScreenPoint(pyre.transform.position + new Vector3(-0.5f,-0.5f,0));
        container.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        // Lerp to final position
        RectTransform rectTransform = GetComponent<RectTransform>();
        float lerpProgress = 0f;
        while (lerpProgress <= 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(transform.position, Vector2.zero, lerpProgress);
            lerpProgress += Time.deltaTime;
            yield return null;
        }
    }
}
