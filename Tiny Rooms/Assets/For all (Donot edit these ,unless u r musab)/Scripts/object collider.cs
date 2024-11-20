using UnityEngine;

public class ObjectPusher : MonoBehaviour
{
    public float pushForce = 2f; // Horizontal force applied to the object
    public float upwardForce = 0.5f; // Upward force for a realistic effect

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if the collided object has a Rigidbody
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            // Calculate the direction of the push
            Vector3 forceDirection = hit.point - transform.position;
            forceDirection.y = 0; // Keep the force horizontal

            // Apply the force to the object
            rb.AddForce(forceDirection.normalized * pushForce + Vector3.up * upwardForce, ForceMode.Impulse);

            Debug.Log("Pushed: " + hit.gameObject.name);
        }
    }
}
