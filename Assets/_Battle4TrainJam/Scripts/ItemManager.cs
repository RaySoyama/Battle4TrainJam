using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private ItemSO itemData;

    public ItemSO ItemData
    {
        get
        {
            return itemData;
        }
    }

}
