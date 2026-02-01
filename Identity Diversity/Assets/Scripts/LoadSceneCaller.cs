using UnityEngine;

public class LoadSceneCaller : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        SceneLoader.Instance().LoadSceneByName(sceneName);
    }
}
