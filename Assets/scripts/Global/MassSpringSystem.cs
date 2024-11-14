using UnityEngine;

public class MassSpringSystem : MonoBehaviour
{
    [Header("Param�tres des Masses")]
    public float m1 = 1.0f; // Masse 1
    public float m2 = 1.0f; // Masse 2

    [Header("Param�tres des Ressorts")]
    public float k1 = 100.0f; // Constante du ressort 1
    public float k2 = 100.0f; // Constante du ressort 2

    [Header("Param�tres d'Amortissement")]
    public float c1 = 5.0f; // Coefficient d'amortissement 1
    public float c2 = 5.0f; // Coefficient d'amortissement 2

    [Header("Param�tres du Ressort")]
    public float L0 = 1.0f; // Longueur au repos du ressort

    [Header("Positions Initiales")]
    public Vector3 initialPosition1 = new Vector3(0, 2, 0); // Position initiale de la masse 1
    public Vector3 initialPosition2 = new Vector3(0, 0, 0); // Position initiale de la masse 2

    private Vector3 pos1;
    private Vector3 pos2;
    private Vector3 vel1;
    private Vector3 vel2;

    [Header("Temps et Pas de Temps")]
    public float time = 0.0f; // Temps de simulation
    public float dt = 0.01f; // Pas de temps

    private GameObject mass1;
    private GameObject mass2;

    void Start()
    {
        pos1 = initialPosition1;
        pos2 = initialPosition2;
        vel1 = Vector3.zero;
        vel2 = Vector3.zero;

        // Cr�er des objets de masse
        mass1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mass2 = GameObject.CreatePrimitive(PrimitiveType.Cube);

        mass1.transform.position = pos1;
        mass2.transform.position = pos2;

        // Ajuster la taille des cubes
        mass1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        mass2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Update()
    {
        // Mise � jour de la simulation
        RK4Step();
        UpdateMassPositions();
    }

    void RK4Step()
    {
        // Forces initiales
        Vector3 force1 = Vector3.zero;
        Vector3 force2 = Vector3.zero;
        float g = 9.81f; // Acc�l�ration due � la gravit�

        // Calculer les forces sur chaque masse
        force1 += -k1 * (pos1.y - L0) * Vector3.up; // Force du ressort 1
        force1 += -k2 * (pos1.y - pos2.y) * Vector3.up; // Force du ressort 2
        force1 += -c1 * vel1; // Force d'amortissement 1
        force1 += m1 * g * Vector3.down; // Force gravitationnelle (vers le bas)

        force2 += -k2 * (pos2.y - pos1.y) * Vector3.up; // Force du ressort 2
        force2 += -c2 * vel2; // Force d'amortissement 2
        force2 += m2 * g * Vector3.down; // Force gravitationnelle (vers le bas)

        // Syst�me d'�quations
        Vector4 state = new Vector4(pos1.y, vel1.y, pos2.y, vel2.y);

        // D�finir les fonctions d�riv�es
        Vector4 k1_local = dt * Derivative(state, force1, force2); // Corrig� : renomm� en k1_local
        Vector4 k2_local = dt * Derivative(state + 0.5f * k1_local, force1, force2); // Corrig� : renomm� en k2_local
        Vector4 k3 = dt * Derivative(state + 0.5f * k2_local, force1, force2);
        Vector4 k4 = dt * Derivative(state + k3, force1, force2);

        // Mettre � jour l'�tat
        pos1.y += (k1_local.x + 2 * k2_local.x + 2 * k3.x + k4.x) / 6.0f;
        vel1.y += (k1_local.y + 2 * k2_local.y + 2 * k3.y + k4.y) / 6.0f;
        pos2.y += (k1_local.z + 2 * k2_local.z + 2 * k3.z + k4.z) / 6.0f;
        vel2.y += (k1_local.w + 2 * k2_local.w + 2 * k3.w + k4.w) / 6.0f;
    }

    Vector4 Derivative(Vector4 state, Vector3 force1, Vector3 force2)
    {
        float y1 = state.x;
        float v1 = state.y;
        float y2 = state.z;
        float v2 = state.w;

        float dy1 = v1;
        float dv1 = (-k1 * (y1 - L0) - k2 * (y1 - y2) - c1 * v1) / m1 + 9.81f;
        float dy2 = v2;
        float dv2 = (-k2 * (y2 - y1) - c2 * v2) / m2 + 9.81f;

        return new Vector4(dy1, dv1, dy2, dv2);
    }

    void UpdateMassPositions()
    {
        mass1.transform.position = pos1;
        mass2.transform.position = pos2;
    }
}
