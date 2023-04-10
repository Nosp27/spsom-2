using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FMODUnity;
using Newtonsoft.Json;
using UnityEngine;

public class Utils
{
    public static float Projection(Vector3 v1, Vector3 v2)
    {
        return Vector3.Dot(v1, v2) / v2.magnitude;
    }

    public static void PlayAudioDetached(AudioSource audioSource)
    {
        if (!audioSource)
            return;

        audioSource.transform.SetParent(null);
        audioSource.Play();
        GameObject.Destroy(audioSource.gameObject, audioSource.clip.length);
    }


    public class GameStorage
    {
        public static void SaveData(string key, object obj, bool overwrite = false)
        {
            string root = Environment.CurrentDirectory;
            string path = Path.Combine(root, "Storage", "Internal", $"{key}.b");
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                if (!overwrite)
                    throw new Exception($"Path {path} exists. Not allowed to overwrite.");
                file.Delete();
            }

            if (file.Directory != null && !file.Directory.Exists) 
                file.Directory.Create();

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                formatter.Serialize(stream, obj);
                Debug.Log("Write done");
            }
        }
    
        public static T LoadData<T>(string key)
        {
            string root = Environment.CurrentDirectory;
            string path = Path.Combine(root, "Storage", "Internal", $"{key}.b");
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                throw new IOException($"File {path} does not exist");
            }

            BinaryFormatter formatter = new BinaryFormatter();
            
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (T)formatter.Deserialize(stream);
            }
        }   
    }
}