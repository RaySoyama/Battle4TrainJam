using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum Action
    {
        Attacking,
        Defending,
        Idle

    }


    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Animator outAnim;


    [Header("Stats")]
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float combatRange = 9;

    public Action currentAction = Action.Idle;

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

    [Space(10)]


    [Header("Roullete Stuff")]

    [SerializeField]
    private Transform roulleteParent;

    [SerializeField][ReadOnlyField]
    private int rouletteIdx = -1;

    [SerializeField][ReadOnlyField]
    private List<ItemSO> rouletteList;

    [SerializeField]
    private List<ItemManager> roulleteObjects;

    void Start()
    {
        DevPopulateBag();

        InitializeItemRoulette();

    }

    void Update()
    {
        if (WorldMachine.World.currentState == WorldMachine.State.Walking)
        {
            OnWalkingStay();

            roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);
            rouletteIdx = -1;
        }
        else if (WorldMachine.World.currentState == WorldMachine.State.EnterCombat)
        {
            anim.SetBool("isWalking", false);
            outAnim.SetBool("isWalking", false);

            //This shit dont work since it needs to turn off in action, and  reset
            roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
            ItemRouletteUpdate();
        }
        else if (WorldMachine.World.currentState == WorldMachine.State.PreAction)
        {
            anim.SetTrigger("pickItem");
            outAnim.SetTrigger("pickItem");
            
            roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
            ItemRouletteUpdate();

            //roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);
            //rouletteIdx = -1;

            backpackNonCombat.SetActive(false);
            backpackInCombat.SetActive(true);
        }
        else if (WorldMachine.World.currentState == WorldMachine.State.Action)
        {
            anim.SetTrigger("block");
            outAnim.SetTrigger("block");

            roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);
            rouletteIdx = -1;
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


    private void OnWalkingEnter()
    {
        anim.SetBool("isWalking", true);
        outAnim.SetBool("isWalking", true);


        backpackNonCombat.SetActive(true);
        backpackInCombat.SetActive(false);

        CheckRangeOfEnemies();
    }

    private void OnWalkingStay()
    {
        CheckRangeOfEnemies();

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnWalkingExit()
    {
        anim.SetBool("isWalking", false);
        outAnim.SetBool("isWalking", false);
    }





    private void OnEnterCombatEnter()
    {



    }

    private void OnEnterCombatStay()
    {


    }

    private void OnEnterCombatExit()
    {


    }


    private void CheckRangeOfEnemies()
    {
        foreach (EnemyController EC in WorldMachine.World.AllEnemies)
        {
            if (EC.transform.position.x - transform.position.x < combatRange)
            {
                WorldMachine.World.enemyInCombat = EC;
                WorldMachine.World.currentState = WorldMachine.State.EnterCombat;

                OnWalkingExit();
                //onEntercombat
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
            return;
        }

        if (rouletteIdx == -1)
        {
            InitializeItemRoulette();
        }


        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rouletteIdx++;
            if (rouletteIdx == rouletteList.Count)
            {
                rouletteIdx = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rouletteIdx--;
            if (rouletteIdx == -1)
            {
                rouletteIdx = rouletteList.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

        }


        //Render shit

        //this some gross shit
        foreach(ItemManager item in roulleteObjects)
        {
            if (item.ItemData.ID == rouletteList[rouletteIdx].ID)
            {
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one, Time.deltaTime * backpackToggleSpeed);

                roulleteParent.transform.localEulerAngles = Vector3.Lerp(roulleteParent.transform.localEulerAngles,Vector3.up * (90 + (item.ItemData.ID * 60)), Time.deltaTime * backpackToggleSpeed);
                //not a high prio fix for the loop back bug
            }
            else
            {
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one * 0.1f, Time.deltaTime * backpackToggleSpeed);
            }
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
        rouletteIdx = 0;
    }

    private void DevPopulateBag()
    {
        foreach(ItemSO item in WorldMachine.World.AllItems)
        {
            if (AddItemToBag(item) == false)
            {
                //ree
            }
        }
    }   
}
