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
            PlayerManager.Player.OnWalkingEnter();
        }



        switch (currentState)
        {
            case State.None:
                break;
            case State.Walking:
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
                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.PreAction));
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
                    TimerCourtine = StartCoroutine(TimerCour(4.0f, State.Action));

                    //On Enter and Exit
                    PlayerManager.Player.OnEnterCombatExit();
                    PlayerManager.Player.OnPreActionEnter();
                    

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
                    TimerCourtine = StartCoroutine(TimerCour(4.0f));

                    //On Enter and Exit
                    PlayerManager.Player.OnPreActionExit();
                    PlayerManager.Player.OnActionEnter();

                    
                    //Player or Boss wil
                }


                break;
            case State.Win:
                break;
            case State.Death:
                break;




        }


        MusicUpdate();

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

    private void MusicUpdate()
    {
        switch (currentState)
        {
            case State.Walking:
                if (currentBeatIndex == 1 && UkuleleAudioSource.isPlaying != true)
                {
                    UkuleleAudioSource.Play();
                }

                break;

            case State.EnterCombat:

                if (currentBeatIndex == 1 && (BassAudioSource.isPlaying == false))
                {
                    UkuleleAudioSource.Play();
                    BassAudioSource.Play();
                }

                break;
            case State.PreAction:


                //Depends on whats happening, enemy chill? dead? attacking?
                //if (currentBeatIndex == 1 && (SynthAudioSource.isPlaying != true))
                //{
                //    SynthAudioSource.Play();
                //}

                break;
            case State.Action:

                if (currentBeatIndex == 1 && (BassAudioSource.isPlaying != true))
                {
                    SynthAudioSource.Play();
                    UkuleleAudioSource.Play();
                    BassAudioSource.Play();
                }

                break;
        }

    }

    private IEnumerator TimerCour(float time, State nextState)
    {
        yield return new WaitForSeconds(time);

        currentState = nextState;
        TimerCourtine = null;

    }

      private IEnumerator TimerCour(float time)
    {
        yield return new WaitForSeconds(time);
        TimerCourtine = null;

    }



}
