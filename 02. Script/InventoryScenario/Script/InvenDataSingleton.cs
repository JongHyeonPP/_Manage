using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InvenDataSingleton : MonoBehaviour
{
    private static InvenDataSingleton inventorySingleton;
    public GameObject trash = null;
    public InvenDataSet data = null;

    public InvenDataSingleton CheckInitOn_DataSgt()
    {
        if (inventorySingleton == null)
        {
            inventorySingleton = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            inventorySingleton.gameObject.SetActive(true);
            inventorySingleton.trash = gameObject;
        }

        return inventorySingleton;
    }

    public void Refreash()
    {
        if (trash != null)
        {
            Destroy(trash);
            trash = null;
        }

        if (data == null)
            data = new();
    }

    public InvenDataSet LoadData()
    {
        Refreash();
        if (inventorySingleton != this)
            Debug.Log("Tlqkf11");

        return data;
    }

    /*
    public void SaveData(InvenData _data)
    {
        data = _data; 
    }


    private T DeepCopy<T>(T obj)
    {
        using (var stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }*/
}

