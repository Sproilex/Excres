using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManejadorTeclas {

    public static KeyCode TeclaMoverAdelante = KeyCode.W;
    public static KeyCode TeclaMoverAtras = KeyCode.S;
    public static KeyCode TeclaMoverIzquierda = KeyCode.A;
    public static KeyCode TeclaMoverDerecha = KeyCode.D;
    public static KeyCode TeclaCorrer = KeyCode.LeftShift;
    public static KeyCode TeclaInteractuar = KeyCode.Space;

    public static bool PresionandoAdelante;
    public static bool PresionandoAtras;
    public static bool PresionandoIzquierda;
    public static bool PresionandoDerecha;
    public static bool PresionandoCorrer;
    public static bool PresionandoInteractuar;
    public static bool IntentandoMover;

    public static void VerificarInput()
    {
        IntentandoMover = Input.GetKey(TeclaMoverAdelante) || Input.GetKey(TeclaMoverAtras) || Input.GetKey(TeclaMoverIzquierda) || Input.GetKey(TeclaMoverDerecha);
        PresionandoAdelante = Input.GetKey(TeclaMoverAdelante);
        PresionandoAtras = Input.GetKey(TeclaMoverAtras);
        PresionandoIzquierda = Input.GetKey(TeclaMoverIzquierda);
        PresionandoDerecha = Input.GetKey(TeclaMoverDerecha);
        PresionandoCorrer = Input.GetKey(TeclaCorrer);
        PresionandoInteractuar = Input.GetKey(TeclaInteractuar);
    }

}
