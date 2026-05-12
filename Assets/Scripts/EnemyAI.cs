using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform endPoint;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        if (endPoint == null)
            endPoint = GameObject.Find("EndPoint").transform;
        agent.SetDestination(endPoint.position);
    }

    void Update()
    {
        // CHANGED: Removed velocity check
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SpawnManager.Instance.ReturnEnemyToPool(gameObject);
        }
    }
}