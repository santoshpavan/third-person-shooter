using UnityEngine;
using UnityEngine.AI;

public class EmuController : MonoBehaviour {

    private enum State {
        IDLE,
        CHASE,
        ATTACK
    }

    [SerializeField] private float chaseDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;

    [SerializeField] private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private State emuState;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        emuState = State.IDLE;
        chaseDistanceThreshold = 15.0f;
        attackDistanceThreshold = 10.0f;
    }

    private void Update() {
        emuUpdateState();

        switch(emuState) {
            default:
            case State.IDLE:
                emuIdleState();
                break;
            case State.CHASE:
                emuChaseState();
                break;
            case State.ATTACK:
                emuAttackState();
                break;
        }
    }

    private void emuUpdateState() {
        navMeshAgent.destination = playerTransform.position;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > chaseDistanceThreshold) {
            emuState = State.IDLE;
        }
        else if (distance <= chaseDistanceThreshold && distance > attackDistanceThreshold) {
            emuState = State.CHASE;
        }
        else {
            emuState = State.ATTACK;
        }
    }

    private void emuIdleState() {
        transform.GetComponent<Renderer>().material.color = Color.blue;
        navMeshAgent.isStopped = true;
    }

    private void emuChaseState() {
        transform.GetComponent<Renderer>().material.color = Color.green;
        if (navMeshAgent.isStopped) {
            navMeshAgent.isStopped = false;
        }
    }

    private void emuAttackState() {
        transform.GetComponent<Renderer>().material.color = Color.red;
    }

}
