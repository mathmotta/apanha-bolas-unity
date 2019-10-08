using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pontuacao : MonoBehaviour
{
    [SerializeField]
    private int _pontos;
    void Start()
    {
        _pontos = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        _pontos++;

    }
}
