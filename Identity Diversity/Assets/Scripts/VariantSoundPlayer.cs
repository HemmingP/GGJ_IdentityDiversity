using UnityEngine;

public class VariantSoundPlayer : MonoBehaviour
{
    public void PlayRandomSound()
    {
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();

        if (audioSources.Length == 0)
        {
            Debug.LogWarning("No AudioSource components found in children of " + gameObject.name);
            return;
        }

        int randomIndex = Random.Range(0, audioSources.Length);
        audioSources[randomIndex].Play();
    }
}
