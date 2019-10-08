using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JogadorComp : MonoBehaviour
{
    [SerializeField]
    private float _velocidade = 1f;

    private Rigidbody _rb;
    private float _objZPos;
    private float _movZ;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        _objZPos = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
    }

    private void OnMouseDrag()
    {
        var mousePoint = Input.mousePosition;
        mousePoint.z = _objZPos;
        _movZ = Camera.main.ScreenToWorldPoint(mousePoint).z;
    }

    void FixedUpdate()
    {
        var x = (_velocidade * Time.fixedDeltaTime) * -1;
        //transform.Translate(new Vector3(x, 0, 0));
        var novaPos = transform.position + new Vector3(x, 0, 0);
        if (_movZ > 0.9f)
            novaPos.z = 0.9f;
        else if (_movZ < -1.9f)
            novaPos.z = -1.9f;
        else
            novaPos.z = _movZ;
        _rb.MovePosition(novaPos);
    }
}
