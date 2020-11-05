using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy_Gather : MeleeEnemy_BaseFSM
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 lookpos = enemyController.objective;
        Vector3 fixedPos = new Vector3(lookpos.x, enemy.transform.position.y, lookpos.z); // fix y pos
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(fixedPos - enemy.transform.position), Time.deltaTime * 6f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("canGather", false);
    }
}
