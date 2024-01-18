using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CtrlJugador : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        //transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
        verticalInput = Input.GetAxis("Vertical");
        // transform.Translate(Vector3.forward * verticalInput * Time.deltaTime *speed);

        Vector3 direccionMovimiento = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
        //MUEVE EL PERSONAJE
        transform.Translate(direccionMovimiento * speed * Time.deltaTime, Space.World);

        //SI LA DIRECCION NO ES CERO
        if (direccionMovimiento != Vector3.zero)
        {
            //SE MUEVE HACIA DELANTE
            transform.forward = direccionMovimiento;
        }

        //LÍMITE//
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

            //ESTABLECE LA POSICIÓN EN EL TRANSFORM QUE HAY ENCIMA DEL PERSONAJE
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
            GameObject.Find("_GameManager").GetComponent<GmManager>().SumarPuntos(-1);
        }
        if(other.gameObject.CompareTag("Objetivo") && tieneFrutaEnMano)
        {
            tieneFrutaEnMano = false;
            //UNA VEZ ENTREGADA LA FRUTA, SE ELIMINA PARA PODER COGER OTRA
            GameObject.Find("SonidoPunto").GetComponent<AudioSource>().Play();
            Destroy(frutaEnMano);
            GameObject.Find("_GameManager").GetComponent<GmManager>().SumarPuntos(1);
        }
    }
}
