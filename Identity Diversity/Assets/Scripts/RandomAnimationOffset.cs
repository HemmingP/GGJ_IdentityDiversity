using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    [SerializeField] private float maxOffset = 1f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // if (animator != null)
        // {
        //     // Get a random offset between 0 and maxOffset
        //     float randomOffset = Random.Range(0f, maxOffset);

        //     // Set the animation to the random offset
        //     animator.SetFloat("Offset", randomOffset);
        // }
        // else
        // {
        //     Debug.LogWarning("RandomAnimationOffset: Animator component not found on " + gameObject.name);
        // }

        animator.GetCurrentAnimatorStateInfo(0);
        float randomOffset = Random.Range(0f, maxOffset);
        animator.Play(0, -1, randomOffset);
    }
}
