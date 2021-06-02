using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public int indexMenuScene;
    public int indexGameScene;
    public int indexEditorScene;

    private void Start()
    {
        instance = GetComponent<MenuManager>();
    }
    public void LoadStartGame()
    {
        MainBoard.loadLevel = 1;
        SceneManager.LoadScene(indexGameScene);
    }

    public void LoadEditorScene()
    {
        SceneManager.LoadScene(indexEditorScene);
    }

    public void LoadLevel(Text levelText)
    {
        int level = System.Convert.ToInt32(levelText.text);
        if (!Save.ExistLevel(level)) return;
        MainBoard.loadLevel = level;
        SceneManager.LoadScene(indexGameScene);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(indexMenuScene);
    }

    public bool isGameScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == indexGameScene) return true;
        else return false;
    }
}
