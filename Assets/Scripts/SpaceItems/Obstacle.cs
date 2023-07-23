using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float PushAwayMultiplier = 15;
    private void OnCollisionEnter(Collision coll)
    {
        Ship ship = coll.collider.GetComponentInParent<Ship>();
        if (ship != null)
        {
            Rigidbody shipRB = coll.rigidbody;
            Vector3 pushVector = (ship.transform.position - transform.position).normalized * PushAwayMultiplier;
            shipRB.AddForce(pushVector, ForceMode.VelocityChange);
            
            ship.MovementService.CancelMovement();
            // ship.BroadcastMessage("Die", SendMessageOptions.DontRequireReceiver);
        }
    }
}
