using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestPathFinding : MonoBehaviour
{
    public NavMeshAgent _navAgent;
    public Transform[] destPos;
    int currentPoint = 0;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        //enable agent updates
        _navAgent.updateRotation = true;

        _navAgent.SetDestination(destPos[currentPoint].position);
        yield return StartCoroutine(WaitForDestination());

        StartCoroutine(NextWaypoint());
    }

    IEnumerator WaitForDestination()
    {
        yield return new WaitForEndOfFrame();
        while (_navAgent.pathPending)
            yield return null;
        yield return new WaitForEndOfFrame();

        float remain = _navAgent.remainingDistance;
        while (remain == Mathf.Infinity || remain - _navAgent.stoppingDistance > float.Epsilon
        || _navAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            remain = _navAgent.remainingDistance;
            yield return null;
        }

        Debug.LogFormat("--- PathComplete to pos:{0}", currentPoint);
    }

    IEnumerator NextWaypoint()
    {
        currentPoint++;
        currentPoint = currentPoint % destPos.Length;
        Transform next = destPos[currentPoint];
        _navAgent.SetDestination(next.position);
        yield return StartCoroutine(WaitForDestination());

        StartCoroutine(NextWaypoint());
    }

}
