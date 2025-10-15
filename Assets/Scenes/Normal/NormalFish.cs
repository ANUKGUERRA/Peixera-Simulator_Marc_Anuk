using UnityEngine;

public class NormalFish : Fish
{
    [Header("Flee Settings")]
    public float safeDistance = 8f;

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
                FleeFromPredator(predator.transform.position);
                return;
            }
        }

        // If no predators nearby and we're far enough from the last predator, stop fleeing
        if (isFleeing)
        {
            isFleeing = false;
        }
    }

    private void FleeFromPredator(Vector3 predatorPosition)
    {
        isFleeing = true;
        Vector3 fleeDirection = (transform.position - predatorPosition).normalized;
        wanderTarget = transform.position + fleeDirection * safeDistance;

        // Clamp wander target within tank bounds
        if (tank != null)
        {
            wanderTarget = tank.ClampPosition(wanderTarget);
        }
    }

    private void Flee()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);

        rb.linearVelocity = transform.forward * fleeSpeed;

        // Check if we've reached safe distance
        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            isFleeing = false;
            GenerateWanderTarget();
        }
    }
}