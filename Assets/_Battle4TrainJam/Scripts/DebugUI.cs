using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public Text beat;
    public Text state;
    void Update()
    {
        beat.text = $"Beat {WorldMachine.World.currentBeatIndex}";
        state.text = $"State {WorldMachine.World.currentState.ToString()}";

    }
}
