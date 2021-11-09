using UnityEngine;
using static GameManager;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class LoadSave
{
    static string path = Application.persistentDataPath;
    public static Progress progress;
    static Progress zeroProgress;
    public static UnityAction savingProgress, clearObjects;

    static char[] splitChars = new char[] { ' ', '(' };

    /// <summary>
    /// Position, Geschw., etc.
    /// </summary>
    [System.Serializable]
    public class ObjectData
    {
        public string name;
        public float[] position;
        public float rotation;
        public float[] velocity;

        public bool activated;
        public float stateVal;

        public ObjectData(GameObject obj, Vector2 vel) => Init(obj, vel, false, 0);
        public ObjectData(GameObject obj, Vector2 vel, bool active, float _stateVal) => Init(obj, vel, active, stateVal);
        public void Init(GameObject obj, Vector2 vel, bool active, float _stateVal)
        {
            name = obj.name.Split(splitChars)[0];
            position = new float[2] { obj.transform.position.x, obj.transform.position.y };
            rotation = obj.transform.eulerAngles.z;
            velocity = new float[2] { vel.x, vel.y };
            activated = active;
            stateVal = _stateVal;
        }


    }

    /// <summary>
    /// Beinhaltet Level, die geschafft wurden sowie Items, die aufgesammelt wurden
    /// </summary>
    [System.Serializable]
    public class Progress
    {
        public int level;
        public bool kidnapped;
        public string objectName;

        public float time;

        public List<ObjectData> objects = new List<ObjectData>();
    }

    public static bool ResetProgress()
    {
        int level = progress != null ? progress.level : 0;
        string filePath = path + "/sftww/" + "sftww_" + level + ".game";
        //foreach(string filePath in Directory.GetFiles(path + "/notbreakout")) File.Delete(filePath);
        progress = null;

        if (!File.Exists(filePath)) return false;
        File.Delete(filePath); return true;
    }

    public static void ResetGame()
    {
        gameComplete = false;
        if (!Directory.Exists(path + "/sftww")) return;
        foreach(string filePath in Directory.GetFiles(path + "/sftww")) File.Delete(filePath);
    }

    public static bool SaveFileExists() => File.Exists(path + "/sftww/sftww_1.game");
    public static int GetLevel()
    {
        if (!Directory.Exists(path + "/sftww")) Directory.CreateDirectory(path + "/sftww");
        return Directory.GetFiles(path + "/sftww").Length;
    }


    public static bool Load()
    {
        int level = GetLevel();
        if (File.Exists(path + "/sftww/" + "sftww_" + GetLevel() + ".game"))
        {
            Debug.Log("loading stuff");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path + "/sftww/" + "sftww_" + level + ".game", FileMode.Open);
            progress = (Progress)formatter.Deserialize(stream);
            stream.Close();

            //Zerstöre alle alten Objekte:
            if (clearObjects != null) clearObjects.Invoke();
            manager.SpawnObjects();

            return true;
        }
        else
        {
            Debug.Log("new");
            if(zeroProgress != null)
            {
                progress = zeroProgress;

                //Zerstöre alle alten Objekte:
                if (clearObjects != null) clearObjects.Invoke();
                manager.SpawnObjects();
                progress.time = 0;
            }
            else
            {
                GetNewProgress();
                zeroProgress = progress;
            }
            return false;
        }
    }

    public static void Save(string objName)
    {
        if (!Directory.Exists(path + "/sftww"))
            Directory.CreateDirectory(path + "/sftww");

        GetNewProgress();

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "/sftww/" + "sftww_" + progress.level + ".game", FileMode.Create);
        formatter.Serialize(stream, progress);
        stream.Close();
    }

    static void GetNewProgress(string objName = "")
    {
        float time_tmp = progress == null ? 0 : progress.time;
        int level_tmp = progress == null ? 0 : progress.level+1;

        progress = new Progress();//progress.objects.Clear();
        progress.level = level_tmp;
        progress.kidnapped = kidnapped;
        progress.objectName = objName;
        progress.time = time_tmp;
        if (MenuScript.mScript != null) MenuScript.mScript.UpdateText();

        if (savingProgress != null) savingProgress.Invoke();//speichere alle Objekte ab
        else Debug.Log("Error: UnityAction kann nicht aufgerufen werden");
    }
}
