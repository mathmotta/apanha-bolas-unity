using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsferaComp : MonoBehaviour
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(transform.position.y > 0.3f)
        {
            _rb.MovePosition(new Vector3(transform.position.x, 0.3f, transform.position.z));
        }
    }
}
