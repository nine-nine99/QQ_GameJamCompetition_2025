using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool> {

    private Dictionary<string, ArrayList> poolDic = new Dictionary<string, ArrayList>();
    private Dictionary<string, GameObject> GODic = new Dictionary<string, GameObject>();

    private Dictionary<string, ArrayList> allPoolDic = new Dictionary<string, ArrayList>();


    private Transform poolTrans;
    public Transform PoolTrans {
        get {
            return poolTrans;
        }
    }

    /// <summary>
    /// 初始化对象池节点
    /// </summary>
    public void Init() {
        GameObject poolGo = new GameObject("Pool");
        poolTrans = poolGo.transform;
    }

    // 对象池加载(指定父物体
    public GameObject Get(string prefabName, Transform parentTrans, bool isResetPos = false) {
        return GetGO(null, prefabName, parentTrans, isResetPos);
    }

    // 对象池加载(默认父物体)
    public GameObject Get(string prefabName, bool isResetPos = false) {
        return GetGO(null, prefabName, poolTrans, isResetPos);
    }

    // 对象池加载(指定父物体)
    public GameObject Get(string path, string prefabName, Transform parentTrans, bool isResetPos = false) {
        return GetGO(path, prefabName, parentTrans, isResetPos);
    }

    // 对象池加载(默认父物体)
    public GameObject Get(string path, string prefabName, bool isResetPos = false) {
        return GetGO(path, prefabName, poolTrans, isResetPos);
    }

    // 获取对象池默认父物体
    public Transform GetTrans() {
        return poolTrans;
    }

    // 对象池回收
    public GameObject Recycle(GameObject o, bool isResetParent = false) {
        string key = o.gameObject.name;
        if (!poolDic.ContainsKey(key)) {
            poolDic[key] = new ArrayList() { o };
        }
        else {
            poolDic[key].Add(o);
        }
        if (isResetParent) {
            o.transform.SetParent(poolTrans);
        }
        o.SetActive(false);
        return o;
    }

    private GameObject GetGO(string path, object prefabName, Transform parentTrans, bool isResetPos) {
        string key = prefabName + "(Clone)";
        string name = prefabName.ToString();
        GameObject o;
        if (poolDic.ContainsKey(key) && poolDic[key].Count > 0) {
            ArrayList list = poolDic[key];
            o = list[0] as GameObject;
            list.RemoveAt(0);
            o.transform.SetParent(parentTrans);
            o.name = key;
        }
        else {
            if (GODic.ContainsKey(name))
                o = GameObject.Instantiate(GODic[name], parentTrans) as GameObject;
            else {
                string loadPath = path == null ? prefabName.ToString() : path + "/" + prefabName;
                GameObject prefab = LocalAssetMgr.Instance.Load_Prefab(loadPath);
                o = GameObject.Instantiate(prefab, parentTrans) as GameObject;
                GODic.Add(name, prefab);
            }
            o.name = key;
        }
        if (isResetPos) {
            o.transform.localPosition = Vector3.zero;
            o.transform.localEulerAngles = Vector3.zero;
        }
        o.SetActive(true);
        return o;
    }

    // 万能对象池加载
    public T GetObj<T>(string path, string prefabName) where T : Object {
        string key = prefabName + "(Clone)";
        T o;
        if (allPoolDic.ContainsKey(key) && allPoolDic[key].Count > 0) {
            ArrayList list = allPoolDic[key];
            o = list[0] as T;
            list.RemoveAt(0);
        }
        else {
            o = LocalAssetMgr.Instance.Load_Asset<T>(path, prefabName);
            if (allPoolDic.ContainsKey(key)) {
                allPoolDic[key].Add(o);
            }
            else {
                ArrayList list = new ArrayList();
                list.Add(o);
                allPoolDic[key] = list;
            }
        }
        o.name = key;
        return o;
    }

    // 万能对象池回收
    public T RecycleObj<T>(T t) where T : Object {
        string key = t.name;
        if (!allPoolDic.ContainsKey(key)) {
            allPoolDic[key] = new ArrayList() { t };
        }
        else {
            allPoolDic[key].Add(t);
        }
        return t;
    }

    //清空对象池
    public void Clear() {
        allPoolDic.Clear();
        poolDic.Clear();
        GODic.Clear();
    }
}