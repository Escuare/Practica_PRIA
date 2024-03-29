using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JugadorFrutas : MonoBehaviourPunCallbacks
{
    [Header ("Movimiento")]
    public float horizontalInput, verticalInput;
    public float speed = 10f;
    private float limiteX = 14f;
    private float limiteZ = 7f;


    [Header("Fruta")]
    public bool tieneFrutaEnMano;
    public Transform posFrutaEnMano;
    private GameObject frutaEnMano;


    [SerializeField] private TextMeshPro nombreJugador_Cabeza;

    // Start is called before the first frame update
    void Start()
    {
        nombreJugador_Cabeza.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Movimiento();

        }

    }

    private void Movimiento()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 direccionMovimiento = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);

        if (direccionMovimiento != Vector3.zero)
        {
            transform.forward = direccionMovimiento;
        }

        //TEXTO FIJO
        nombreJugador_Cabeza.transform.rotation = new Quaternion(0, 180, 0, 0);

        //L�MITE//
        if (transform.position.x > limiteX)
            transform.position = new Vector3(limiteX, transform.position.y, transform.position.z);
        if (transform.position.x < -limiteX)
            transform.position = new Vector3(-limiteX, transform.position.y, transform.position.z);
        if (transform.position.z > limiteZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, limiteZ);
        if (transform.position.z < -limiteZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, -limiteZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fruta") && !tieneFrutaEnMano)
        {
            Debug.Log("He cogido una fruta " + other.gameObject.GetComponent<ScrFruta>().nombreFruta);
            tieneFrutaEnMano = true;

            //CREA UNA COPIA DE LA FRUTA RECOGIDAY LE ELIMINA EL SCRIPT, PARA QUE SEA SOLO LA IMAGEN
            frutaEnMano = Instantiate(other.gameObject, posFrutaEnMano);
            Destroy(frutaEnMano.GetComponent<ScrFruta>());

            //ESTABLECE LA POSICI�N EN EL TRANSFORM QUE HAY ENCIMA DEL PERSONAJE
            frutaEnMano.transform.SetParent(posFrutaEnMano);
            frutaEnMano.transform.localPosition = Vector3.zero;

            //QUITAMOS EL COLLIDER, POR SI ACASO.
            frutaEnMano.GetComponent<Collider>().enabled = false;

            //DESTRUYE LA FRUTA DEL ESCENARIO
            Destroy(other.gameObject);

        } 
        if (other.gameObject.CompareTag("Cacahuete") && !tieneFrutaEnMano)
        {
            other.gameObject.GetComponent<Collider>().enabled = false;
            Destroy(other.gameObject);
            Debug.Log("Cacahuete destruido");
            GameObject.Find("_GameManager").GetComponent<GameManager>().SumarPuntos(-1);
        }
        if(other.gameObject.CompareTag("Objetivo") && tieneFrutaEnMano)
        {
            tieneFrutaEnMano = false;
            //UNA VEZ ENTREGADA LA FRUTA, SE ELIMINA PARA PODER COGER OTRA
            GameObject.Find("SonidoPunto").GetComponent<AudioSource>().Play();
            Destroy(frutaEnMano);
            GameObject.Find("_GameManager").GetComponent<GameManager>().SumarPuntos(1);
        }
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //MANDA DATOS A LOS DEM�S JUGADORES
        {

            //DATOS DE LA POSICION AL SERVIDOR
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            //ENVIA QUE FRUTA HA RECOGIDO EL USUARIO
            stream.SendNext(tieneFrutaEnMano);

            if (tieneFrutaEnMano) //SI TIENE UNA FRUTA
            {
                //POSIC��N DE LA FRUTA EN LA CABEZA
                stream.SendNext(frutaEnMano.transform.position);
                stream.SendNext(frutaEnMano.transform.rotation);
            }
        }
        else
        {
            //RECIBE DATOS DE LOS DEM�S JUGADORES
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            //RECIBE LAS FRUTAS QUE RECOGEN LOS DEM�S JUGADORES
            tieneFrutaEnMano = (bool)stream.ReceiveNext();

            if (tieneFrutaEnMano)
            {
                //FRUTA EN LA CABEZA DE LOS DEM�S JUGADORES
                Vector3 frutaPos = (Vector3)stream.ReceiveNext();
                Quaternion frutaRot = (Quaternion)stream.ReceiveNext();

                //POSICI�N DE ESAS FRUTAS
                if (frutaEnMano != null)
                {
                    frutaEnMano.transform.position = frutaPos;
                    frutaEnMano.transform.rotation = frutaRot;
                }
            }
        }
    }
}
