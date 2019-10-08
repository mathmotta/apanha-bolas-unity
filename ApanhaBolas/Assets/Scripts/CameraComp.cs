using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComp : MonoBehaviour
{
    [SerializeField]
    private GameObject _jogador;

    private void LateUpdate()
    {
        transform.position = new Vector3(_jogador.transform.position.x + 4, transform.position.y, transform.position.z);

    }
}
