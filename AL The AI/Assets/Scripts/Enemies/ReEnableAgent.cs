using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReEnableAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public void SetAgent(NavMeshAgent _agent)
    {
        agent = _agent;
        StartCoroutine(EnableAgent());
    }
    IEnumerator EnableAgent()
    {
        // wait 2 frames for navmesh to recalculate after obstable carving disabled.
        yield return null;
        yield return null;

        agent.enabled = true;
        agent.isStopped = false;
    }
}
