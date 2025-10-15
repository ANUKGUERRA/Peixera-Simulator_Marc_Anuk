using UnityEngine;

public class PredatorFish : Fish
{
    [Header("Predator Settings")]
    public float attackRange = 1.5f;
    public float catchCooldown = 2f;
    private float lastCatchTime = 0f;

    private NormalFish currentTarget;

    protected override void Start()
    {
        base.Start();
        lastCatchTime = -catchCooldown; // Allow immediate attack
    }

    protected override void Update()
    {
        base.Update();

        if (!isChasing)
        {
            FindPrey();

            if (!isChasing)
                Wander();
        }
        else
        {
            Chase();
            CheckAttackRange();
        }
    }

    private void FindPrey()
    {
        Collider[] nearbyFish = GetNearbyFish();
        float closestDistance = float.MaxValue;
        NormalFish closestPrey = null;

        foreach (Collider col in nearbyFish)
        {
            NormalFish prey = col.GetComponent<NormalFish>();
            if (prey != null && !prey.IsFleeing) // Prefer non-fleeing fish
            {
                float distance = Vector3.Distance(transform.position, prey.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPrey = prey;
                }
            }
        }

        // If no non-fleeing fish, target any fish
        if (closestPrey == null)
        {
            foreach (Collider col in nearbyFish)
            {
                NormalFish prey = col.GetComponent<NormalFish>();
                if (prey != null)
                {
                    float distance = Vector3.Distance(transform.position, prey.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPrey = prey;
                    }
                }
            }
        }

        if (closestPrey != null)
        {
            currentTarget = closestPrey;
            isChasing = true;
        }
    }

    private void Chase()
    {
        if (currentTarget == null)
        {
            isChasing = false;
            return;
        }

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);

        rb.linearVelocity = transform.forward * chaseSpeed;

        // Check if target is out of range
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > detectionRadius * 1.5f)
        {
            isChasing = false;
            currentTarget = null;
        }
    }

    private void CheckAttackRange()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget <= attackRange && Time.time >= lastCatchTime + catchCooldown)
        {
            // Try to catch the fish
            CatchFish();
        }
    }

    private void CatchFish()
    {
        if (currentTarget != null)
        {
            currentTarget.Die();
            lastCatchTime = Time.time;
            isChasing = false;
            currentTarget = null;

            // Take a short break after catching
            GenerateWanderTarget();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        NormalFish normalFish = collision.gameObject.GetComponent<NormalFish>();
        if (normalFish != null && Time.time >= lastCatchTime + catchCooldown)
        {
            // Direct collision catch
            normalFish.Die();
            lastCatchTime = Time.time;
            isChasing = false;
            currentTarget = null;
        }
    }
}