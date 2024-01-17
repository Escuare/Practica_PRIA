using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject spawnFrutas;

    [Header ("Canvas")]
    public TextMeshProUGUI txtPuntosJug1;
    public TextMeshProUGUI txtPuntosJug2;
    public TextMeshProUGUI txtTiempoVar;
    public TextMeshProUGUI txtTiempoPausa;
    public GameObject panelPausa;

    [Header("Tiempo")]
    private bool juegoOn = false;
    private float tiempo = 60f;
    private float tiempoInicio = 3f;

    private void Awake()
    {
        // Pausa el juego
        Time.timeScale = 0f;
    }

    void Start()
    {
        StartCoroutine(IniciarCuentaAtras());
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempo > 1 && juegoOn)
        {
            //TEMPORIZADOR CUENTA ATRÁS
            tiempo -= Time.deltaTime;
            txtTiempoVar.text = Mathf.FloorToInt(tiempo % 60).ToString();

        } else if (tiempo < 1 && juegoOn)
        {
            AcabarJuego();
        }
    }

    #region METODOS

    private void AcabarJuego()
    {
        juegoOn = false;
        //CAMBIAR ESCENA
        spawnFrutas.SetActive(false);
        GameObject[] todasLasFrutas = GameObject.FindGameObjectsWithTag("Fruta");
        //DESTRUYE TODOS LOS CUBOS Y GANA 5 PUNTOS X CADA UNO DESTRUIDO.
        foreach (GameObject fruta in todasLasFrutas)
        {
            Destroy(fruta);
        }
    }

    IEnumerator IniciarCuentaAtras()
    {

        while (tiempoInicio > 0)
        {
            txtTiempoPausa.text = tiempoInicio.ToString();
            yield return new WaitForSecondsRealtime(1f);
            tiempoInicio--;
        }

        panelPausa.SetActive(false);
        juegoOn = true;
        Time.timeScale = 1f;

    }


    #endregion
}
