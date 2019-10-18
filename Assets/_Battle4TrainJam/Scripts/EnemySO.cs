using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Rays Objects/New Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public enum Action
    { 
        Attack,
        Vulnerable
    };

    [SerializeField]
    private int health = 1;

    public int Health
    {
        get
        {
            return health;
        }
    }


    [SerializeField]
    private int attack = 1;

    public int Attack
    {
        get
        {
            return attack;
        }
    }

    [SerializeField]
    private int rarity = 1;

    public int Rarity
    {
        get
        {
            return rarity;
        }

    }


    [SerializeField]
    private List<Action> attackPattern;

    public List<Action> AttackPattern
    {
        get
        {
            return attackPattern;
        }
    }


    [SerializeField]
    private List<ItemSO> onDeathDrops = new List<ItemSO>();

    public List<ItemSO> OnDeathDrops
    {
        get
        {
            return onDeathDrops;
        }
    }

    [SerializeField]
    private List<ItemSO> onTurnDrops = new List<ItemSO>();

    public List<ItemSO> OnTurnDrops
    {
        get
        {
            return onTurnDrops;
        }
    }




}


