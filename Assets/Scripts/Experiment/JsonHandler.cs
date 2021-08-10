namespace Experiment
{
    using System.IO;
    using UnityEngine;

    public class JsonHelper
    {
        /// <summary> Saves savegame data to a JSON file.</summary>
        /// <param name="ed">The savegame data to be saved.</param>
        /// <param name="filename">The filename without file ending.</param>
        public static void SaveData(ExperimentData ed, string filename)
        {
            string json = JsonUtility.ToJson(ed);

            File.WriteAllText(Application.persistentDataPath + "/" + filename + ".json", json);

#if UNITY_EDITOR
            Debug.Log(filename + ".json saved at:" + Application.persistentDataPath);
#endif
        }

        /// <summary> Loads data from internal savegame, e.g. new game save state. </summary>
        public static ExperimentData LoadDataInternal(string filename)
        {
            ExperimentData ed = new ExperimentData();

            TextAsset json = Resources.Load<TextAsset>("InternalSaveStates/" + filename);

            ed = JsonUtility.FromJson<ExperimentData>(json.text);

            return ed;
        }

        public static ExperimentData LoadData(string filename)
        {
            ExperimentData ed = new ExperimentData();

            string json = File.ReadAllText(Application.persistentDataPath + "/" + filename + ".json");

            ed = JsonUtility.FromJson<ExperimentData>(json);

            return ed;
        }

        public static void Delete(string filename)
        {
            File.Delete(Application.persistentDataPath + "/" + filename + ".json");
        }

        public static bool CheckExists(string filename)
        {
            string path = Application.persistentDataPath + "/" + filename + ".json";
            return File.Exists(path);
        }
    }
}