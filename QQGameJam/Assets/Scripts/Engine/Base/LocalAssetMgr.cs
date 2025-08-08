using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System;

/// <summary>
/// 本地资源管理器
/// 
/// 先下载资源到本地，不是边玩边下载
/// </summary>
public class LocalAssetMgr : Singleton<LocalAssetMgr> {
    private const string basic_ui = "ui/";
    private const string basic_refdata = "refdata/";
    private const string basic_music = "Sound/";

    public string EditorFilePath = Application.streamingAssetsPath + "/Editor";

    public void Init() {
        EditorFilePath = EditorFilePath.Replace('/', '\\');
	}
	
	public void Clear(){
		
	}

    /// <summary>
    /// refdata
    /// </summary>
    public void Load_RefData (string name, System.Action<TextAsset> callback) {
        TextAsset tableText = null;

        string path = basic_refdata + name;
        tableText = Resources.Load<TextAsset>(path);

        if (tableText == null ) {
            Debug.LogError("Failed Load_RefData from " + path);
        }

        callback(tableText);
    }

    public GameObject Load_UI(string name) {
        GameObject prefab = null;

        string path = basic_ui + "Win_" + name;
        prefab = Resources.Load(path) as GameObject;
        if (prefab == null) {
            Debug.LogError("Failed UIPrb_Local from " + path);
        }

        return prefab;
    }

    public GameObject Load_Prefab(string name) {
        GameObject prefab = null;
        string subPath = "Prefab/";
        string path = subPath + name;
        prefab = Resources.Load(path) as GameObject;
        if (prefab == null) {
            Debug.LogError("Failed Load_UIPrefab from " + path);
        }
        return prefab;
    }

    public GameObject Load_UIPrefab(string name) {
        GameObject prefab = null;
        string subPath = "Prefab/";
        string path = basic_ui + subPath + name;
        prefab = Resources.Load(path) as GameObject;
        if (prefab == null) {
            Debug.LogError("Failed Load_UIPrefab from " + path);
        }
        return prefab;
    }

    public Sprite Load_UISprite(string pack, string name) {
        Sprite sprite = null;
        string path;
        path = string.Format("Atlas/{0}/{1}", pack, name );
        sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        if (sprite == null) {
            Debug.LogError("Load_UISprite: sprite = " + path + ", failed!");
        }

        return sprite;
    }

    // 加载场景
    public void Load_Scene(string name) {
        Debug.LogWarning("Load_Scene : " + name);
        SceneManager.LoadScene(name);
    }

    public AudioClip Load_Music(string name) {
        AudioClip clip = null;
        string path = "";
        path = basic_music + name;
        clip = Resources.Load(path) as AudioClip;
        if (null == clip) {
            Debug.LogError("Failed Load_Music from " + path);
        }

        return clip;
    }

    public GameObject Load_Avatar(string name) {
        GameObject prefab = null;
        string subPath = "Atlas/Avatar/";
        string path = subPath + name;
        prefab = Resources.Load(path) as GameObject;
        if (prefab == null) {
            Debug.LogError("Failed Load_UIPrefab from " + path);
        }
        return prefab;
    }

    public GameObject Load_Anim(string name) {
        GameObject prefab = null;
        string subPath = "Atlas/Anim/";
        string path = subPath + name;
        prefab = Resources.Load(path) as GameObject;
        if (prefab == null) {
            Debug.LogError("Failed Load_Anim from " + path);
        }
        return prefab;
    }

    public void CreateOrOpenFile(string name, string info) {
        try {
            DirectoryInfo dir = new DirectoryInfo(EditorFilePath);
            if (!dir.Exists) {
                dir.Create();
            }

            StreamWriter sw;
            FileInfo fi = new FileInfo(EditorFilePath + "//" + name + ".dp");
            sw = fi.CreateText();
            sw.WriteLine(info, Encoding.UTF8);
            sw.Close();
        }
        catch (System.Exception ex) {
            Debug.LogError(ex);
        }
    }

    public void CreatPng(string name, Texture2D texture) {
        try {
            DirectoryInfo dir = new DirectoryInfo(EditorFilePath);
            if (!dir.Exists) {
                dir.Create();
            }

            byte[] arrByte = texture.EncodeToPNG();
            File.WriteAllBytes(EditorFilePath + "//" + name + ".png", arrByte);
        }
        catch (System.Exception ex) {
            Debug.LogError(ex);
        }
    }

    public T Load_Asset<T>(string path, string name) where T : UnityEngine.Object {
        T prefab = null;
        string loadPath = path + "/" + name;
        prefab = Resources.Load(loadPath, typeof(T)) as T;
        if (prefab == null) {
            Debug.LogError("Failed Load_Asset from " + loadPath);
        }
        return prefab;
    }

    public void LoadFile(string filePath, string name, System.Action<string> callback) {
        try {
            string path = filePath + "//" + name + ".dp";
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line = sr.ReadToEnd();
            if(line != null){
                callback(line);
            }
            else {
                Debug.LogError("line is null");
            }
            sr.Close();
        }
        catch (IOException e) {
            Debug.LogError(e);
        }
    }

    public void LoadPng(string filePath, string name, System.Action<Texture2D> callback) {
        try {
            string path = filePath + "//" + name;
            FileStream fileSteam = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileSteam.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[fileSteam.Length];
            fileSteam.Read(bytes, 0, (int)fileSteam.Length);
            fileSteam.Close();
            fileSteam.Dispose();
            fileSteam = null;

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            callback(tex);
        }
        catch (IOException e) {
            Debug.LogError(e);
        }
    }

    public FileInfo[] GetAllDP(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly) {
        DirectoryInfo theFolder = new DirectoryInfo(path);
        if (!theFolder.Exists) {
            theFolder.Create();
        }
        FileInfo[] arrFile = theFolder.GetFiles("*.dp", searchOption);
        return arrFile;
    }

    public FileInfo[] GetAllPng(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly) {
        DirectoryInfo theFolder = new DirectoryInfo(path);
        if (!theFolder.Exists) {
            theFolder.Create();
        }
        List<FileInfo> list = new List<FileInfo>();
        list.AddRange( theFolder.GetFiles("*.png", searchOption) );
        list.AddRange( theFolder.GetFiles("*.jpg", searchOption) );
        return list.ToArray();
    }
}
