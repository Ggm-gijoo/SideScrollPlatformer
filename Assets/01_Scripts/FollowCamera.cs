using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera mainCam;
    private Transform playerTransform;
    public Vector3 camDistance;

    private void Awake()
    {
        mainCam = Camera.main;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        mainCam.transform.position = playerTransform.position + camDistance;
    }
}
