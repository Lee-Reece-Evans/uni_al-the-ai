using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy_BaseFSM : StateMachineBehaviour
{
    public GameObject enemy;
    public NavMeshAgent agent;
    public EnemyMelee enemyController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        enemyController = enemy.GetComponent<EnemyMelee>();
    }
}
