using System;
using ControlManager;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Scriptable Objects/EnemyProfile")]
public class EnemyProfile : ScriptableObject
{
    public string Name;
    public float AttackSpeed;
    public float FuelGauge;
    public int speed;
    public EnemyController prefabEnemy;
   

    public EnemyProfile()
    {
        AttackSpeed = 2f;
        FuelGauge = 5f;
    }

   

    public EnemyController getPrefabEnemy()
    {
        if (prefabEnemy) return prefabEnemy;
        else
        {
            return null;
        }
    }

   
}
