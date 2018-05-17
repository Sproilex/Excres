using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManejadorPartida {

    private static KeyCode TeclaMoverAdelante = KeyCode.W;
    private static KeyCode TeclaMoverAtras = KeyCode.S;
    private static KeyCode TeclaMoverIzquierda = KeyCode.A;
    private static KeyCode TeclaMoverDerecha = KeyCode.D;
    private static KeyCode TeclaCorrer = KeyCode.LeftShift;
    private static KeyCode TeclaEscalar = KeyCode.Space;
    private static KeyCode TeclaInteractuar = KeyCode.E;

    public static bool PresionandoAdelante;
    public static bool PresionandoAtras;
    public static bool PresionandoIzquierda;
    public static bool PresionandoDerecha;
    public static bool PresionandoCorrer;
    public static bool PresionandoEscalar;
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
        PresionandoEscalar = Input.GetKey(TeclaEscalar);
        PresionandoInteractuar = Input.GetKey(TeclaInteractuar);
    }

    public static IEnumerator Esperar(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
    }

}
