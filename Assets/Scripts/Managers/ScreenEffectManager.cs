using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffectManager : MonoBehaviour
{
    public static ScreenEffectManager singleton;

    [SerializeField]
    private Image fade;

    [SerializeField]
    private float fadeSeconds;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("A screen effect manager already exists!");
        }
        else
        {
            singleton = this;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutAnimation());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInAnimation());
    }

    IEnumerator FadeOutAnimation()
    {
        Color color = fade.color;
        float currentTime = 0f;
        while (currentTime < fadeSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeSeconds);
            fade.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeInAnimation()
    {
        Color color = fade.color;
        float currentTime = 0f;
        while (currentTime < fadeSeconds)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeSeconds);
            fade.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
