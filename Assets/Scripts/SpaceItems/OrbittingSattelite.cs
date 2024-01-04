using UnityEngine;

namespace SpaceItems
{
    public class OrbittingSattelite : MonoBehaviour
    {
        [SerializeField] private Transform orbitCenter;
        private Vector3 startRadius;
        [SerializeField] private float orbitSpeed = 2f;

        void Start()
        {
            startRadius = orbitCenter.position - transform.position;
            transform.LookAt(transform.position + ResolveForward(), ResolveUp());
        }

        Vector3 ResolveForward()
        {
            var f = Vector3.ProjectOnPlane(transform.forward, startRadius);
            Debug.DrawRay(transform.position, f * 100, Color.red);
            Debug.DrawRay(transform.position, startRadius, Color.green);
            return f;
        }
        
        Vector3 ResolveUp()
        {
            return Vector3.ProjectOnPlane(transform.up, startRadius);
        }

        void Update()
        {
            transform.RotateAround(orbitCenter.position, transform.up,
                orbitSpeed / startRadius.magnitude * Time.deltaTime);
        }
    }
}