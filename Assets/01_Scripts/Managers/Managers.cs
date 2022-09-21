using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;

    public static Managers Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    #region System

    ScenesManager _scene = new ScenesManager();
    PoolManager _pool = new PoolManager();
    SoundManager _sound = new SoundManager();
    ResourceManager _resource = new ResourceManager();

    public static ScenesManager Scene { get { return Instance._scene; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    #endregion

    private void Awake()
    {
        Init();
    }

    static void Init()
    {
       if (instance == null)
       {
           GameObject obj = GameObject.Find("@Managers");
           if (obj == null)
           {
               obj = new GameObject { name = "@Managers" };
               instance = obj.AddComponent<Managers>();
           }
           DontDestroyOnLoad(obj);
           instance = obj.GetComponent<Managers>();

            instance._pool.Init();
            instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Scene.Clear();
        Pool.Clear();
    }
}
