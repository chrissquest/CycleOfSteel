using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public float maxHealth;
    public float health;
    public float moveSpeed;
    public float critChance;
    public int dataCells;
    public int slotSelected;
    public int[] inventoryIDs;
    public int[] upgradeMenuLevels;
    public bool[] eventsCompleted;
    public float difficulty;

    public PlayerSaveData() 
    {
        difficulty = 1f;
        // I think this is useless bc it gets overriden by load to null anyways
        upgradeMenuLevels = new int[3];
        for (int i = 0; i < upgradeMenuLevels.Length; i++)
            upgradeMenuLevels[i] = 0;
        
        // Test if this is necissary
        //eventsCompleted = new bool[5];

        inventoryIDs = new int[4];
        for (int i = 0; i < inventoryIDs.Length; i++)
            inventoryIDs[i] = -1;
    }

    // https://discussions.unity.com/t/how-do-you-save-write-and-load-from-a-file/180577/2
    public void Save()
    {
        string destination = Application.persistentDataPath + "/save1.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, this);
        file.Close();
    }

    public PlayerSaveData Load()
    {
        string destination = Application.persistentDataPath + "/save1.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            // Its ok if no file was found, just return null
            //Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        PlayerSaveData data = (PlayerSaveData)bf.Deserialize(file);
        file.Close();

        return data;
    }

}
