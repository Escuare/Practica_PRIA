using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlJugador : MonoBehaviour
{

    public float horizontalInput, verticalInput;
    public float speed = 10f;

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
}
