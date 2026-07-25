using UnityEngine;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour
{
    public static TurretManager singleton;

    [SerializeField]
    [Tooltip("List of turret bases in order of assembly.")]
    public List<TurretBase> availableTurrets;

    public Turret turretPrefab;

    public List<Turret> turretInstances;

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
            turretInstances.Add(Instantiate(turretPrefab, availableTurrets[0].transform.position, Quaternion.identity));
            availableTurrets.RemoveAt(0);
        }
    }


    public void AddTurret(Turret turret)
    {
        turretInstances.Add(turret);
    }
    
    public void DoTurn()
    {
        if (TowerDefenseManager.singleton.isTowerDefenseMode)
        {
            foreach (Turret turret in turretInstances)
            {
                turret.DoTurn();
            }
        }
    }

    public void ReloadAllTurrets()
    {
        foreach(Turret turret in turretInstances)
        {
            turret.Reload(false);
        }
    }

    public void RemoveAllTurretLights()
    {
        foreach(Turret turret in turretInstances)
        {
            Destroy(turret.GetComponent<FogOfWarLight>());
        }
    }
}
