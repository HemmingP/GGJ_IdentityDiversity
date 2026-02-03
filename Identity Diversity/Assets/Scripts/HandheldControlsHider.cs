using UnityEngine;

public class HandheldControlsHider : MonoBehaviour
{
    private static HandheldControlsHider instance;
    public static HandheldControlsHider Instance()
    {
        return instance;
    }

    void Awake()
    {
        // Making this into a singleton to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            gameObject.SetActive(false);
        }
    }
}
