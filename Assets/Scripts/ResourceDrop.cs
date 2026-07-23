using UnityEngine;
using System.Collections;

public class ResourceDrop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("What type of resource is this")]
    public ResourceType resourceType;

    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("How long does this move towards the player")]
    private float moveDuration;

    [SerializeField]
    [Tooltip("How long does this sit on the ground before magnetizing to the player")]
    private float collectionDelaySeconds;

    [SerializeField]
    [Tooltip("How close to the player is considered close enough to be collected")]
    private float collectionRange;

    public void SetResourceType(ResourceType type)
    {
        resourceType = type;
        spriteRenderer.sprite = ResourceDictionary.singleton.GetResourceDataByType(type).resourceArt;
        StartCoroutine(ResourceCollect());
    }

    IEnumerator ResourceCollect()
    {
        // wait for a bit
        yield return new WaitForSeconds(collectionDelaySeconds);

        // magnetize to the player
        float elapsedTime = 0f;
        while(Vector3.Distance(Player.singleton.transform.position, transform.position) > collectionRange)
        {
            transform.position = Vector3.Lerp(transform.position, Player.singleton.transform.position, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // add it to inventory
        Player.singleton.playerInventory.AddItemToInventory(resourceType);

        // destroy this
        Destroy(gameObject);
    }
}

