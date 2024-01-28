using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Camera m_camera;

    void Start()
    {
        m_camera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.LookAt(m_camera.transform);
        this.gameObject.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
    }
}
