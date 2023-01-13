using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCurrentLevel : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("SceneLevel"))
        {
            int sceneLvl = PlayerPrefs.GetInt("SceneLevel");
            SceneManager.LoadScene(sceneLvl);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
