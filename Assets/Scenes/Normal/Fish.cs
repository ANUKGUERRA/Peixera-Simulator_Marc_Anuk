using UnityEngine;
using System.Collections;

public abstract class Fish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float fleeSpeed = 6f; // Increased flee speed
    public float rotationSpeed = 2f;
    public float wanderRadius = 3f;
    public float wanderDistance = 5f;
    public float wanderJitter = 1f;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask detectionLayer;

    protected Vector3 wanderTarget;
    protected Rigidbody rb;
    protected FishTank tank;
    protected bool isFleeing = false;
    protected bool isChasing = false;

    public bool IsFleeing { get { return isFleeing; } }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        tank = FindAnyObjectByType<FishTank>();
        GenerateWanderTarget();
    }

    protected virtual void Update()
    {
        AvoidWalls();
    }

    protected void GenerateWanderTarget()
    {
        wanderTarget += new Vector3(
            Random.Range(-1f, 1f) * wanderJitter,
            Random.Range(-1f, 1f) * wanderJitter,
            Random.Range(-1f, 1f) * wanderJitter
        );

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        wanderTarget += Vector3.forward * wanderDistance;
        wanderTarget = transform.TransformPoint(wanderTarget);
    }

    protected void Wander()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.linearVelocity = transform.forward * wanderSpeed;

        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            GenerateWanderTarget();
        }
    }

    protected void AvoidWalls()
    {
        if (tank == null) return;

        Vector3 tankSize = tank.GetTankSize();
        Vector3 currentPos = transform.position;

        // Calculate safe distance from walls
        float safeDistance = 1f;
        Vector3 avoidanceForce = Vector3.zero;

        // Check each wall and add avoidance force
        if (currentPos.x < -tankSize.x / 2 + safeDistance)
            avoidanceForce.x = 1f;
        else if (currentPos.x > tankSize.x / 2 - safeDistance)
            avoidanceForce.x = -1f;

        if (currentPos.y < -tankSize.y / 2 + safeDistance)
            avoidanceForce.y = 1f;
        else if (currentPos.y > tankSize.y / 2 - safeDistance)
            avoidanceForce.y = -1f;

        if (currentPos.z < -tankSize.z / 2 + safeDistance)
            avoidanceForce.z = 1f;
        else if (currentPos.z > tankSize.z / 2 - safeDistance)
            avoidanceForce.z = -1f;

        if (avoidanceForce != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(avoidanceForce.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);
            GenerateWanderTarget();
        }
    }

    protected Collider[] GetNearbyFish()
    {
        return Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
    }

    // Virtual method to handle death
    public virtual void Die()
    {
        if (tank != null)
        {
            tank.RemoveFishFromList(gameObject);
        }
        Destroy(gameObject);
    }
}