using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    private enum TutorialMode { TIMEBASED_UPON_VIEW, BUMP_INTERACT, PLAYER_DISTANCE }

    [SerializeField]
    private TutorialMode tutorialMode;

    [SerializeField]
    private float fadeAwaySeconds;

    [SerializeField]
    private TextMeshPro tutorialText;

    [SerializeField]
    [Tooltip("If set to bump interact, which target are we waiting to be interacted with.")]
    private Vector3Int bumpTargetLocation;
    private BumpableTile bumpableTile;

    [SerializeField]
    [Tooltip("If set to distance, how close the player needs to be to clear it.")]
    private float distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(tutorialMode == TutorialMode.BUMP_INTERACT)
        {
            bumpableTile = GridManager.singleton.resourceTilemap.GetInstantiatedObject(GridManager.singleton.resourceTilemap.WorldToCell(bumpTargetLocation)).GetComponent<BumpableTile>();
            bumpableTile.OnInteract += ClearTutorial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(tutorialMode)
        {
            case TutorialMode.TIMEBASED_UPON_VIEW:
                break;
            case TutorialMode.PLAYER_DISTANCE:
                if(Vector3.Distance(Player.singleton.transform.position, transform.position) <= distance)
                {
                    ClearTutorial();
                }
                break;
        }
    }

    private Coroutine clearTutorialCoroutine;
    private void ClearTutorial()
    {
        if (clearTutorialCoroutine == null)
        {
            clearTutorialCoroutine = StartCoroutine(ClearTutorialAnimation());
        }
    }

    IEnumerator ClearTutorialAnimation()
    {
        Color color = tutorialText.color;
        float currentTime = 0f;
        while (currentTime < fadeAwaySeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeAwaySeconds);
            tutorialText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }


}
