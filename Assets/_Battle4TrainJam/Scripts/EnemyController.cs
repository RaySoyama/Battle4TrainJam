using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public Animator anim;
    [SerializeField]
    public Animator outlineAnim;



    [SerializeField]
    private EnemySO enemyStats;

    public EnemySO EnemyStats
    {
        get
        {
            return enemyStats;
        }
    }


    [ReadOnlyField]
    public int currentHP;

    [SerializeField][ReadOnlyField]
    private int attackPatternIndex = 0;


    public ParticleSystem particle;

    void Start()
    {
        currentHP = enemyStats.Health;
    }


    void Update()
    {
        
    }


    public void OnEnterCombatEnter()
    {
        anim.SetTrigger("taunt");
        outlineAnim.SetTrigger("taunt");
    }
    public void OnPreActionEnter()
    {
        if (enemyStats.AttackPattern[attackPatternIndex] == EnemySO.Action.Attack)
        {
            anim.SetTrigger("attack");
            outlineAnim.SetTrigger("attack");
        }
        else if (enemyStats.AttackPattern[attackPatternIndex] == EnemySO.Action.Vulnerable)
        {
            anim.SetTrigger("vulnerable");
            outlineAnim.SetTrigger("vulnerable");

        }

        attackPatternIndex++;

        if (attackPatternIndex >= enemyStats.AttackPattern.Count)
        {
            attackPatternIndex = 0;
        }
    
    }


    public void OnActionExit()
    {
        //Spawn Items



    }


}
