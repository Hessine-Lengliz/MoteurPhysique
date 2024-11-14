using UnityEngine;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    public List<FreeFallWithRK4> balls;

    void Start()
    {
        // Collect all FreeFallWithRK4 objects in the scene
        balls = new List<FreeFallWithRK4>(FindObjectsOfType<FreeFallWithRK4>());
    }

    void FixedUpdate()
    {
        foreach (FreeFallWithRK4 ball in balls)
        {
            ball.FixedUpdate(); // Update the position of each ball

            // Check for collisions between balls
            foreach (FreeFallWithRK4 otherBall in balls)
            {
                if (ball != otherBall)
                {
                    ball.HandleCollisionWithBall(otherBall);
                }
            }
        }
    }
}
