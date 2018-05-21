using UnityEngine;
using System.Collections;

public class Spin_Reverse : MonoBehaviour
{
    public float speed = 10f;


    void Update()
    {
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
    }
}