using UnityEngine;

public class FreeFallWithCollisionDetection : MonoBehaviour
{
    public Vector3 position;         // Position de l'objet
    public Vector3 velocity;         // Vitesse de l'objet
    public float mass = 1.0f;        // Masse de l'objet
    public float gravity = -9.81f;   // Accélération gravitationnelle
    public float frictionCoefficient = 0.1f; // Coefficient de frottement
    public float timeStep = 0.02f;   // Pas de temps pour l'intégration

    private bool hasCollided = false; // Pour détecter la première collision

    void Start()
    {
        position = new Vector3(0, 10, 0); // Position initiale au-dessus du sol
        velocity = Vector3.zero;          // Vitesse initiale nulle
    }

    void FixedUpdate()
    {
        // Si la collision est déjà survenue, arrêter le cube au sol
        if (hasCollided)
        {
            position.y = 0;
            velocity = Vector3.zero;
            transform.position = position;
            return;
        }

        // Exécuter l'intégration RK4 pour obtenir la nouvelle position et vitesse
        Vector3 initialPosition = position;
        Vector3 initialVelocity = velocity;
        Vector3 acceleration = ComputeAcceleration(position, velocity);

        RK4Integration(acceleration, out Vector3 newPosition, out Vector3 newVelocity);

        // Vérifier si une collision s'est produite pendant cette étape de temps
        if (HasCollided(initialPosition.y, newPosition.y))
        {
            // Calculer le temps exact de collision t_c
            float collisionTime = CalculateCollisionTime(initialPosition, initialVelocity, timeStep);

            // Mettre à jour l'état à l'instant de collision t_c
            position = initialPosition + initialVelocity * collisionTime + 0.5f * acceleration * collisionTime * collisionTime;
            position.y = 0; // S'assurer que la position en Y est exactement 0 au moment de la collision
            velocity = Vector3.zero; // Vitesse nulle après collision
            hasCollided = true; // Marquer la collision comme survenue
        }
        else
        {
            // Pas de collision, continuer avec les nouvelles valeurs
            position = newPosition;
            velocity = newVelocity;
        }

        // Mise à jour de la position visuelle du cube dans Unity
        transform.position = position;
    }

    Vector3 ComputeAcceleration(Vector3 pos, Vector3 vel)
    {
        // Calcul de l'accélération due à la gravité et au frottement
        Vector3 gravityForce = mass * new Vector3(0, gravity, 0);
        Vector3 frictionForce = -frictionCoefficient * vel;
        return (gravityForce + frictionForce) / mass;
    }

    void RK4Integration(Vector3 acceleration, out Vector3 newPosition, out Vector3 newVelocity)
    {
        // Calcul de l'intégration RK4 pour une étape de temps
        Vector3 k1_v = acceleration * timeStep;
        Vector3 k1_x = velocity * timeStep;

        Vector3 k2_v = ComputeAcceleration(position + k1_x * 0.5f, velocity + k1_v * 0.5f) * timeStep;
        Vector3 k2_x = (velocity + k1_v * 0.5f) * timeStep;

        Vector3 k3_v = ComputeAcceleration(position + k2_x * 0.5f, velocity + k2_v * 0.5f) * timeStep;
        Vector3 k3_x = (velocity + k2_v * 0.5f) * timeStep;

        Vector3 k4_v = ComputeAcceleration(position + k3_x, velocity + k3_v) * timeStep;
        Vector3 k4_x = (velocity + k3_v) * timeStep;

        newVelocity = velocity + (k1_v + 2 * k2_v + 2 * k3_v + k4_v) / 6;
        newPosition = position + (k1_x + 2 * k2_x + 2 * k3_x + k4_x) / 6;
    }

    bool HasCollided(float startY, float endY)
    {
        // Vérifie si l'objet a franchi y=0 entre deux étapes de temps
        return startY > 0 && endY <= 0;
    }

    float CalculateCollisionTime(Vector3 pos, Vector3 vel, float dt)
    {
        // Calculer le temps de collision exact dans l'intervalle [0, dt] en utilisant la méthode de bisection
        float lower = 0.0f;
        float upper = dt;
        float mid = 0.0f;

        while (upper - lower > 1e-5f) // Précision de la collision
        {
            mid = (lower + upper) / 2.0f;
            float testY = pos.y + vel.y * mid + 0.5f * gravity * mid * mid;

            if (testY <= 0)
                upper = mid;
            else
                lower = mid;
        }

        return mid;
    }
}
