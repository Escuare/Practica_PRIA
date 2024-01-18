using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControlJugador : MonoBehaviourPunCallbacks
{
    [Header("Caracteristicas")]
    [SerializeField] private float velocidad;
    [SerializeField] private float velocidadGiro;

    #region DeclaracionSalto
    [Header("Salto")]
    [SerializeField] private float fuerzaSalto;
    #endregion

    #region DeclaracionDisparo
    [Header("Disparo")]
    [SerializeField] private Transform puntoDisparo;
    bool puedoDisparar = true;
    LineRenderer lineaDisparo;
    #endregion

    Rigidbody rigi;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        lineaDisparo = puntoDisparo.GetComponent<LineRenderer>();
    }

    // ---------------------------------------------------------------------------------------------
    // La cantidad de ejecuciones por segundo de Update es variable,
    // mientras que la cantidad de ejecuciones por segundo de FixedUpdate es fija
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            #region Movimiento
            Vector3 velocidadNormalizada = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            velocidadNormalizada = velocidadNormalizada.normalized;

            rigi.velocity = transform.forward * velocidadNormalizada.z * velocidad +
                           transform.right * velocidadNormalizada.x * velocidad +
                           transform.up * rigi.velocity.y;


            transform.Rotate(Input.GetAxis("Mouse X") * velocidadGiro * transform.up);
            #endregion

            #region Disparo
            if (puedoDisparar && Input.GetButton("Fire1"))  // Ctrl izquierdo
            {
                StartCoroutine("Fire2");
            }

            #endregion

            #region Salto
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Saltar();
            }

            #endregion

            anim.SetFloat("velocidad", rigi.velocity.magnitude);   // añadir a la animación la verlocidad del Rigidbody
            anim.SetFloat("velocidadY", new Vector3(0, rigi.velocity.y, 0).magnitude);
        }
    }


    //-------------------------------------------------------------------------------------------
    /// <summary>
    /// Hace saltar al personaje
    /// </summary>
    private void Saltar()
    {
        Ray rayo = new Ray(transform.position + new Vector3(0, 0.2f, 0), Vector3.down);
        if (Physics.Raycast(rayo, 0.4f))
        {
            anim.SetTrigger("salto"); // nombre del trigger de la animación
            rigi.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }


    //-------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Fire()
    {
        Debug.Log("Disparando");
        puedoDisparar = false;
        // El disparo va a ser un rayo (RaycastHit)
        RaycastHit hit = new RaycastHit();

        // el rayo irá desde el puntoDisparo hacia delante
        if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out hit, 50))
        {
            // si choca con algo, el disparo se dibuja hasta la posición de choque
            Debug.DrawLine(puntoDisparo.position, hit.point, Color.red, 1); // el disparo dura 1 segundo

        }
        else
        {
            Debug.DrawLine(puntoDisparo.position, puntoDisparo.forward * 50, Color.red, 1);
        }

        // Espera de tiempo hasta que se pueda volver a disparar
        yield return new WaitForSeconds(0.5f); 
        puedoDisparar = true;
    }

    //-------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    ///
    /// </summary>
    /// <returns></returns>
    IEnumerator Fire2()
    {
        puedoDisparar = false;
        // El disparo va a ser un rayo (RaycastHit) -- en debug
        RaycastHit hit = new RaycastHit();

        lineaDisparo.enabled = true;
        lineaDisparo.SetPosition(0, puntoDisparo.position);  // posición 0 de la línea

        // el rayo irá desde el puntoDisparo hacia delante
        if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out hit, 50))
        {
            // si choca con algo, el disparo se dibuja hasta la posición de choque
            lineaDisparo.SetPosition(1, hit.point);

            Debug.DrawLine(puntoDisparo.position, hit.point, Color.red, 1); // el disparo dura 1 segundo
        }
        else
        {
            lineaDisparo.SetPosition(1, puntoDisparo.forward * 50);
            Debug.DrawLine(puntoDisparo.position, puntoDisparo.forward * 50, Color.red, 1);
        }

        yield return new WaitForSeconds(0.3f); // cuando pase 0,3 segundos se desactiva
        lineaDisparo.enabled = false;

        yield return new WaitForSeconds(0.5f); // Espera de tiempo hasta que se pueda volver a disparar (0,5 segundos)
        puedoDisparar = true;
    }


}
