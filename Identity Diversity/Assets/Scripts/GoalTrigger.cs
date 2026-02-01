using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerEventer : MonoBehaviour
{
    public string nextSceneName = "";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null)
        {
            SceneLoader.Instance()?.LoadSceneByName(nextSceneName);
        }
    }
}
