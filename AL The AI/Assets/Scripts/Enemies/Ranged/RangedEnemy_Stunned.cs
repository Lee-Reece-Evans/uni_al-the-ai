using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy_Stunned : RangedEnemy_BaseFSM
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        agent.isStopped = true;
    }
}
