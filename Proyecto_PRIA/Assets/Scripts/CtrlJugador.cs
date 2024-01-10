using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CtrlJugador : MonoBehaviour
{

    public float horizontalInput, verticalInput;
    public float speed = 10f;
    public bool frutaEnMano;
    public int puntos = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);

        verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * Time.deltaTime *speed);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fruta") && !frutaEnMano)
        {
            Debug.Log("He cogido una fruta " + other.gameObject.GetComponent<ScrFruta>().nombreFruta);
            frutaEnMano = true;
            Destroy(other.gameObject);
        }
        if(other.gameObject.CompareTag("Objetivo") && frutaEnMano)
        {
            frutaEnMano = false;
            puntos++;
            Debug.Log(puntos);
        }
    }
}
