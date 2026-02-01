using UnityEngine;

public class Emitter : MonoBehaviour
{
    public GameObject emittedObject;

    [SerializeField]
    private float emitInterval = 1f;
    [SerializeField]
    private float timeSinceLastEmit = 0f;

    [SerializeField]
    private Vector2 emitOffset = Vector2.zero;
    [SerializeField]
    private float lifetime = 5f;

    // Update is called once per frame
    void Update()
    {
        timeSinceLastEmit += Time.deltaTime;
        if (timeSinceLastEmit >= emitInterval)
        {
            timeSinceLastEmit -= emitInterval;
            GameObject emitted = Instantiate(emittedObject, transform.position + (Vector3)emitOffset, transform.rotation);
            Destroy(emitted, lifetime);
        }
    }
}
