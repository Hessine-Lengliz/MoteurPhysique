using UnityEngine;

public static class RK4Utility
{
    public static (Vector3, Vector3) RK4SolverMethod(Vector3 pos, Vector3 vel, Vector3 acc, float dt)
    {
        Vector3 k1 = dt * vel;
        Vector3 l1 = dt * acc;

        Vector3 k2 = dt * (vel + 0.5f * l1);
        Vector3 l2 = dt * (acc + 0.5f * l1);

        Vector3 k3 = dt * (vel + 0.5f * l2);
        Vector3 l3 = dt * (acc + 0.5f * l2);

        Vector3 k4 = dt * (vel + l3);
        Vector3 l4 = dt * (acc + l3);

        Vector3 newPos = pos + (k1 + 2 * k2 + 2 * k3 + k4) / 6.0f;
        Vector3 newVel = vel + (l1 + 2 * l2 + 2 * l3 + l4) / 6.0f;

        return (newPos, newVel);
    }
}
