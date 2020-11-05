using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy_BaseFSM : StateMachineBehaviour
{
    public GameObject enemy;
    public NavMeshAgent agent;
    public EnemyRanged enemyController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        enemyController = enemy.GetComponent<EnemyRanged>();
    }
}
