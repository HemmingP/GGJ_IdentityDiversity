using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Oestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    [SerializeField] private UnityEngine.UI.Image blackScreen;
    string sceneNameToTransitionTo = "";

    public void LoadSceneByName(string sceneName)
    {
        sceneNameToTransitionTo = sceneName;
    }

    public void ReloadCurrentScene()
    {
        sceneNameToTransitionTo = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    void LoadScene()
    {
        if (sceneNameToTransitionTo == "Exit Application")
        {
            Application.Quit();
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameToTransitionTo);
        sceneNameToTransitionTo = "";
    }

    void Update()
    {
        if (sceneNameToTransitionTo != "")
        {
            // blackScreen.color = Color.Lerp(blackScreen.color, Color.black, Time.deltaTime);
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Clamp01(blackScreen.color.a + Time.deltaTime));
            if (blackScreen.color.a >= 1f)
            {
                LoadScene();
            }
        }
        else
        {
            // blackScreen.color = Color.Lerp(blackScreen.color, new Color(0f, 0f, 0f, 0f), Time.deltaTime);
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Clamp01(blackScreen.color.a - Time.deltaTime));
        }
    }
}
