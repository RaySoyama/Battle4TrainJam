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
        MusicUpdate();


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


        if(Input.GetKeyDown(KeyCode.U))
        {
            currentState = State.Walking;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentState = State.PreAction;
        }


    }

    private void HeartBeatUpdate()
    {
        BeatTime += Time.deltaTime;
        if (BeatTime >= beatDuration)
        {
            BeatTime -= beatDuration;
            Debug.Log("BEAT");

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
                if (currentBeatIndex == 1 && UkuleleAudioSource.isPlaying == false)
                {
                    UkuleleAudioSource.Play();
                }

                break;
            case State.PreAction:
                if (currentBeatIndex == 1 && (UkuleleAudioSource.isPlaying == false || BassAudioSource.isPlaying == false))
                {
                    UkuleleAudioSource.Play();
                    BassAudioSource.Play();
                }
                break;

        }

    }


}
