using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Rays Objects/New Item", order = 1)]
public class ItemSO : ScriptableObject
{
    [SerializeField]
    private bool isShield = false;

    public bool IsShield
    {
        get
        {
            return isShield;
        }
    }


    [SerializeField]
    private int stat = 1;

    public int Stat
    {
        get
        {
            return stat;
        }
    }

    [SerializeField]
    private int size = 1;

    public int Size
    {
        get
        {
            return size;
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
}


