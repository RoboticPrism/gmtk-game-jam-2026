using UnityEngine;
using System.Collections;

public class ResourceSpend : MonoBehaviour
{
    [SerializeField]
    [Tooltip("What type of resource is this")]
    public ResourceType resourceType;

    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("How long does this move towards the target")]
    private float moveDuration;

    [SerializeField]
    [Tooltip("How long does this sit on the ground before magnetizing to the target")]
    private float collectionDelaySeconds;

    [SerializeField]
    [Tooltip("How close to the player is considered close enough to be collected")]
    private float collectionRange;

    [SerializeField]
    [Tooltip("How far items get flung from their source")]
    private float flingRange;

    private Vector3 flingTarget;

    [SerializeField]
    private Transform finalTarget;

    public void Start()
    {
        flingTarget = transform.position + new Vector3(Random.Range(-flingRange, flingRange), Random.Range(-flingRange, flingRange), 0);
    }

    public void SetResourceTypeAndTarget(ResourceType type, Transform target)
    {
        resourceType = type;
        spriteRenderer.sprite = ResourceDictionary.singleton.GetResourceDataByType(type).resourceArt;
        finalTarget = target;
        StartCoroutine(ResourceCollect());
    }

    IEnumerator ResourceCollect()
    {
        // fling out
        float elapsedTime = 0f;
        while(Vector3.Distance(flingTarget, transform.position) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, flingTarget, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // wait for a bit
        yield return new WaitForSeconds(collectionDelaySeconds);

        // magnetize to the target
        elapsedTime = 0f;
        while(Vector3.Distance(finalTarget.position, transform.position) > collectionRange)
        {
            transform.position = Vector3.Lerp(transform.position, finalTarget.position, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // destroy this
        Destroy(gameObject);
    }
}
