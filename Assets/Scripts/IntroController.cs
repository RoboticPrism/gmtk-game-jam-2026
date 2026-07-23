using UnityEngine;
using TMPro;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI firstText;

    [SerializeField]
    [Tooltip("How long the first text stays on screen")]
    private float firstTextDurationSeconds;

    [SerializeField]
    [Tooltip("How long the first text fades from screen")]
    private float firstTextFadeSeconds;

    [SerializeField]
    private TextMeshProUGUI secondText;

    [SerializeField]
    [Tooltip("How long the second text stays on screen")]
    private float secondTextDurationSeconds;

    [SerializeField]
    [Tooltip("How long the second text fades from screen")]
    private float secondTextFadeSeconds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Animate());    
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(firstTextDurationSeconds);

        Color color = firstText.color;
        float currentTime = 0f;
        while (currentTime < firstTextFadeSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / firstTextFadeSeconds);
            firstText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        firstText.gameObject.SetActive(false);

        secondText.gameObject.SetActive(true);

        yield return new WaitForSeconds(secondTextDurationSeconds);

        color = secondText.color;
        currentTime = 0f;
        while (currentTime < secondTextFadeSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / secondTextFadeSeconds);
            secondText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
