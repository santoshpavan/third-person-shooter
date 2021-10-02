using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EmuController : MonoBehaviour {

    private enum State {
        IDLE,
        CHASE,
        ATTACK,
        DEAD
    }

    [SerializeField] private float chaseDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;
    // [SerializeField] private float range_deg = 0f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private int health;

    private NavMeshAgent navMeshAgent;
    private State emuState;
    private Animator animator;

    private string runState = "Run";
    private string shootBulletMethod = "shootBullet";
    private string bulletTag = "Bullet";
    private bool isShooting = false;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        emuState = State.IDLE;
        chaseDistanceThreshold = 15.0f;
        attackDistanceThreshold = 10.0f;
        health = 100;
    }

    private void Update() {
        Debug.Log(health);
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
            case State.DEAD:
                emuDeadState();
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

        if (health <= 0) {
            emuState = State.DEAD;
        }
    }

    private void emuIdleState() {
        navMeshAgent.isStopped = true;
        animator.SetBool(runState, false);
    }

    private void emuChaseState() {
        isShooting = false;
        CancelInvoke(shootBulletMethod);
        animator.SetBool(runState, true);
        Quaternion lookOnLook = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime);
        if (navMeshAgent.isStopped) {
            navMeshAgent.isStopped = false;
        }
    }

    private void emuAttackState() {
        animator.SetBool(runState, false);
        navMeshAgent.isStopped = true;
        if (!isShooting) {
            isShooting = true;
            InvokeRepeating(shootBulletMethod, 0.0f, 1.0f);
        }
    }

    private void emuDeadState() {
        animator.SetBool(runState, false);
        navMeshAgent.isStopped = true;
        Debug.Log("delete");
        Destroy(this);
    }

    private void shootBullet() {
        Vector3 aimDirection = (playerTransform.position - bulletSpawnPosition.position).normalized;
        Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(bulletTag)) {
            health -= 10;
        }
    }

}
