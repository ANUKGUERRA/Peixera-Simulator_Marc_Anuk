using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{

    #region Singleton
    static BoidManager steeringObstacleManager;
    public static BoidManager instance
    {
        get
        {
            return RequestSteeringObstacleManager();
        }
    }

    private static BoidManager RequestSteeringObstacleManager()
    {
        if (!steeringObstacleManager)
        {
            GameObject steeringObstacleManagerObj = new GameObject("BoidManager");
            steeringObstacleManager = steeringObstacleManagerObj.AddComponent<BoidManager>();
            steeringObstacleManager.boids = new List<Boid>();
        }
        return steeringObstacleManager;
    }

    private void Awake()
    {
        if (steeringObstacleManager == null)
        {
            steeringObstacleManager = this;
            steeringObstacleManager.boids = new List<Boid>();

        }
        else if (steeringObstacleManager != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] List<Boid> boids = new List<Boid>();
}
