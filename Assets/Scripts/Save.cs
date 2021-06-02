using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
class Save
{
    public int[] saveTargetPoint;
    public int[,] saveTiles;
    public int numMove;

    public static void SaveGame(int level)
    {
        string LevelFileName = "/Level" + level + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + LevelFileName);
        Save data = new Save();
        data.saveTargetPoint = MainBoard.instance.DictionaryToInt(MainBoard.instance.GetTargetPoint());
        data.saveTiles = MainBoard.instance.GameobjectToInt(MainBoard.instance.GetTiles());
        data.numMove = MainBoard.instance.GetNumMove();
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public static void LoadGame(int level)
    {
        string LevelFileName = "/Level" + level + ".dat";
        if (File.Exists(Application.persistentDataPath + LevelFileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + LevelFileName, FileMode.Open);
            Save data = (Save)bf.Deserialize(file);
            file.Close();
            MainBoard.instance.SetTargetPoint(MainBoard.instance.IntToDictionary(data.saveTargetPoint));
            MainBoard.instance.ReloadBoard(MainBoard.instance.IntToGameobject(data.saveTiles));
            MainBoard.instance.SetNumMove(data.numMove);
            Debug.Log("Game data loaded!");
        }
        else
        {
            Debug.LogError("There is no save data!");
        }

    }

    public static bool ExistLevel(int level)
    {
        string LevelFileName = "/Level" + level + ".dat";
        if (File.Exists(Application.persistentDataPath + LevelFileName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //void ResetData(int level)
    //{
    //    string LevelFileName = "/Level" + level + ".dat";
    //    if (File.Exists(Application.persistentDataPath + LevelFileName))
    //    {
    //        File.Delete(Application.persistentDataPath + LevelFileName);
    //        //intToSave = 0;
    //        //floatToSave = 0.0f;
    //        //boolToSave = false;
    //        Debug.Log("Data reset complete!");
    //    }
    //    else
    //    {
    //        Debug.LogError("No save data to delete.");
    //    }

    //}
}
