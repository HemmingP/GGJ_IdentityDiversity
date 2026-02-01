using UnityEngine;

public class RagdollPartHolder : MonoBehaviour
{
    public void EnableRagdoll()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            // rb2d.simulated = true;
        }

        Collider2D collider2d = GetComponent<Collider2D>();
        if (collider2d != null)
        {
            collider2d.enabled = true;
        }

        HingeJoint2D hingeJoint2D = GetComponent<HingeJoint2D>();
        if (hingeJoint2D != null)
        {
            hingeJoint2D.enabled = true;
        }
    }
}
