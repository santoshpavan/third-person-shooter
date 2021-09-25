
using UnityEngine;
using UnityEngine.AI;

public class EmuAIController : MonoBehaviour {
    [SerializeField] private Transform playerTransform;
    private NavMeshAgent navMeshAgent;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        navMeshAgent.destination = playerTransform.position;
    }

}
