using UnityEngine;
using System.Collections;

public abstract class Fish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float fleeSpeed = 6f;
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

    private void FixedUpdate()
    {
        // Clamp position every physics update to ensure fish never escape
        if (tank != null)
        {
            transform.position = tank.ClampPosition(transform.position);
        }
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

        // Clamp wander target to stay within tank
        if (tank != null)
        {
            wanderTarget = tank.ClampPosition(wanderTarget);
        }
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

        // Increased safe distance for earlier avoidance
        float safeDistance = 2f;
        Vector3 avoidanceForce = Vector3.zero;
        bool needsAvoidance = false;

        // Check each wall and add avoidance force
        if (currentPos.x < -tankSize.x / 2 + safeDistance)
        {
            avoidanceForce.x = 1f;
            needsAvoidance = true;
        }
        else if (currentPos.x > tankSize.x / 2 - safeDistance)
        {
            avoidanceForce.x = -1f;
            needsAvoidance = true;
        }

        if (currentPos.y < -tankSize.y / 2 + safeDistance)
        {
            avoidanceForce.y = 1f;
            needsAvoidance = true;
        }
        else if (currentPos.y > tankSize.y / 2 - safeDistance)
        {
            avoidanceForce.y = -1f;
            needsAvoidance = true;
        }

        if (currentPos.z < -tankSize.z / 2 + safeDistance)
        {
            avoidanceForce.z = 1f;
            needsAvoidance = true;
        }
        else if (currentPos.z > tankSize.z / 2 - safeDistance)
        {
            avoidanceForce.z = -1f;
            needsAvoidance = true;
        }

        if (needsAvoidance)
        {
            // Apply stronger rotation when near walls
            Quaternion targetRotation = Quaternion.LookRotation(avoidanceForce.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 4 * Time.deltaTime);

            // Reduce velocity when near walls
            rb.linearVelocity *= 0.5f;

            // Generate new wander target away from wall
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