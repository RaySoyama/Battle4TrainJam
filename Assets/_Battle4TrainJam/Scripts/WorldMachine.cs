using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMachine : MonoBehaviour
{
    public static WorldMachine World;

    [SerializeField]
    private AudioSource HeatbeatAudioSource = null;

    [SerializeField]
    private float BeatsPerMinute = 120;

    [SerializeField][ReadOnlyField]
    private float BeatTime;

    public int currentBeatIndex;




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
        
    }

    private void HeartBeatUpdate()
    {
        BeatTime += Time.deltaTime;
        if (BeatTime >= beatDuration)
        {
            BeatTime -= beatDuration;
            Debug.Log("BEAT");

            currentBeatIndex++;
            HeatbeatAudioSource.Play();

            if (currentBeatIndex > 4)
            {
                currentBeatIndex = 1;
            }
        }
    }

}
