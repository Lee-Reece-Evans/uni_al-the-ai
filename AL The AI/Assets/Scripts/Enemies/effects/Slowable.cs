using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slowable : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    private int insideAnotherSlow = 0;
    private float origMovementSpeed;
    private float origAnimSpeed;

    private bool initialised = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        origMovementSpeed = agent.speed;
        origAnimSpeed = anim.speed;

        initialised = true;
    }

    private void OnDisable()
    {
        if (!initialised)
            return;

        insideAnotherSlow = 0;
        agent.speed = origMovementSpeed;
        anim.speed = origAnimSpeed;
    }

    public void Slow()
    {
        if (insideAnotherSlow == 0) // if i'm not already inside a slow trigger.
        {
            // need to slow animations also
            anim.speed = anim.speed * 0.75f;
            agent.speed = agent.speed * 0.75f;
            insideAnotherSlow++; // to know we are inside one
        }
        else // entered another slow tower's range within this ones range.
        {
            insideAnotherSlow++; // keep track of how many others we enter
        }
    }

    public void UndoSlow()
    {
        insideAnotherSlow--; // left a slow trigger, decrease the amount we are inside

        if (insideAnotherSlow == 0) // exited all triggers, return speed.
        {
            agent.speed = origMovementSpeed;
            anim.speed = origAnimSpeed;
        }
    }
}
