using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    void Start()
    {
        transform.position = startPoint.position;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(endPoint.position);
    }

    void Update()
    {
        // Check if reached end point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            gameObject.SetActive(false); // Disable for now
        }
    }
}