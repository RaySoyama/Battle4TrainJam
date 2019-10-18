using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    private EnemySO enemyStats;

    public EnemySO EnemyStats
    {
        get
        {
            return enemyStats;
        }
    }

    public int currentHP;


    void Start()
    {
        currentHP = enemyStats.Health;
    }



}
