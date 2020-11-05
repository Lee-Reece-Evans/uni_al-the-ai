using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // following code is taken from a tutorial video by Brackeys: https://www.youtube.com/watch?v=XOjd_qU2Ido 
    public static void SaveLevel(SaveDataManager SDM)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ALsave01.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(SDM);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadLevel()
    {
        string path = Application.persistentDataPath + "/ALsave01.sav";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            SaveLevel(SaveDataManager.instance);
        }
        return null;
    }
}
