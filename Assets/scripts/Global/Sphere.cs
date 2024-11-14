using UnityEngine;

public class Sphere
{
    public Vector3 position;
    public Vector3 velocity;
    public float radius;
    public float mass;
    public GameObject visualObject;

    public Sphere(Vector3 pos, Vector3 vel, float rad, float m)
    {
        position = pos;
        velocity = vel;
        radius = rad;
        mass = m;

        // Création de l'objet GameObject pour la sphère visuelle
        visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualObject.GetComponent<Renderer>().material.color = Color.red; // Par exemple, rouge
        visualObject.transform.position = position;
        visualObject.transform.localScale = Vector3.one * radius * 2; // Ajuste la taille en fonction du rayon
    }

    public void UpdatePosition(float deltaTime)
    {
        position += velocity * deltaTime;
        visualObject.transform.position = position; // Mise à jour de la position visuelle
    }
}
