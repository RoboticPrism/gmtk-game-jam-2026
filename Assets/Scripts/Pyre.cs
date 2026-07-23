using UnityEngine;
using TMPro;
using System.Collections;

public class Pyre : BumpableTile
{

    [SerializeField]
    [Tooltip("How much wood it costs to refueld the fire")]
    private int refuelCost;

    [SerializeField]
    [Tooltip("How many steps you get back from refueling")]
    private int refuelSteps;

    [SerializeField]
    private TextMeshPro counterText;

    [SerializeField]
    private TextMeshPro title;

    [SerializeField]
    private float titleHoldSeconds;

    [SerializeField]
    private float fadeInSeconds;

    [SerializeField]
    private float holdSeconds;

    [SerializeField]
    private float fadeOutSeconds;

    [SerializeField]
    private TextMeshPro subtext1;
    
    [SerializeField]
    private TextMeshPro subtext2;

    private bool hasAnimated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counterText.gameObject.SetActive(false);  
    }

    // Update is called once per frame
    void Update()
    {
        counterText.text = CounterManager.singleton.steps.ToString();

        if(Vector3.Distance(Player.singleton.transform.position, transform.position) < 10f && !hasAnimated) {
            hasAnimated = true;
            StartCoroutine(PyreTextAnimation());
        }
    }

    public override void OnBump()
    {
        base.OnBump();
        if(Player.singleton.playerInventory.GetHeldAmount(ResourceType.WOOD) >= refuelCost)
        {
            Player.singleton.playerInventory.SpendResourcesOnTarget(ResourceType.WOOD, transform, refuelCost);
            CounterManager.singleton.AddSteps(refuelSteps);
        }
        else
        {

        }
    }

    IEnumerator PyreTextAnimation()
    {
        // Pop title
        title.gameObject.SetActive(true);

        // Hold
        yield return new WaitForSeconds(titleHoldSeconds);

        // Fade in subtext 1
        Color color = subtext1.color;
        float currentTime = 0f;
        while (currentTime < fadeInSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInSeconds);
            subtext1.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Fade in subtext 2
        color = subtext2.color;
        currentTime = 0f;
        while (currentTime < fadeInSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInSeconds);
            subtext2.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Play the counter intro
        FindAnyObjectByType<CounterUI>().PlayIntro(this);

        // Actually start counting steps
        CounterManager.singleton.StartCounting();

        yield return new WaitForSeconds(1f);

        // Activate the pyre's counter too
        counterText.gameObject.SetActive(true);  

        // Hold
        yield return new WaitForSeconds(holdSeconds);

        // Fade out all
        color = subtext1.color;
        currentTime = 0f;
        while (currentTime < fadeOutSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutSeconds);
            title.color = new Color(color.r, color.g, color.b, alpha);
            subtext1.color = new Color(color.r, color.g, color.b, alpha);
            subtext2.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
