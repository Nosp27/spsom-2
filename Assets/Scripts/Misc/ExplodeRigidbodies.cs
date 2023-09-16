using UnityEngine;
using Random = UnityEngine.Random;

public class ExplodeRigidbodies : MonoBehaviour
{
    [SerializeField] private float force = 65;
    [SerializeField] private Vector2 randomTorque = new Vector2(1, 5);
    private bool explodeOnEnable = true;

    void OnEnable()
    {
        if (explodeOnEnable)
            Explode();
    }

    public void Explode()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(force * (rb.transform.position - transform.position).normalized);
            rb.AddTorque(Random.onUnitSphere * Random.Range(randomTorque.x, randomTorque.y));
        }
    }
}