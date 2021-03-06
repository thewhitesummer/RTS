﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour {
    
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    [ServerCallback]
    private void Update() {
        Targetable target = targeter.GetTarget();
        if(target != null) {
            if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) {
                agent.SetDestination(target.transform.position);
                return;
            }
            if(!agent.hasPath) { return; }
        }
        else {
            if(!agent.hasPath) { return; }
            if(agent.remainingDistance > agent.stoppingDistance) { return; }
        }
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 newPos) {
        targeter.ClearTarget();

        if(!NavMesh.SamplePosition(newPos, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    #endregion

}
