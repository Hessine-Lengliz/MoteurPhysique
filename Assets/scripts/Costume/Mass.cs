using UnityEngine;
public class Mass : MonoBehaviour
{  [SerializeField]
    public float MassValue;
      [SerializeField]    public Vector3 Position;
    public Vector3 Velocity;
    [SerializeField]
   public Vector3 netForce  = Vector3.zero;
    public void ApplyForce(Vector3 force)
    {
        netForce += force;
    }

    public void UpdateState(float deltaTime)
    {
        Vector3 acceleration = netForce / MassValue;
        Velocity += acceleration * deltaTime;
        Position += Velocity * deltaTime;
        netForce = Vector3.zero;  // Reset net force after each update
    }

    public void ResetForce()
    {
        netForce = Vector3.zero;
    }
}