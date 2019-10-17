using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private Animator anim;


    [Header("Stats")]
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float combatRange = 9;



    [Header("Inventory Stuff")]

    [SerializeField]
    private int inventoryMaxCap = 25;

    public List<ItemSO> inventory;

    public int inventorySize;

    [SerializeField]
    private GameObject backpackInCombat;

    [SerializeField]
    private GameObject backpackNonCombat;

    [SerializeField]
    private float backpackToggleSpeed = 0.4f;

    

    [SerializeField][ReadOnlyField]
    private int rouletteIdx = -1;

    [SerializeField][ReadOnlyField]
    private List<ItemSO> rouletteList;

    void Start()
    {
        DevPopulateBag();

        InitializeItemRoulette();

    }

    void Update()
    {
        if (WorldMachine.World.currentState == WorldMachine.State.Walking)
        {
            WalkingUpdate();
        }
        else
        {
            backpackNonCombat.SetActive(false);
            backpackInCombat.SetActive(true);
        }


        if (Input.GetKey(KeyCode.T))
        {
            backpackInCombat.transform.localScale =  Vector3.Lerp(backpackInCombat.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
            //backpackNonCombat.transform.localScale =  Vector3.Lerp(backpackNonCombat.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
        }
        else
        {
            backpackInCombat.transform.localScale = Vector3.Lerp(backpackInCombat.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);
            //backpackNonCombat.transform.localScale = Vector3.Lerp(backpackNonCombat.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);
        }
    }


    private void WalkingUpdate()
    {
        CheckRangeOfEnemies();

        backpackNonCombat.SetActive(true);
        backpackInCombat.SetActive(false);

        //play animation of walking
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void CheckRangeOfEnemies()
    {
        foreach (EnemyController EC in WorldMachine.World.AllEnemies)
        {
            if (EC.transform.position.x - transform.position.x < combatRange)
            {
                WorldMachine.World.enemyInCombat = EC;
                WorldMachine.World.currentState = WorldMachine.State.EnterCombat;
                //turn off animation for walking
                return;
            }
        }
    
    }

    public bool AddItemToBag(ItemSO newItem)
    {
        if(inventorySize + newItem.Size > 25)
        {
            Debug.Log("Space Overflow");
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

    private void ItemRouletteUpdate()
    {
        if (inventory.Count == 0)
        {
            //big sad
            return;
        }

        if (rouletteIdx == -1)
        { 
            
        }



    }

    private void InitializeItemRoulette()
    {
        //get items accesable
        rouletteList.Clear();
        foreach (ItemSO item in WorldMachine.World.AllItems)
        {
            if (inventory.Contains(item) == true && rouletteList.Contains(item) == false)
            {
                rouletteList.Add(item);
            }
        }

    }

    private void DevPopulateBag()
    {
        foreach(ItemSO item in WorldMachine.World.AllItems)
        {
            if (AddItemToBag(item) == false)
            {
            }
        }
    }


}
