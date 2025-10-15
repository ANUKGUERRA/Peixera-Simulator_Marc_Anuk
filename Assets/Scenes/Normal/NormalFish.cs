using UnityEngine;

public class NormalFish : Fish
{
    [Header("Flee Settings")]
    public float safeDistance = 8f;
    public float fleeDuration = 3f;
    private float fleeTimer = 0f;

    [Header("Visual Feedback")]
    public Renderer fishRenderer;
    public Color normalColor = Color.blue;
    public Color fleeColor = Color.red;

    private void Start()
    {
        base.Start();

        // Set initial color
        if (fishRenderer != null)
        {
            fishRenderer.material.color = normalColor;
        }
    }

    private void Update()
    {
        base.Update();

        if (!isFleeing)
        {
            CheckForPredators();

            if (!isFleeing)
                Wander();
        }
        else
        {
            Flee();
            UpdateFleeTimer();
        }
    }

    private void CheckForPredators()
    {
        Collider[] nearbyFish = GetNearbyFish();

        foreach (Collider col in nearbyFish)
        {
            PredatorFish predator = col.GetComponent<PredatorFish>();
            if (predator != null)
            {
                Debug.Log("Predator detected");
                FleeFromPredator(predator.transform.position);
                return;
            }
        }
    }

    private void FleeFromPredator(Vector3 predatorPosition)
    {
        isFleeing = true;
        fleeTimer = fleeDuration;


        Vector3 fleeDirection = (transform.position - predatorPosition).normalized;
        wanderTarget = transform.position + fleeDirection * safeDistance;

        if (tank != null)
        {
            wanderTarget = tank.ClampPosition(wanderTarget);
        }
    }

    private void Flee()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 3 * Time.deltaTime); 

        rb.linearVelocity = transform.forward * fleeSpeed;

        if (Vector3.Distance(transform.position, wanderTarget) < 1f || fleeTimer <= 0f)
        {
            StopFleeing();
        }
    }

    private void UpdateFleeTimer()
    {
        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0f)
        {
            StopFleeing();
        }
    }

    private void StopFleeing()
    {
        isFleeing = false;

        if (fishRenderer != null)
        {
            fishRenderer.material.color = normalColor;
        }

        GenerateWanderTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PredatorFish predator = collision.gameObject.GetComponent<PredatorFish>();
        if (predator != null)
        {
            Die();
        }
    }
}