using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [ReadOnlyField]
    public int count = 0;

    [SerializeField]
    private Text countText;

    public Text CountText
    { 
        get
        {
            return countText;
        }

    }


}
