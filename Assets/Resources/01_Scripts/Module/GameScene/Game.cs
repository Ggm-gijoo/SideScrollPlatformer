using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Base
{

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;

        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            list.Add(Managers.Resource.Instantiate("Monster"));
        }
        Debug.Log(list.Count);
        for(int i = 0; i < 5; i++)
        {
            Managers.Resource.Destroy(list[i]);
            list.RemoveAt(i);
        }
    }
    public override void Clear()
    {

    }
}
