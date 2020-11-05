using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy_Stunned : MeleeEnemy_BaseFSM
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        agent.isStopped = true;
    }
}
