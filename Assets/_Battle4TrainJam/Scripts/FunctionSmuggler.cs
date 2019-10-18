using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionSmuggler : MonoBehaviour
{
    public void PlayerAttacks()
    {
        PlayerManager.Player.DoAttackDamageCamdenThisIsYou();
    }


    public void EnemyAttacks()
    {
        WorldMachine.World.EnemyAttacksPlayerEvent();
    }
}
