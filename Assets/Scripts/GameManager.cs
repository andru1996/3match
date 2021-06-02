using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        MainBoard.instance.LoadBoard(MainBoard.loadLevel);
    }

    public static void GameOver()
    {
        MainBoard.instance.LoadBoard(MainBoard.loadLevel);
    }

    public static void GameWin()
    {
        if(Save.ExistLevel(MainBoard.loadLevel+1))
        {
            MainBoard.loadLevel++;
            MainBoard.instance.LoadBoard(MainBoard.loadLevel);
        }
        else
        {
            MenuManager.instance.LoadMenu();
        }
    }

}
