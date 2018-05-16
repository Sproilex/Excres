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
    GameObject AgrupadorJugador;

    private Camera Camara;

    Vector3 direccionFrontal, direccionLateral;
    //Escalar
    [SerializeField]
    private float VelocidadEscalar;
    private bool estaEscalando;
    private float distanciaEscalada;
    private float nivelEscalada;
    private float offsetEscalada;
    private Vector3 posicionLLegarEscalada;
    private Vector3 posicionInicial;

    private Rigidbody rigidbody;

    void Start()
    {
        Camara = Camera.main;
        direccionFrontal = Camara.transform.forward;
        direccionFrontal.y = 0;
        direccionFrontal = Vector3.Normalize(direccionFrontal);
        direccionLateral = Quaternion.Euler(new Vector3(0, 90, 0)) * direccionFrontal;
        cantidadEnergiaActual = CantidadEnergia;
        PadreBarraEnergia = BarraEnergia.transform.parent.gameObject;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ManejadorPartida.VerificarInput();
        if (ManejadorPartida.IntentandoMover)
            StartCoroutine(UpdateControlador());
    }

    IEnumerator UpdateControlador()
    {
        if (!estaEscalando)
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

            ColocarForward(direccionMirar);

            Ray rayo = new Ray();
            rayo.direction = this.transform.forward;
            rayo.origin = this.transform.position;

            if (!Physics.Raycast(rayo, RangoDetectarColision))
                this.transform.position += vectorarreglado;
        } //Caminar

        yield return new WaitForSeconds(1);
    }

    IEnumerator EscalarJugador()
    {
        yield return new WaitForSeconds(.01f);
        nivelEscalada += Mathf.Abs(.01f / (distanciaEscalada)) * VelocidadEscalar;
        Debug.Log(nivelEscalada);
        transform.position = Vector3.Lerp(posicionInicial, posicionLLegarEscalada, nivelEscalada);

        if (nivelEscalada < 1)
            StartCoroutine(EscalarJugador());
        else
        {
            Vector3 agregarForward = new Vector3(Mathf.Abs(transform.forward.x) == 1 ? offsetEscalada * (transform.forward.x.ToString().Contains("-") ? -1 : 1) : Mathf.Abs(transform.forward.x) > 0 ? offsetEscalada / 2 * (transform.forward.x.ToString().Contains("-") ? -1 : 1) : 0,
                                                 0,
                                                 Mathf.Abs(transform.forward.z) == 1 ? offsetEscalada * (transform.forward.z.ToString().Contains("-") ? -1 : 1) : Mathf.Abs(transform.forward.z) > 0 ? offsetEscalada / 2 * (transform.forward.z.ToString().Contains("-") ? -1 : 1) : 0);
            transform.position += transform.forward + agregarForward;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            distanciaEscalada = 0;
            nivelEscalada = 0;
            offsetEscalada = 0;
            posicionLLegarEscalada = Vector3.zero;
            posicionInicial = Vector3.zero;
            estaEscalando = false;
            yield return null;
        }
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

    public void AsignarObjetivoEscalada(Vector3 posicionObstaculo,Vector3 posicionObjetoLLegar,float offsetFinal)
    {
        if (!estaEscalando && rigidbody.velocity.y == 0)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            ColocarForward(posicionObstaculo - this.transform.position);
            posicionInicial = transform.position;
            posicionLLegarEscalada = new Vector3(transform.position.x, posicionObjetoLLegar.y,transform.position.z);
            distanciaEscalada = Vector3.Distance(posicionObjetoLLegar, this.transform.position);
            offsetEscalada = offsetFinal;
            estaEscalando = true;
            StartCoroutine(EscalarJugador());
        }
    }

    private void ColocarForward(Vector3 forwardColocar)
    {
        AgrupadorJugador.transform.SetParent(AgrupadorJugador.transform.parent.parent);
        this.transform.forward = forwardColocar;
        AgrupadorJugador.transform.SetParent(this.transform);
    }
}
