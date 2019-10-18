using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
public class WorldMachine : MonoBehaviour
{
    public enum State
    { 
        None,
        Walking,
        EnterCombat,
        PreAction,
        Action,
        PostKill,
        Death
    }


    public static WorldMachine World;


    [SerializeField]
    private List<string> AudioNameList;


    [SerializeField]
    private List<AudioSource> AudioSourceList;

    private Dictionary<string, AudioSource> AudioLibrary;


    [Space(10)]
    [SerializeField]
    public List<ItemSO> AllItems = new List<ItemSO>();
    //Small to Large, Shield to Sword

    
    [Space(10)]

    [SerializeField]
    private CinemachineVirtualCamera walkingCam;

    [SerializeField]
    private CinemachineVirtualCamera combatCam;

    [Space(10)]

    [SerializeField]
    private float BeatsPerMinute = 120;

    public int currentBeatIndex;


    [Space(10)]
    [Header("Enemy Stuff")]

    [ReadOnlyField]
    public EnemyController enemyInCombat;

    [ReadOnlyField]
    public State currentState;

    public List<EnemyController> AllEnemies;

    //player ref
    //enemy ref

    [SerializeField]
    private Animator beatAnim;

    [Space(10)]

    [SerializeField]
    private GameObject GameOverScreen;

    //Colors for abilities
    public Color Rare1;
    public Color Rare2;
    public Color Rare3;



    private Coroutine TimerCourtine = null;

    private delegate void MyDelegate();
    private MyDelegate FunctionToDo;


    private bool audioSetUp = false;

    private bool isInDragonArea = false;

    private float beatDuration;
    void Start()
    {
        if (World == null)
        {
            World = this;
        }

        currentBeatIndex = 0;

        beatDuration = 60.0f / BeatsPerMinute;

        InvokeRepeating("HeartBeatUpdate", 1.0f, beatDuration);


        GameOverScreen.SetActive(false);

        //Make Dictonary of all sounds
        AudioLibrary = new Dictionary<string, AudioSource>();

        for (int i = 0; i < AudioSourceList.Count; i++)
        {
            AudioLibrary.Add(AudioNameList[i], AudioSourceList[i]);
        }


        AudioStarter();

    }

    void Update()
    {
        //Camera
        if (currentState == State.Walking)
        {
            walkingCam.Priority = 11;
            combatCam.Priority = 10;


        }
        else
        {
            walkingCam.Priority = 10;
            combatCam.Priority = 11;

        }


        //ADD THIS

        //Enter Area One
        AudioLibrary["IntroLoop"].volume = 0.5f;
        AudioLibrary["DungeonLoop"].volume = 0.5f;




        switch (currentState)
        {
            case State.None:
                break;
            case State.Walking:

                if (currentBeatIndex != 1)
                {
                    return;
                }

                //wait 4 seconds
                if (TimerCourtine == null)
                {
                    TimerCourtine = StartCoroutine(TimerCour(4.0f));

                    PlayerManager.Player.OnWalkingEnter();

                    //Audio
                    //AudioLibrary["Bass3"].volume = 1;
                    //AudioLibrary["Synth3"].volume = 1;
                    //AudioLibrary["Uke3"].volume = 1;
                }
                break;
            case State.EnterCombat:
                
                //Handle Time Offset here
                if(currentBeatIndex != 1)
                {
                    return;
                }

                //wait 4 seconds
                if (TimerCourtine == null)
                {
                    FunctionToDo += PlayerManager.Player.OnEnterCombatExit;

                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.PreAction));

                    enemyInCombat.OnEnterCombatEnter();

                    //Audio
                    //AudioLibrary["Uke1"].volume = 1;

                    //AudioLibrary["Bass3"].volume = 0;
                    //AudioLibrary["Synth3"].volume = 0;
                    //AudioLibrary["Uke3"].volume = 0;

                }
                 
                break;

            case State.PreAction:

                //Handle Time Offset here (shouldnt be offset)
                if (currentBeatIndex != 1)
                {
                    return;
                }

                //wait 4 seconds
                if (TimerCourtine == null)
                {

                    FunctionToDo += PlayerManager.Player.OnPreActionExit;
                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.Action));

                    //On Enter and Exit      
                    PlayerManager.Player.OnPreActionEnter();
                    enemyInCombat.OnPreActionEnter();


                    //Audio


                }

                break;

            case State.Action:

                //Handle Time Offset here (shouldnt be offset)
                if (currentBeatIndex != 1)
                {
                    return;
                }

                //wait 4 seconds
                if (TimerCourtine == null)
                {
                    TimerCourtine = StartCoroutine(TimerCour(4.0f, true));

                    //On Enter
                    
                    PlayerManager.Player.OnActionEnter();

                    //Audio
                    //SynthAudioSource.Play();
                    //UkuleleAudioSource.Play();
                    //BassAudioSource.Play();


                    //Player or Boss will decide, BUT FOR NOW, go to Pre
                }


                break;
            case State.PostKill:

                if (currentBeatIndex != 1)
                {
                    return;
                }

                //wait 4 seconds
                if (TimerCourtine == null)
                {
                    FunctionToDo += PlayerManager.Player.OnPostKillExit;

                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.Walking));

                    //On Enter
                    PlayerManager.Player.OnPostKillEnter();


                    //Audio Victory
                    //SynthAudioSource.Play();
                    //UkuleleAudioSource.Play();
                    //BassAudioSource.Play();


                    //Player or Boss will decide, BUT FOR NOW, go to Pre
                }

                break;
            case State.Death:

                GameOverScreen.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
                {
                    SceneManager.LoadScene("BuildScene");
                }

                break;

        }

    }

    private void HeartBeatUpdate()
    {
        currentBeatIndex++;

        if (audioSetUp == false)
        {
            beatAnim.SetBool("count" , true);

            audioSetUp = true;

            foreach (AudioSource kyak in AudioSourceList)
            {
                //kyak.Play();
                kyak.volume = 0;
            }
        }


        if (currentBeatIndex > 8)
        {
            currentBeatIndex = 1;
        }

    }

    private IEnumerator TimerCour(float time, State nextState)
    {
        yield return new WaitForSeconds(time);

        currentState = nextState;

        FunctionToDo.Invoke();
        FunctionToDo = null;

        TimerCourtine = null;

    }

    private IEnumerator TimerCour(float time)
    {
        yield return new WaitForSeconds(time);
        TimerCourtine = null;

    }
    
    private IEnumerator TimerCour(float time, bool ActionCheck)
    {
        yield return new WaitForSeconds(time);

        if (enemyInCombat.currentHP <= 0)
        {
            //Enemy killed
            //play enemy animation

            PlayerManager.Player.OnActionExit();
            PlayerManager.Player.OnPostKillEnter();

            AllEnemies.Remove(enemyInCombat);

            //Spawn Kill Items
            foreach (ItemManager IM in PlayerManager.Player.ItemSpawnData)
            {
                if (IM != null)
                { 
                    Destroy(IM.gameObject);
                }
            }
            PlayerManager.Player.ItemSpawnData.Clear();


            PlayerManager.Player.ItemSpawnData.Add(Instantiate(enemyInCombat.EnemyStats.OnDeathDrops[(Random.Range(0, enemyInCombat.EnemyStats.OnDeathDrops.Count))].Prefab,PlayerManager.Player.ItemSpawnPos[0].transform).GetComponent<ItemManager>());
            PlayerManager.Player.ItemSpawnData.Add(Instantiate(enemyInCombat.EnemyStats.OnDeathDrops[(Random.Range(0, enemyInCombat.EnemyStats.OnDeathDrops.Count))].Prefab,PlayerManager.Player.ItemSpawnPos[1].transform).GetComponent<ItemManager>());



            currentState = State.PostKill;
            enemyInCombat = null;
        }
        //else if player dead
        else
        {
            currentState = State.PreAction;
            PlayerManager.Player.OnActionExit();

            //drop items
            foreach (ItemManager IM in PlayerManager.Player.ItemSpawnData)
            {
                if (IM != null)
                { 
                    Destroy(IM.gameObject);
                }
            }
            PlayerManager.Player.ItemSpawnData.Clear();

            PlayerManager.Player.ItemSpawnData.Add(Instantiate(enemyInCombat.EnemyStats.OnTurnDrops[(Random.Range(0, enemyInCombat.EnemyStats.OnTurnDrops.Count))].Prefab, PlayerManager.Player.ItemSpawnPos[0].transform).GetComponent<ItemManager>());
            PlayerManager.Player.ItemSpawnData.Add(Instantiate(enemyInCombat.EnemyStats.OnTurnDrops[(Random.Range(0, enemyInCombat.EnemyStats.OnTurnDrops.Count))].Prefab, PlayerManager.Player.ItemSpawnPos[1].transform).GetComponent<ItemManager>());


        }



        TimerCourtine = null;

    }


    //Reaason its on world machine is cuz if not all enemies will attack, regardless of agro
    public void EnemyAttacksPlayerEvent()
    {
        if (PlayerManager.Player.currentAction == PlayerManager.Action.Blocking)
        {
            if (enemyInCombat.EnemyStats.Attack - PlayerManager.Player.currentItem.Stat > 0)
            {
                //do nothing
            }
            else
            {
                PlayerManager.Player.health -= enemyInCombat.EnemyStats.Attack - PlayerManager.Player.currentItem.Stat;

                //Particle burst for hit
            }
        }
        else 
        {
            PlayerManager.Player.health -= enemyInCombat.EnemyStats.Attack;
        }


    }



    private IEnumerator AudioStarter()
    {

        AudioLibrary["IntroLoop"].volume = 0.5f;

        while (AudioLibrary["IntroLoop"].isPlaying == true)
        {
            yield return new WaitForEndOfFrame();
        }

        AudioLibrary["IntroLoop"].volume = 0.0f;
        AudioLibrary["DungeonLoop"].volume = 0.5f;

        while (PlayerManager.Player.gameObject.transform.position.x < 32)
        {
            yield return new WaitForEndOfFrame();
        }

        

    }
}
