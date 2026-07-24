using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private GameObject minimapContainer;

    // Update is called once per frame
    void Update()
    {
        minimapContainer.SetActive(!TowerDefenseManager.singleton.isTowerDefenseMode);
    }
}
