using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ScrFruta : MonoBehaviour
{
    public String nombreFruta;
    public float rotateSpeed = 0.1f;
    private float swingSpeed = 1f;
    public AnimationCurve myCurve;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //PARA QUE GIRE SOBRE SU PROPIO EJE
        rotateSpeed += 0.1f;
        transform.rotation = Quaternion.Euler(-90, rotateSpeed, 0);
        transform.position = new Vector3(transform.position.x, myCurve.Evaluate((Time.time % myCurve.length)), transform.position.z);

    }

}
