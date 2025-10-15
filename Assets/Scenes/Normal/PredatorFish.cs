using UnityEngine;

public class PredatorFish : Fish
{
    private NormalFish currentTarget;

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
}