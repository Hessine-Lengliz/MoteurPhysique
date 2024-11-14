using UnityEngine;  
public class SpringSimulator : MonoBehaviour
{
    public Mass mass;
    public Spring spring;
    public Vector3 initialForce;
    public PhysicsIntegrator integrator;

    private void Start()
    {
        mass.ApplyForce(initialForce);
        integrator = new PhysicsIntegrator();
    }

    private void Update()
    {
        Vector3 springForce = spring.CalculateForce(mass.Position);
        mass.ApplyForce(springForce);
        
        integrator.Step(mass, Time.deltaTime);
        UpdateMassVisual(mass.Position);
    }

    private void UpdateMassVisual(Vector3 position)
    {
        // Update visual representation in the scene
        transform.position = position;
    }
}