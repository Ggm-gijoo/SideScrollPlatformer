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
        mainCam = GetComponent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, playerTransform.position + camDistance, 5f * Time.deltaTime);
    }
}
