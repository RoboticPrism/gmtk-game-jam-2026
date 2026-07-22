using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum ResourceType { WOOD, STONE, MUSHROOM }

public class ResourceDictionary : MonoBehaviour
{

    public static ResourceDictionary singleton;

    [SerializeField]
    [Tooltip("The global prefab to use for dropping resources")]
    public ResourceDrop resourceDropPrefab;

    [System.Serializable]
    public class ResourceData
    {
        public ResourceType resourceType;
        public Sprite resourceArt;
    }

    public List<ResourceData> resourceDatas;

    public ResourceData GetResourceDataByType(ResourceType resourceType)
    {
        return resourceDatas.First(data => data.resourceType == resourceType);
    }

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another resource dictionary already exists!");
        }
        else
        {
            singleton = this;
        }
    }
}
