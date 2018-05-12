using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    [Header("Parametros de Informacion")]
    [SerializeField]
    float VelocidadCaminar;
    [SerializeField]
    float VelocidadCansancio;
    [SerializeField]
    float VelocidadCorrer;
    [SerializeField]
    int CantidadEnergia;
    [SerializeField]
    int Vida = 100;
    [SerializeField]
    [Tooltip("En segundos")]
    float TiempoEsperaSubirEnergia = 1;
    [SerializeField]
    float CantidadSubirEnergia = .5f;
    float cantidadEnergiaActual;
    [SerializeField]
    float RangoDetectarColision;
    private float velocidadActual;
    private bool EsPosibleCorrer = true;

    [Header("Objetos Necesarios")]
    [SerializeField]
    Image BarraVida;
    [SerializeField]
    Image BarraEnergia;
    GameObject PadreBarraEnergia;
    [SerializeField]
    GameObject ObjetoJugador;

    private Camera Camara;

    Vector3 direccionFrontal, direccionLateral;

    void Start()
    {
        Camara = Camera.main;
        direccionFrontal = Camara.transform.forward;
        direccionFrontal.y = 0;
        direccionFrontal = Vector3.Normalize(direccionFrontal);
        direccionLateral = Quaternion.Euler(new Vector3(0, 90, 0)) * direccionFrontal;
        cantidadEnergiaActual = CantidadEnergia;
        PadreBarraEnergia = BarraEnergia.transform.parent.gameObject;
    }

    void Update()
    {
        ManejadorPartida.VerificarInput();
        if (ManejadorPartida.IntentandoMover)
            StartCoroutine(UpdateControlador());
    }

    IEnumerator UpdateControlador()
    {
        if (ManejadorPartida.PresionandoCorrer && EsPosibleCorrer)
        {
            velocidadActual = VelocidadCorrer;
            cantidadEnergiaActual -= .5f;
        }
        else if (!EsPosibleCorrer)
            velocidadActual = VelocidadCansancio;
        else
        {
            velocidadActual = VelocidadCaminar;
        }

        StartCoroutine(ControlarBarraEnergia(ManejadorPartida.PresionandoCorrer));

        Vector3 movimientoLateral = direccionLateral * velocidadActual * Time.deltaTime * (ManejadorPartida.PresionandoDerecha ? 1 : ManejadorPartida.PresionandoIzquierda ? -1 : 0);
        Vector3 movimientoFrontal = direccionFrontal * velocidadActual * Time.deltaTime * (ManejadorPartida.PresionandoAdelante ? 1 : ManejadorPartida.PresionandoAtras ? -1 : 0);

        Vector3 vectorarreglado = movimientoLateral + movimientoFrontal;
        vectorarreglado = new Vector3(vectorarreglado.x / 2, 0, vectorarreglado.z / 2);

        Vector3 direccionMirar = Vector3.Normalize(vectorarreglado);

        ObjetoJugador.transform.forward = direccionMirar;

        Ray rayo = new Ray();
        rayo.direction = ObjetoJugador.transform.forward;
        rayo.origin = ObjetoJugador.transform.position;

        if (!Physics.Raycast(rayo,RangoDetectarColision))
            ObjetoJugador.transform.parent.position += vectorarreglado;

        yield return new WaitForSeconds(1);
    }

    IEnumerator ControlarBarraEnergia(bool bajarBarra)
    {
        BarraEnergia.fillAmount = cantidadEnergiaActual / CantidadEnergia;

        if (cantidadEnergiaActual == 0)
            EsPosibleCorrer = false;

        if (bajarBarra)
            PadreBarraEnergia.SetActive(true);
        else
        {
            if (cantidadEnergiaActual < CantidadEnergia)
            {
                yield return ManejadorPartida.Esperar(TiempoEsperaSubirEnergia);
                cantidadEnergiaActual += CantidadSubirEnergia;
                StartCoroutine(ControlarBarraEnergia(false));
            }
            else
            {
                cantidadEnergiaActual = CantidadEnergia;
                EsPosibleCorrer = true;
                PadreBarraEnergia.SetActive(false);
            }
        }

        yield return null;
    }

    public void BajarVida(int cantidad)
    {
        Vida -= cantidad;
        ManejarBarraVida();
    }

    public void ManejarBarraVida()
    {
        BarraVida.fillAmount = Vida / 100f;
    }
}
