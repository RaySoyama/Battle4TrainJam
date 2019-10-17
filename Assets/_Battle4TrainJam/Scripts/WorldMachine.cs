using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WorldMachine : MonoBehaviour
{
    public enum State
    { 
        None,
        Walking,
        EnterCombat,
        PreAction,
        Action,
        Win,
        Death
    }


    public static WorldMachine World;


    [SerializeField]
    private AudioSource HeatbeatAudioSource = null;

    [SerializeField]
    private AudioSource BassAudioSource = null;

    [SerializeField]
    private AudioSource UkuleleAudioSource = null;

    [SerializeField]
    private AudioSource SynthAudioSource = null;

    [SerializeField]
    private AudioSource ActionAudioSource = null;


    [SerializeField]
    private AudioClip WalkingUke;
    [SerializeField]
    private AudioClip WalkingSynth;
    [SerializeField]
    private AudioClip WalkingBass;
    [SerializeField]
    private AudioClip EnemyAttackingSynth;
    [SerializeField]
    private AudioClip EnemyAttackingBass;
    [SerializeField]
    private AudioClip CombatNoActionSynth;
    [SerializeField]
    private AudioClip CombatNoActionUke;
    [SerializeField]
    private AudioClip CombatNoActionBass;
    


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

    [SerializeField][ReadOnlyField]
    private float BeatTime;

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

    private Coroutine TimerCourtine = null;

    private delegate void MyDelegate();
    private MyDelegate FunctionToDo;



    private bool cycleCheck = false;
    private float beatDuration;
    void Start()
    {
        if (World == null)
        {
            World = this;    
        }

        currentBeatIndex = 0;
        BeatTime = 0;
        beatDuration = 60.0f / BeatsPerMinute;
    }

    void Update()
    {

        HeartBeatUpdate();

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


        if (Input.GetKeyDown(KeyCode.U))
        {
            currentState = State.Walking;
        }



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
                    UkuleleAudioSource.Play();
                    UkuleleAudioSource.loop = true;
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

                    //Audio
                    UkuleleAudioSource.Play();
                    BassAudioSource.Play();
                    UkuleleAudioSource.loop = false;
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
                    
                    //Audio
                    SynthAudioSource.Play();
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


                    FunctionToDo += PlayerManager.Player.OnActionExit;
                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.PreAction));

                    //On Enter
                    
                    PlayerManager.Player.OnActionEnter();

                    //Audio
                    SynthAudioSource.Play();
                    UkuleleAudioSource.Play();
                    BassAudioSource.Play();


                    //Player or Boss will decide, BUT FOR NOW, go to Pre
                }


                break;
            case State.Win:
                break;
            case State.Death:
                break;

        }

    }

    private void HeartBeatUpdate()
    {
        BeatTime += Time.deltaTime;
        if (BeatTime >= beatDuration)
        {
            BeatTime -= beatDuration;

            currentBeatIndex++;
            //HeatbeatAudioSource.Play();

            if (currentBeatIndex > 8)
            {
                currentBeatIndex = 1;
            }
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



}
