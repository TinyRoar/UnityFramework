using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_5_3_OR_NEWER
    using UnityEngine.SceneManagement;
#endif

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> LoadingBarList;
    [SerializeField]
    private int LevelIndex = 1;
    private AsyncOperation async;

    void Start()
    {
        #if UNITY_5_2 || UNITY_5_1 || UNITY_5_0
            async = Application.LoadLevelAsync(LevelIndex);
        #else
            async = SceneManager.LoadSceneAsync(LevelIndex);
        #endif

        StartCoroutine(LevelCoroutine());

    }

    IEnumerator LevelCoroutine()
    {
        float progress = 0;

        while (!async.isDone)
        {
            progress = async.progress+0.11f;
            UpdateUI(progress);

            yield return null;
        }

        if (progress >= 0.9f)
            progress += 0.1f;

        UpdateUI(progress);

        yield return null;
    }

    void UpdateUI(float progress)
    {
        foreach (var LoadingBar in LoadingBarList)
        {
            if (LoadingBar.GetComponent<Image>() != null)
                LoadingBar.GetComponent<Image>().fillAmount = progress;
            else if (LoadingBar.GetComponent<Text>() != null)
                LoadingBar.GetComponent<Text>().text = (int)(progress * 100) + " %";
        }
    }
}