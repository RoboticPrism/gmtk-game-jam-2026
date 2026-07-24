using UnityEngine;
using TMPro;
using System.Collections;

public class Pyre : BumpableTile
{
    public static Pyre singleton;

    [SerializeField]
    [Tooltip("How much wood it costs to refueld the fire")]
    private int refuelCost;

    [SerializeField]
    [Tooltip("How many steps you get back from refueling")]
    private int refuelSteps;

    [SerializeField]
    [Tooltip("How much health the pyre starts with")]
    public int health;

    [SerializeField]
    private TextMeshPro counterText;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite highFireCount;

    [SerializeField]
    private Sprite highFireCountCombat;

    [SerializeField]
    private int highFireThreshold;

    [SerializeField]
    private int highHealthThreshold;

    [SerializeField]
    private Sprite mediumFireCount;

    [SerializeField]
    private Sprite mediumFireCountCombat;

    [SerializeField]
    private int mediumFireThreshold;

    [SerializeField]
    private int mediumHealthThreshold;

    [SerializeField]
    private Sprite lowFireCount;

    [SerializeField]
    private Sprite lowFireCountCombat;

    [SerializeField]
    private int lowFireThreshold;

    [SerializeField]
    private int lowHealthThreshold;

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
    private bool hasFinishedAnimation = false;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("A pyre already exists!");
        }
        else
        {
            singleton = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        counterText.gameObject.SetActive(false);  
    }

    // Update is called once per frame
    void Update()
    {
            // Step the background based on how many health is left
        if (TowerDefenseManager.singleton.isTowerDefenseMode)
        {
            counterText.gameObject.SetActive(false);

            if (CounterManager.singleton.steps >= highHealthThreshold)
            {
                spriteRenderer.sprite = highFireCountCombat;
            }
            else if (CounterManager.singleton.steps >= mediumHealthThreshold)
            {
                spriteRenderer.sprite = mediumFireCountCombat;
            }
            else
            {
                spriteRenderer.sprite = lowFireCountCombat;
            }
        }
        else
        {
            // Step the background based on how many steps are left
            if (CounterManager.singleton.steps >= highFireThreshold)
            {
                spriteRenderer.sprite = highFireCount;
            }
            else if (CounterManager.singleton.steps >= mediumFireThreshold)
            {
                spriteRenderer.sprite = mediumFireCount;
            }
            else
            {
                spriteRenderer.sprite = lowFireCount;
            }

            counterText.gameObject.SetActive(hasFinishedAnimation);
            counterText.text = CounterManager.singleton.steps.ToString();
        }

        // Check when to play intro animation
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
        hasFinishedAnimation = true;

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
