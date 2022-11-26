using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float PushAwayMultiplier = 1;
    private void OnCollisionEnter(Collision coll)
    {
        Ship ship = coll.collider.GetComponentInParent<Ship>();
        if (ship != null)
        {
            Rigidbody shipRB = coll.rigidbody;
            Vector3 pushVector = (ship.transform.position - transform.position) * shipRB.mass * PushAwayMultiplier;
            shipRB.velocity = pushVector;
            
            ship.CancelMovement();
            // ship.BroadcastMessage("Die", SendMessageOptions.DontRequireReceiver);
        }
    }
}
