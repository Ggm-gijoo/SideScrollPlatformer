using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;


    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        
    }

    public abstract void Clear();
}
