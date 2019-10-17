using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private int inventoryMaxCap = 25;



    public List<ItemSO> inventory;

    public int inventorySize;




    void Start()
    {
        
    }

    void Update()
    {
        
    }


    
    public bool AddItemToBag(ItemSO newItem)
    {
        if(inventorySize + newItem.Size > 25)
        {
            return false;
        }

        inventory.Add(newItem);
        inventorySize += newItem.Size;
        //Destroy Item?
        //Add to Visual Inventory
        return true;
    }

    public void RemoveItemFromBag(ItemSO newItem)
    {
        inventory.Remove(newItem);
        inventorySize -= newItem.Size;    
    }




}
