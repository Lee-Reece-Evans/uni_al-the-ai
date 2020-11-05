using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy_Attack : RangedEnemy_BaseFSM
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyController != null && enemyController.target != enemyController.targetResource)
        {
            // rotate body towards objective
            Vector3 lookpos = enemyController.objective + Vector3.up * 1;
            Vector3 fixedPos = new Vector3(lookpos.x, enemy.transform.position.y, lookpos.z); // fix y pos
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(fixedPos - enemy.transform.position), Time.deltaTime * 6f);
            enemyController.muzzlePos.LookAt(lookpos); // temporary fix for gun accuracy
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("canShoot", false);
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // rotate enemy torso towards target 
        animator.SetLookAtWeight(1.0f, 0.25f, 0.9f, 1.0f, 1.0f);
        animator.SetLookAtPosition(enemyController.objective);
    }
}
