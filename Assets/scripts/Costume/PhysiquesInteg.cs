using UnityEngine;

public class PhysicsIntegrator : MonoBehaviour
{
    public enum IntegrationMethod { Euler, Verlet }
    public IntegrationMethod Method { get; set; } = IntegrationMethod.Verlet; // Default to Verlet

    private Vector3 previousPosition;
    private bool isFirstUpdate = true;

    public void Step(Mass mass, float deltaTime)
    {
        switch (Method)
        {
            case IntegrationMethod.Euler:
                EulerStep(mass, deltaTime);
                break;
            case IntegrationMethod.Verlet:
                VerletStep(mass, deltaTime);
                break;
        }
    }

    private void EulerStep(Mass mass, float deltaTime)
    {
        // Calculate acceleration
        Vector3 acceleration = mass.netForce / mass.MassValue;

        // Update velocity and position
        mass.Velocity += acceleration * deltaTime;
        mass.Position += mass.Velocity * deltaTime;
        
        // Reset net force after the update step
        mass.ResetForce();
    }

    private void VerletStep(Mass mass, float deltaTime)
    {
        if (isFirstUpdate)
        {
            // For the first update, we initialize the previous position based on Euler step
            previousPosition = mass.Position;
            EulerStep(mass, deltaTime);
            isFirstUpdate = false;
        }
        else
        {
            // Calculate acceleration
            Vector3 acceleration = mass.netForce / mass.MassValue;

            // Update position based on Verlet integration
            Vector3 newPosition = 2 * mass.Position - previousPosition + acceleration * deltaTime * deltaTime;

            // Update velocity as well (approximation for rendering or further calculations)
            mass.Velocity = (newPosition - previousPosition) / (2 * deltaTime);

            // Update positions for the next step
            previousPosition = mass.Position;
            mass.Position = newPosition;

            // Reset net force after the update step
            mass.ResetForce();
        }
    }
}
