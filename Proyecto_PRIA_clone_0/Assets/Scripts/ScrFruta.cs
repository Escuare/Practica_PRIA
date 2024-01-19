using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScrFruta : MonoBehaviour
{
    public String nombreFruta;

    [Header ("Flotación")]
    public float rotateSpeed = 0.1f;
    public AnimationCurve myCurve;

    [Header("Tiempo de vida")]
    private Coroutine tiempoVida;

    // Start is called before the first frame update
    void Start()
    {
        tiempoVida = StartCoroutine(TiempoVida());
    }

    // Update is called once per frame
    void Update()
    {

        //PARA QUE GIRE SOBRE SU PROPIO EJE
        rotateSpeed += 0.1f;
        transform.rotation = Quaternion.Euler(-90, rotateSpeed, 0);
        transform.position = new Vector3(transform.position.x, myCurve.Evaluate((Time.time % myCurve.length)), transform.position.z);

    }

    IEnumerator TiempoVida()
    {
        float tiempoRandom = Random.Range(5, 8);
        yield return new WaitForSeconds(tiempoRandom);
        Destroy(gameObject);
    }

}
