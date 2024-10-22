using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
//using UnityEngine.Windows;

public static class SaveSystem{
    private const string SAVE_EXTENSION = "txt";

    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static bool isInit = false;

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            //Test if Save Folder Exist
            if (!Directory.Exists(SAVE_FOLDER))
            {
                //Create Save Folder
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overWrite)
    {
        Init();
        string saveFileName = fileName;
        if(!overWrite)
        {
            // Making sure the save number is always unique
            int saveNumber = 0;
            saveFileName = fileName + "_" + saveNumber;
            while (File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }

        }
        else
        {
            int saveNumber = 0;
            saveFileName = fileName + "_" + saveNumber;
            while (File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
            saveNumber--;
            saveFileName = fileName + "_" + saveNumber;
        }

        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Init();
        TextAsset savesString = Resources.Load<TextAsset>(fileName);
        string resourceSaveString = savesString.text;
        Debug.Log(resourceSaveString);
        //string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
        //return saveString;
        return resourceSaveString;
    }
    
    public static string LoadMostRecentFile()
    {
        Init();
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        //Get all save files
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        //Cycling thru all files and indetify the most recent
        FileInfo mostRecentFile = null;
        foreach( FileInfo fileInfo in saveFiles)
        {
            //Debug.Log(fileInfo);
            if(mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = fileInfo;
                }
            }
        }

        if(mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            //Debug.Log(mostRecentFile);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static void SaveObject(object saveObject, bool overwrite)
    {
        SaveObject("save", saveObject, overwrite);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Init();
        string saveString = LoadMostRecentFile();
        
        if(saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Init();
        string saveString = Load(fileName);
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}
