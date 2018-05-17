using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum TipoEscalar { Arbol = 1, Roca = 2 };

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
    [SerializeField]
    Vector3 DistanciaMantenerCamara;
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
    private bool estaEscalando = false;
    private bool BloquearMovimiento = false;
    private bool estaArriba = false;

    //Componentes
    private Rigidbody rigidbodyJugador;
    private Animator animatorJugador;

    void Start()
    {
        Camara = Camera.main;
        direccionFrontal = Camara.transform.forward;
        direccionFrontal.y = 0;
        direccionFrontal = Vector3.Normalize(direccionFrontal);
        direccionLateral = Quaternion.Euler(new Vector3(0, 90, 0)) * direccionFrontal;
        cantidadEnergiaActual = CantidadEnergia;
        PadreBarraEnergia = BarraEnergia.transform.parent.gameObject;
        rigidbodyJugador = GetComponent<Rigidbody>();
        animatorJugador = GetComponent<Animator>();
    }

    void Update()
    {
        ManejadorPartida.VerificarInput();
        StartCoroutine(UpdateControlador());
    }

    IEnumerator UpdateControlador()
    {
        if (!estaEscalando && !BloquearMovimiento)
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
            this.transform.forward = direccionMirar;

            Ray rayo = new Ray();
            rayo.direction = this.transform.forward;
            rayo.origin = this.transform.position;

            if (!Physics.Raycast(rayo, RangoDetectarColision))
                this.transform.position += vectorarreglado;
        } //Caminar

        AgrupadorJugador.transform.position = transform.position + DistanciaMantenerCamara;

        yield return new WaitForSeconds(1);
    }

    IEnumerator EscalarJugador()
    {
        yield return new WaitForSeconds(.2f);
        if (ManejadorPartida.PresionandoEscalar && !estaArriba)
        {
            animatorJugador.SetBool("SubirObjeto",true);
        }
        else if (ManejadorPartida.PresionandoAtras && !estaArriba)
        {
            animatorJugador.SetBool("Soltarse", true);
            TerminarEscalar();
        }
        else if (ManejadorPartida.PresionandoEscalar && estaArriba)
        {
            animatorJugador.SetBool("BajarObjeto",true);
            TerminarEscalar();
        }

        if (estaEscalando)
            StartCoroutine(EscalarJugador());
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

    public void IniciarEscalada(Transform objeto, TipoEscalar tipoObstaculo,bool mantenerQuietoAlSubir)
    {
        if (!estaEscalando)
        {
            rigidbodyJugador.useGravity = false;
            rigidbodyJugador.isKinematic = true;
            animatorJugador.SetInteger("Agarrarse",(int)tipoObstaculo);
            estaEscalando = true;
            BloquearMovimiento = true;
            Vector3 direccionMirar = objeto.position - transform.position;
            this.transform.forward = direccionMirar;
            StartCoroutine(EscalarJugador());
        }
    }

    public void AlSubirEscalando()
    {
        animatorJugador.SetBool("SubirObjeto", false);
        estaArriba = true;
    }

    public void TerminarEscalar()
    {
        estaEscalando = false;
        rigidbodyJugador.useGravity = true;
        rigidbodyJugador.isKinematic = false;
        BloquearMovimiento = false;
        estaArriba = false;
    }

    public void LimpiarValoresAnimatorEscalada()
    {
        animatorJugador.SetInteger("Agarrarse",0);
        animatorJugador.SetBool("SubirObjeto",false);
        animatorJugador.SetBool("BajarObjeto", false);
        animatorJugador.SetBool("Soltarse",false);
    }
}
