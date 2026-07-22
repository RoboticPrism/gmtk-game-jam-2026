using UnityEngine;

public class FogOfWarLight : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How large of a radius this emits light")]
    public int lightRadius;

    public void OnDestroy()
    {
        FogOfWarManager.TriggerRemoveLightUpdate(this);
    }
}
