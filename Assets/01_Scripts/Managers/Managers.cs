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

           //instance._sound.Init();
       }
    }
}
