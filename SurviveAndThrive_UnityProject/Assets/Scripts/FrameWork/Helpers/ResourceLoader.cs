using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceLoader {
    Dictionary<string, string> paths = new Dictionary<string, string>();
    Dictionary<string, FileInfo> resourcesDictionary = new Dictionary<string, FileInfo>();
    private static Dictionary<string, FileInfo> resourcesExtensionsDictionary = new Dictionary<string, FileInfo>();

    public static string[] GetAllScenesInProject() {
        GetAllResourcesWithExtension();
        List<string> scenes = new List<string>();

        foreach (KeyValuePair<string, FileInfo> pair in resourcesExtensionsDictionary) {
            if (pair.Key.Contains(".unity")) {
                scenes.Add(Path.GetFileNameWithoutExtension(pair.Key));
            }
        }

        string[] scenesArray = scenes.ToArray();
        return scenesArray;
    }

    public string GetResourcePathByName(string resourceName) {
        GetAllPaths();

        string path = null;

        foreach (KeyValuePair<string, string> pair in paths) {
            if (pair.Key == resourceName) {
                path = pair.Value;
            }
        }
        return path;
    }

    private void GetAllPaths() {
        GetAllResources();

        paths.Clear();

        string path = null;

        foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
            path = resource.ToString();
            if (!paths.ContainsKey(resource.Key))
                paths.Add(resource.Key, path);
        }
    }

#if UNITYEDITOR
	public List<T> GetAllResourcesInEditorByType<T>() where T : Object {
		GetAllResources();

		List<T> typeList = new List<T>();

		foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
			string subString = Application.dataPath;
			string path = resource.Value.ToString().Remove(0, subString.Length);
			T obj = (T)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets" + path, typeof(T));
			if (obj != null) {
				typeList.Add(obj);
			}
		}

		return typeList;
	}
#endif

	public List<T> GetAllResourcesByType<T>() where T : Object {
		GetAllResources();

		List<T> typeList = new List<T>();

		string subString = Application.dataPath + @"\Resources\";

		foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
			string path = resource.Value.ToString().Remove(0, subString.Length);
			path = Path.ChangeExtension(path, null);
			path = path.Replace(@"\", "/");

			T obj = (T)Resources.Load(path, typeof(T));
			//Debug.Log("BaseItem: " + obj + " Path: " + path);
			if (obj != null) {
				typeList.Add(obj);
			}
		}

		return typeList;
	}

	public List<string> GetAllResourcesPathsByType<T>() where T : Object {
		GetAllResources();

		List<string> typePaths = new List<string>();

		string subString = Application.dataPath + @"\Resources\";
		foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
			string path = resource.Value.ToString().Remove(0, subString.Length);
			path = Path.ChangeExtension(path, null);
			path = path.Replace(@"\", "/");

			T obj = (T)Resources.Load(path, typeof(T));
			if (obj != null) {
				typePaths.Add(path);
			}
		}

		return typePaths;
	}

	// --------------- By Name ---------------
	#region By Name
	public Object GetResourceByName(string resourceName) {
        GetAllResources();

        Object file = null;
        foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
            if (resource.Key == resourceName) {
                file = Resources.Load(resource.Value.ToString());
                Debug.Log(resource.Value.ToString());
            }
        }
        return file;
    }

    private void GetAllResources() {
        resourcesDictionary.Clear();

        DirectoryInfo info = new DirectoryInfo("Assets/Resources");

        GetAllFiles(info);
        GetAllFolders(info);
    }

    private void GetAllFiles(DirectoryInfo info) {
        FileInfo[] files = info.GetFiles();

        foreach (FileInfo file in files) {
            if (!file.Name.Contains(".meta")) {
                if (!resourcesDictionary.ContainsKey(Path.GetFileNameWithoutExtension(file.Name)))
                    resourcesDictionary.Add(Path.GetFileNameWithoutExtension(file.Name), file);
            }
        }
    }


    private void GetAllFolders(DirectoryInfo info) {
        DirectoryInfo[] folders = info.GetDirectories();

        foreach (DirectoryInfo folder in folders) {
            GetAllFiles(folder);
            GetAllFolders(folder);
        }
    }
    #endregion
    // --------------- By Name ---------------

    // --------------- By Extension ---------------
    #region By Extension
    public static List<Object> GetResourcesByExtension(string extension) {
        GetAllResourcesWithExtension();

        List<Object> files = new List<Object>();
        foreach (KeyValuePair<string, FileInfo> resource in resourcesExtensionsDictionary) {
            if (resource.Key.Contains(extension)) {
                files.Add(Resources.Load(resource.Value.ToString()));
            }
        }
        return files;
    }

    private static void GetAllResourcesWithExtension() {
        resourcesExtensionsDictionary.Clear();

        DirectoryInfo info = new DirectoryInfo("Assets");

        GetAllFilesWithExtensions(info);
        GetAllFoldersWithExtension(info);
    }

    private static void GetAllFilesWithExtensions(DirectoryInfo info) {
        FileInfo[] files = info.GetFiles();

        foreach (FileInfo file in files) {
            //string fileName = file.Name;

            if (!file.Name.Contains(".meta")) {
                if (!resourcesExtensionsDictionary.ContainsKey(file.Name))
                    resourcesExtensionsDictionary.Add(file.Name, file);
            }
        }
    }

    private static void GetAllFoldersWithExtension(DirectoryInfo info) {
        DirectoryInfo[] folders = info.GetDirectories();

        foreach (DirectoryInfo folder in folders) {
            GetAllFilesWithExtensions(folder);
            GetAllFoldersWithExtension(folder);
        }
    }
    #endregion
    // --------------- By Extension ---------------

    // --------------- Other Stuff ---------------
    #region Other Stuff
    private void OnGUI() {
        GUILayout.BeginArea(new Rect(400, 0, 200, 200));
        if (GUILayout.Button("Get All Files In Resources")) {
            GetAllResources();
        }

        if (GUILayout.Button("Print All Files")) {
            if (resourcesDictionary != null) {
                foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
                    Debug.Log("File Name: " + resource.Key);
                }
            } else {
                Debug.Log("No files in folder");
            }
        }

        if (GUILayout.Button("Get Resource By Name")) {
            Material color = (Material)GetResourceByName("Color 2");
            GameObject.Find("TargetObject").GetComponent<Renderer>().material = color;
        }

        if (GUILayout.Button("Print Out Dictonary")) {
            PrintOutNamesDictionary();
        }
        GUILayout.EndArea();
    }

    public void PrintOutNamesDictionary() {
        GetAllResources();

        foreach (KeyValuePair<string, FileInfo> resource in resourcesDictionary) {
            Debug.Log("Name: " + resource.Key + " File: " + resource.Value);
        }
    }

    public void PrintOutExtensionDictionary() {
        GetAllResourcesWithExtension();

        foreach (KeyValuePair<string, FileInfo> resource in resourcesExtensionsDictionary) {
            Debug.Log("Name: " + resource.Key + " Extension: " + resource.Value.Extension + " File: " + resource.Value);
        }
    }
    #endregion
    // --------------- Other Stuff ---------------
}
