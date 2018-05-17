using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour {

    [SerializeField]
    private bool EsPosibleEscalar;
    [SerializeField]
    private TipoEscalar TipoObjeto;
    [SerializeField]
    private bool NoMoverAlSubir;

    private void OnTriggerStay(Collider other)
    {
        if(EsPosibleEscalar && other.tag == "Player" && ManejadorPartida.PresionandoEscalar)
        {
            Jugador jugador = other.GetComponent<Jugador>();
            jugador.IniciarEscalada(transform,TipoObjeto,NoMoverAlSubir);
        }
    }
}
