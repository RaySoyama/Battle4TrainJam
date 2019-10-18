using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum Action
    {
        Idle,
        Attacking,
        Blocking
    }

    public static PlayerManager Player;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Animator outAnim;


    [Header("Stats")]
    public int health;

    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float combatRange = 9;

    [ReadOnlyField]
    public Action currentAction = Action.Idle;

    [ReadOnlyField]
    public ItemSO currentItem;

    [SerializeField]
    private UnityEngine.UI.Text InventorySizeUI;

    [Header("Inventory Stuff")]

    [SerializeField]
    private int inventoryMaxCap = 25;

    public List<ItemSO> inventory;

    public int inventorySize;

    [SerializeField]
    private GameObject backpackInCombat;

    [SerializeField]
    private List<ItemManager> BackpackUI    ;

    [SerializeField]
    private GameObject backpackNonCombat;

    [Space(10)]
    [SerializeField]
    private float backpackToggleSpeed = 0.4f;


    public List<GameObject> ItemSpawnPos;

    public List<ItemManager> ItemSpawnData;


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

        if (Player == null)
        {
            Player = this;
        }


        AddItemToBag(WorldMachine.World.AllItems[0]);
        AddItemToBag(WorldMachine.World.AllItems[1]);
        //DevPopulateBag();

        InitializeItemRoulette();
    }

    void Update()
    {

        InventorySizeUI.text = $"{inventory.Count}/25";

        switch (WorldMachine.World.currentState)
        {
            case WorldMachine.State.None:
                

                break;
            case WorldMachine.State.Walking:
                OnWalkingStay();

                break;
            case WorldMachine.State.EnterCombat:
                OnEnterCombatStay();

                break;
            case WorldMachine.State.PreAction:
                OnPreActionStay();

                break;
            case WorldMachine.State.Action:
                OnActionStay();

                break;
            case WorldMachine.State.PostKill:
                OnPostKillStay();
                break;
            case WorldMachine.State.Death:

                break;
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

        //Dev Test
        if (Input.GetKeyDown(KeyCode.O))
        {
            DoAttackDamageCamdenThisIsYou();
        }


    }


    public void OnWalkingEnter()
    {
        anim.SetBool("isWalking", true);
        outAnim.SetBool("isWalking", true);


        backpackNonCombat.SetActive(true);
        backpackInCombat.SetActive(false);

        CheckRangeOfEnemies();


    }

    private void OnWalkingStay()
    {
        //Remove Item Menu
        roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);

        CheckRangeOfEnemies();

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnWalkingExit()
    {
        anim.SetBool("isWalking", false);
        outAnim.SetBool("isWalking", false);
        InitializeItemRoulette();
    }





    public void OnEnterCombatEnter()
    {
        backpackNonCombat.SetActive(false);
        backpackInCombat.SetActive(true);
    }

    private void OnEnterCombatStay()
    {
        //Open Menu JK
        /*
        roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
        ItemRouletteInput();
        ItemRouletteRender();
         */
    }

    public void OnEnterCombatExit()
    {

    }





    public void OnPreActionEnter()
    {
        anim.SetTrigger("pickItem");
        outAnim.SetTrigger("pickItem");

        backpackNonCombat.SetActive(false);
        backpackInCombat.SetActive(true);
    }

    private void OnPreActionStay()
    {
        
        //Keep Inventory Open 
        //if (WorldMachine.World.currentBeatIndex < 7)
        //{
        //}
        
        ItemRouletteInput();

        ItemRouletteRender();

        PickUpSpawnItemInput();
        roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.one, backpackToggleSpeed * Time.deltaTime);
        
    }
    
    public void OnPreActionExit()
    {
        switch (currentAction)
        {
            case Action.Idle:


                break;

            case Action.Attacking:
                anim.SetTrigger("attack");
                outAnim.SetTrigger("attack");

                break;

            case Action.Blocking:
                anim.SetTrigger("block");
                outAnim.SetTrigger("block");

                break;
        }
    }






    public void OnActionEnter()
    {
        //if(blocking of hitting or neither)

    }

    private void OnActionStay()
    {
        roulleteParent.transform.localScale = Vector3.Lerp(roulleteParent.transform.localScale, Vector3.zero, backpackToggleSpeed * Time.deltaTime);

        //logic
    }

    public void OnActionExit()
    {
        //Determine shit
        InitializeItemRoulette();
    }



    public void OnPostKillEnter()
    { 
        //celebrate Anim if we make one
        //currently an idle
    }
    private void OnPostKillStay()
    {

        ItemRouletteInput();
        ItemRouletteRender();

        PickUpSpawnItemInput();
    }
    public void OnPostKillExit()
    {
        foreach (ItemManager IM in ItemSpawnData)
        {
            if (IM != null)
            { 
                Destroy(IM.gameObject);
            }
        }
        ItemSpawnData.Clear();
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
                OnEnterCombatEnter();
                return;
            }
        }
    
    }


    private void PickUpSpawnItemInput()
    {
        if (currentAction != Action.Idle)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ItemSpawnData[0] != null)
            {
                if (AddItemToBag(ItemSpawnData[0].ItemData) == true)
                {
                    Destroy(ItemSpawnData[0].gameObject);
                    ItemSpawnData[0] = null;
                    InitializeItemRoulette();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (ItemSpawnData[1] != null)
            {
                if (AddItemToBag(ItemSpawnData[1].ItemData) == true)
                {
                    Destroy(ItemSpawnData[1].gameObject);
                    ItemSpawnData[1] = null;
                    InitializeItemRoulette();
                }
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

        foreach (ItemManager IM in BackpackUI)
        { 
            if(IM.ItemData == newItem)
            {
                IM.count++;
                IM.CountText.text = $"x{IM.count}";
            }
        }

        
        //Destroy Item?

        return true;
    }
    
    public void RemoveItemFromBag(ItemSO newItem)
    {
        inventory.Remove(newItem);
        inventorySize -= newItem.Size;

        foreach (ItemManager IM in BackpackUI)
        {
            if (IM.ItemData == newItem)
            {
                IM.count--;
                IM.CountText.text = $"x{IM.count}";
            }
        }

    }

    private void ItemRouletteInput()
    {
        if (inventory.Count == 0)
        {
            return;
        }

        if (rouletteIdx == -1)
        {
            InitializeItemRoulette();
        }


        if (currentAction == Action.Idle)
        {
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


            //Cant 'use' an item outside of preaction
            if (WorldMachine.World.currentState == WorldMachine.State.PreAction)
            { 
                if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //pick item, do damage
                if (rouletteList[rouletteIdx].IsShield == true)
                {
                    currentAction = Action.Blocking;
                }

                if (rouletteList[rouletteIdx].IsShield == false)
                {
                    currentAction = Action.Attacking;
                }


                //Move s hit up

                foreach (ItemManager IM in roulleteObjects)
                {
                    if (IM.ItemData == rouletteList[rouletteIdx])
                    { 
                        IM.gameObject.transform.Translate(Vector3.up);
                    }
                }

                currentItem = rouletteList[rouletteIdx];

                RemoveItemFromBag(rouletteList[rouletteIdx]);
            }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //throw shit out
                RemoveItemFromBag(rouletteList[rouletteIdx]);
                InitializeItemRoulette();
            }



        }
    }

    private void ItemRouletteRender()
    {
        //Render shit
        foreach (ItemManager item in roulleteObjects)
        {
            if (rouletteList.Count == 0)
            {
                item.transform.localScale = Vector3.zero;

                continue;
            }


            if (item.ItemData.ID == rouletteList[rouletteIdx].ID)
            {
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one, Time.deltaTime * backpackToggleSpeed);

                /*
                if (transform.localEulerAngles.y - (30 + (item.ItemData.ID * 60)) > 30)
                {
                    roulleteParent.transform.localEulerAngles = Vector3.Lerp(new Vector3(roulleteParent.transform.localEulerAngles.x, roulleteParent.transform.localEulerAngles.y + 360, roulleteParent.transform.localEulerAngles.z),
                                                                Vector3.up * (30 + (item.ItemData.ID * 60)), Time.deltaTime * backpackToggleSpeed);
                }
                else if (transform.localEulerAngles.y - (30 + (item.ItemData.ID * 60)) < 30)
                {
                    roulleteParent.transform.localEulerAngles = Vector3.Lerp(roulleteParent.transform.localEulerAngles,
                                                              Vector3.up * (390 + (item.ItemData.ID * 60)), Time.deltaTime * backpackToggleSpeed);
                }
                else 
                {
                }
                 */
                    roulleteParent.transform.localEulerAngles = Vector3.Lerp(roulleteParent.transform.localEulerAngles,
                                                                Vector3.up * (30 + (item.ItemData.ID * 60)), Time.deltaTime * backpackToggleSpeed);
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
        currentAction = Action.Idle;
        currentItem = null;

        //get items accesable
        rouletteList.Clear();

        foreach (ItemSO item in WorldMachine.World.AllItems)
        {
            if (inventory.Contains(item) == true && rouletteList.Contains(item) == false)
            {
                rouletteList.Add(item);
            }
        }

        foreach (ItemManager IM in roulleteObjects)
        {
            IM.gameObject.transform.localPosition = new Vector3(IM.gameObject.transform.localPosition.x,0, IM.gameObject.transform.localPosition.z);
        }

        rouletteIdx = 0;
    }
    
    private void DevPopulateBag()
    {
        AddItemToBag(WorldMachine.World.AllItems[5]);

        foreach(ItemSO item in WorldMachine.World.AllItems)
        {
            if (AddItemToBag(item) == false)
            {
                //ree
            }
        }
    }

    

    public void DoAttackDamageCamdenThisIsYou()
    {
        //Enemy should be alive
        //Do Effect
        //oh my god, the reason why we divide by 2, is cuz the outline will also call the fucking do damage function tooooooooo
        WorldMachine.World.enemyInCombat.currentHP -= currentItem.Stat;
    }

}
