using UnityEngine;
using UnityEngine.AI;

public enum AIState { RunState, HideState, DeathState }

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform endPoint;

    [SerializeField] private AIState currentState = AIState.RunState;
    [SerializeField] private Color runStateColor = new Color(0.9f, 1f, 0.5f);
    [SerializeField] private Color hideStateColor = Color.blue;
    [SerializeField] private Color deathStateColor = Color.red;

    private Renderer meshRenderer;
    private Material material;
    private float hideTimer = 0f;
    [SerializeField] private float hideRandomMin = 1f;
    [SerializeField] private float hideRandomMax = 3f;
    private float hideRandomDuration = 0f;
    private float deathTimer = 0f;

    [SerializeField] private float hideStopDistance = 0.5f;
    private Transform selectedHideSpot = null;
    private int hideCount = 0;
    [SerializeField] private int maxHides = 2;
    [SerializeField] private int pointsOnDeath = 50; // CHANGED: Death points

    private float cooldownTimer = 0f;
    [SerializeField] private float cooldownMin = 5f;
    [SerializeField] private float cooldownMax = 7f;
    private float cooldownDuration = 0f;
    private bool inCooldown = false;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        if (endPoint == null)
            endPoint = GameObject.Find("EndPoint").transform;

        meshRenderer = GetComponent<Renderer>();
        material = meshRenderer.material;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        currentState = AIState.RunState;
        agent.enabled = true;

        hideCount = 0;
        selectedHideSpot = null;
        hideTimer = 0f;
        deathTimer = 0f;
        cooldownTimer = 0f;
        inCooldown = false;
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.RunState:
                HandleRunState();
                break;
            case AIState.HideState:
                HandleHideState();
                break;
            case AIState.DeathState:
                HandleDeathState();
                break;
        }

        UpdateColorBySpeed();
    }

    void HandleRunState()
    {
        if (inCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                cooldownTimer = 0f;
                inCooldown = false;
            }
        }

        if (hideCount < maxHides && !inCooldown)
        {
            Transform nextHideSpot = HideSpotManager.Instance.FindNearestEmptySpot(transform.position);
            if (nextHideSpot != null)
            {
                selectedHideSpot = nextHideSpot;
                hideRandomDuration = Random.Range(hideRandomMin, hideRandomMax);
                SetState(AIState.HideState);
                return;
            }
        }

        if (!agent.hasPath)
        {
            agent.SetDestination(endPoint.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint") || other.name.Contains("EndPoint"))
        {
            SpawnManager.Instance.ReturnEnemyToPool(gameObject);
        }
    }

    void HandleHideState()
    {
        if (selectedHideSpot != null)
        {
            agent.SetDestination(selectedHideSpot.position);

            if (!agent.pathPending && agent.remainingDistance < hideStopDistance)
            {
                agent.isStopped = true;
                agent.ResetPath();

                hideTimer += Time.deltaTime;

                if (hideTimer >= hideRandomDuration)
                {
                    hideTimer = 0f;
                    hideCount++;
                    HideSpotManager.Instance.ReleaseHideSpot(selectedHideSpot);

                    if (hideCount < maxHides)
                    {
                        inCooldown = true;
                        cooldownDuration = Random.Range(cooldownMin, cooldownMax);
                    }

                    agent.isStopped = false;
                    agent.SetDestination(endPoint.position);

                    SetState(AIState.RunState);
                }
            }
        }
    }

    void HandleDeathState()
    {
        agent.enabled = false;
        deathTimer += Time.deltaTime;

        // CHANGED: Award points immediately
        if (deathTimer < 0.1f)
        {
            GameManager.Instance.AddScore(pointsOnDeath);
            // TODO: Trigger death animation here
            Debug.Log($"Enemy died! +{pointsOnDeath} points");
        }

        if (deathTimer >= 3f)
        {
            SpawnManager.Instance.ReturnEnemyToPool(gameObject);
            deathTimer = 0f;
            agent.enabled = true;
        }
    }

    // CHANGED: Color based on speed
    void UpdateColorBySpeed()
    {
        if (currentState == AIState.DeathState)
        {
            material.color = deathStateColor; // Red
            return;
        }

        if (currentState == AIState.HideState && agent.isStopped)
        {
            material.color = hideStateColor; // Blue when stopped hiding
            return;
        }

        // All other times = yellowish
        material.color = runStateColor;
    }

    public void TakeDamage()
    {
        if (currentState != AIState.DeathState)
        {
            SetState(AIState.DeathState);
        }
    }

    public void SetState(AIState newState)
    {
        currentState = newState;
        Debug.Log($"Enemy changed to: {currentState}, HideCount: {hideCount}", gameObject);
    }

    public AIState GetCurrentState() => currentState;
}