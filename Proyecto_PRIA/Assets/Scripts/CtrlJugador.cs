using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CtrlJugador : MonoBehaviourPunCallbacks
{
    [Header("Caracteristicas")]
    [SerializeField] private float velocidad;
    [SerializeField] private float velocidadGiro;

    [Header("Disparo")]
    [SerializeField] private Transform puntoDisparo;
    bool puedoDisparar = true;
    LineRenderer lineaDisparo;

    [SerializeField] private GameObject hitEfectoDisparo_Prefab; // para el método Disparar_2

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto;


    [Header("Vida")]
    [SerializeField] private int maxVidas = 3;
    int vida;


    Rigidbody rig;
    Animator anim;

    // para el CHAT
    string nombre;
    bool habilitarMovimiento ; 


    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        lineaDisparo = puntoDisparo.GetComponent<LineRenderer>();

        vida = maxVidas;


 

        if (photonView.IsMine)
        {
            //Desplazo el personaje
            this.transform.position = new Vector3(UnityEngine.Random.Range(-2, 2), 0,
                UnityEngine.Random.Range(-2, 2));

            //Coloco la cámara
            Camera.main.transform.SetParent(this.transform);
            Camera.main.transform.position += this.transform.position;// + new Vector3(0, 2, -2.5f);

        }



        // para el chat
        habilitarMovimiento = true;
        //GetComponent<CtrlChat>().Conectarse();
    }

    //--------------------------------------------------------------------------------------------
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Debug.Log($"===> Habilitar movimiento: {habilitarMovimiento}");
            // if implementado para el CHAT
            if (HabilitarMovimiento)
            {
                #region Movimiento
                rig.velocity = transform.forward * Input.GetAxis("Vertical") * velocidad +
                                transform.right * Input.GetAxis("Horizontal") * velocidad +
                                transform.up * rig.velocity.y;

                transform.Rotate(transform.up * Input.GetAxis("Mouse X") * velocidadGiro);
                #endregion



                #region Disparo
                /*
                if (puedoDisparar && Input.GetButton("Fire1")) // Ctrl Izquierdo
                {
                    StartCoroutine("Disparar");
                }
                */
                
                if (Input.GetButtonDown("Fire1")) // ctrl izquierda
                {
                    Disparar_2();
                }
                

                #endregion


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Saltar();
                }


                // Actualizar animación con la velocidad del rigidbody
                anim.SetFloat("velocidad", new Vector3(rig.velocity.x, 0, rig.velocity.z).magnitude);
                anim.SetFloat("velocidadY", new Vector3(0, rig.velocity.y, 0).magnitude);


            } // fin if Habilitar Movimiento
        } // fin if IsMine

        
    }


    //--------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    private void Saltar()
    {
        Ray rayo = new Ray(transform.position + new Vector3(0, 0.2f, 0), Vector3.down);
        if( Physics.Raycast( rayo, 0.4f))
        {
            anim.SetTrigger("salto"); // nombre del trigger de la animación
            rig.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }


    //--------------------------------------------------------------------------------------------
    /// <summary>
    /// Corrutina para el disparo
    /// </summary>
    /// <returns></returns>

    IEnumerator Disparar()
    {
        puedoDisparar = false;

        // El disparo va a ser un rayo (RaycastHit)
        RaycastHit hit = new RaycastHit();


        //lineaDisparo.enabled = true;
        //lineaDisparo.SetPosition(0, puntoDisparo.position); // posición 0 de la línea

        // el rayo irá desde el puntoDisparo hacia delante
        if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out hit, 50))
        {
            // si choca con algo, el disparo se dibuja hasta la posición de choque

            // lineaDisparo.SetPosition(1, hit.point);

            Debug.DrawLine(puntoDisparo.position, hit.point, Color.red, 1); // el disparo dura 1 segundo

        }
        else
        {
            // lineaDisparo.SetPosition(1, puntoDisparo.forward * 50);

            Debug.DrawLine(puntoDisparo.position, puntoDisparo.forward * 50, Color.red, 1);
        }

        yield return new WaitForSeconds(0.3f); // cuando pase 0,3 segundos se desactiva
        lineaDisparo.enabled = false;

        yield return new WaitForSeconds(0.5f); // Espera de tiempo hasta que se pueda volver a disparar
        puedoDisparar = true;
    }


    //--------------------------------------------------------------------------------------------
    private void Disparar_2()
    {
        puedoDisparar = false;

        Debug.Log("Estoy en el metodo diparar 2");

        RaycastHit hit = new RaycastHit();

        // el punto de disparo va hacia delante
        if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out hit, 50))
        {
            //Debug.DrawLine(puntoDisparo.position, hit.point, Color.red, 1); // el disparo dura un segundo 1

            //Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CrearEfectoDisparo", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("HacerDanio", RpcTarget.All, 1);
            }
        }

        puedoDisparar = true;
    }


    //--------------------------------------------------------------------------------------------
    [PunRPC]
    public void CrearEfectoDisparo(Vector3 _posicion)
    {
        Debug.Log("Estoy en el efecto disparo");
        GameObject hitEfecto = Instantiate(hitEfectoDisparo_Prefab, _posicion, Quaternion.identity);
        Destroy(hitEfecto, 0.3f); // destruir el efecto despues de 3 segundos
    }



    //--------------------------------------------------------------------------------------------
    [PunRPC]
    public void HacerDanio(int _quitarVida, PhotonMessageInfo _info)
    {

        vida -= _quitarVida;


        Debug.Log("-->    Vidas: " + vida);

        if (vida <= 0)
        {
            // se muere
            anim.SetTrigger("muerte");

            // por si resucita

            rig.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            this.enabled = false;

            Debug.Log(_info.Sender.NickName + " ha matado a " + _info.photonView.Owner.NickName);
        }
    }


    //--------------------------------------------------------------------------------------------
    // Métodos para el CHAT
    //--------------------------------------------------------------------------------------------
    public bool HabilitarMovimiento
    {
        get { return habilitarMovimiento; }
        set { habilitarMovimiento = value; }
    }

    //--------------------------------------------------------------------------------------------
    public string Nombre
    {
        get { return (nombre = photonView.Owner.NickName);  }
        set { nombre = value; }
    }
}
