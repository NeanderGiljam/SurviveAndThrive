using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

public static class XMLManager {

    public static void XMLWrite<T>(T obj, string fileName, string path, System.Type[] extraTypes) {
        XmlSerializer writer;
        if (extraTypes == null) {
            writer = new XmlSerializer(obj.GetType());
        } else {
            writer = new XmlSerializer(obj.GetType(), extraTypes);
        }
        StreamWriter file = new StreamWriter(Application.dataPath + "/" + path + "/" + fileName + ".xml");
        writer.Serialize(file, obj);
        file.Close();

        Debug.Log("XML Save Succesfull, path: " + Application.dataPath + "/" + path + "/" + fileName + ".xml");
    }

    public static void XMLWrite<T>(T obj, string fileName, System.Type[] extraTypes) {
        XmlSerializer writer;
        if (extraTypes == null) {
            writer = new XmlSerializer(obj.GetType());
        } else {
            writer = new XmlSerializer(obj.GetType(), extraTypes);
        }
        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + fileName + ".xml");
        writer.Serialize(file, obj);
        file.Close();

        Debug.Log("XML Save Succesfull, path: " + Application.persistentDataPath + "/" + fileName + ".xml");
    }

	public static T XMLRead<T>(string fileName, System.Type[] extraTypes) {
		XmlSerializer reader;
        if (extraTypes == null) {
			reader = new XmlSerializer(typeof(T));
		} else {
			reader = new XmlSerializer(typeof(T), extraTypes);
		}
		StreamReader file;
        try {
			file = new StreamReader(Application.persistentDataPath + "/" + fileName + ".xml");
		} catch {
			return default(T);
		}

		T result = (T)reader.Deserialize(file);
        file.Close();

        return result;
    }

    public static T XMLReadFromResources<T>(string fileName) {
        XmlSerializer reader = new XmlSerializer(typeof(T));
        TextAsset xmlDoc = (TextAsset)Resources.Load("XML/" + fileName, typeof(TextAsset));
		if (xmlDoc == null)
			return default(T);

		StringReader file = new StringReader(xmlDoc.text);
        T result = (T)reader.Deserialize(file);
        file.Close();

        return result;
    }

	/* Reimplement when reading from unity projct folder is needed
	public static T XMLRead<T>(string fileName, string path) {
		XmlSerializer reader = new XmlSerializer(typeof(T));
		StreamReader file = new StreamReader(Application.dataPath + "/" + path + "/" + fileName + ".xml");
		if (file == null)
			return default(T);

		T result = (T)reader.Deserialize(file);
		file.Close();

		return result;
	}
	*/
}
