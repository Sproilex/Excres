using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    [Header("Parametros de Informacion")]
    [SerializeField]
    float VelocidadCaminar;
    [SerializeField]
    float VelocidadCorrer;
    private float velocidadActual;

    [Header("Objetos Necesarios")]
    [SerializeField]
    Image BarraVida;

    private int Vida = 100;

    private Camera Camara;

    Vector3 direccionFrontal, direccionLateral;

    void Start()
    {
        Camara = Camera.main;
        direccionFrontal = Camara.transform.forward;
        direccionFrontal.y = 0;
        direccionFrontal = Vector3.Normalize(direccionFrontal);
        direccionLateral = Quaternion.Euler(new Vector3(0, 90, 0)) * direccionFrontal;
    }

    void Update()
    {
        ManejadorTeclas.VerificarInput();
        if (ManejadorTeclas.IntentandoMover)
            StartCoroutine(UpdateControlador());
    }

    IEnumerator UpdateControlador()
    {

        ManejadorTeclas.VerificarInput();

        if (ManejadorTeclas.PresionandoCorrer)
        {
            velocidadActual = VelocidadCorrer;
            //BarraEnergia.gameObject.SetActive(true);
        }
        else
        {
            velocidadActual = VelocidadCaminar;
           // BarraEnergia.gameObject.SetActive(false);
        }

        Vector3 movimientoLateral = direccionLateral * velocidadActual * Time.deltaTime * (ManejadorTeclas.PresionandoDerecha ? 1 : ManejadorTeclas.PresionandoIzquierda ? -1 : 0);
        Vector3 movimientoFrontal = direccionFrontal * velocidadActual * Time.deltaTime * (ManejadorTeclas.PresionandoAdelante ? 1 : ManejadorTeclas.PresionandoAtras ? -1 : 0);

        Vector3 vectorarreglado = movimientoLateral + movimientoFrontal;
        vectorarreglado = new Vector3(vectorarreglado.x / 2,0,vectorarreglado.z / 2);

        Vector3 direccionMirar = Vector3.Normalize(vectorarreglado);

        transform.forward = direccionMirar;
        transform.position += vectorarreglado;

        yield return new WaitForSeconds(.5f);
    }

    public void BajarVida(int cantidad)
    {
        Vida -= cantidad;
        ManejarBarra();
    }

    public void ManejarBarra()
    {
        BarraVida.fillAmount = Vida / 100f;
    }
}
