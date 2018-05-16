using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour {

    [SerializeField]
    private bool EsPosibleEscalar;
    [SerializeField]
    private float OffsetEscalar;
    [SerializeField]
    private GameObject PosicionadorSubida;

    private void OnTriggerEnter(Collider other)
    {
        if (EsPosibleEscalar && other.tag == "Player")
        {
            Jugador jugador = other.GetComponent<Jugador>();
            jugador.AsignarObjetivoEscalada(this.transform.position, PosicionadorSubida.transform.position, OffsetEscalar);
        }
    }
}
