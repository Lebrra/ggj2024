using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;
public class FerrisWHeel : MonoBehaviour
{

    [SerializeField] private Transform wheel;

    public void Update() {
        wheel.transform.Rotate(Vector3.forward * Time.deltaTime * 20);
    }

}
