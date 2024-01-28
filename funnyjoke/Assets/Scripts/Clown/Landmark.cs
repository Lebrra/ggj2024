using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    public SphereCollider collider;
    public void Awake() {
        collider = this.GetComponent<SphereCollider>();
    }

    public Vector3 GetPointFromLandmark() {
        return (UnityEngine.Random.insideUnitSphere * collider.radius) + transform.position;
    }


}
