using UnityEngine;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour
{
    public static TurretManager singleton;

    [SerializeField]
    [Tooltip("List of turret bases in order of assembly.")]
    public List<TurretBase> availableTurrets;

    public Turret turretPrefab;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another turret manager already exists!");
        }
        else
        {
            singleton = this;
        }
        availableTurrets = new List<TurretBase>(FindObjectsByType<TurretBase>());
    }

    public void AssembleTurret()
    {
        if (availableTurrets.Count > 0) {
            Instantiate(turretPrefab, availableTurrets[0].transform.position, Quaternion.identity);
            availableTurrets.RemoveAt(0);
        }
    }
}
