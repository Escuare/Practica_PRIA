using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GmManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject spawnFrutas;

    [Header("Canvas")]
    public TextMeshProUGUI txtPuntos;
    public TextMeshProUGUI txtPuntosDobles;
    public TextMeshProUGUI txtTiempoVar;
    public TextMeshProUGUI txtTiempoPausa;
    public GameObject panelPausa;

    [Header("Tiempo")]
    private bool juegoOn = false;
    private float tiempo = 12f;
    private float tiempoInicio = 3f;

    [Header("Puntos")]
    public int puntos = 0;
    public bool puntosDobles = false;

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
            //TEMPORIZADOR CUENTA ATR�S
            tiempo -= Time.deltaTime;
            txtTiempoVar.text = Mathf.FloorToInt(tiempo % 60).ToString();

        } 
        else if (tiempo < 1 && juegoOn)
        {
            AcabarJuego();
        }

        if (tiempo < 11 && !puntosDobles)
        {
            puntosDobles = true;
            Debug.Log("Puntos dobles");
            txtPuntosDobles.gameObject.SetActive(true);
        }
    }

    #region METODOS

    public void SumarPuntos(int puntos)
    {
        if (puntosDobles && puntos > 0)
            puntos *= 2;
        this.puntos += puntos;
        Debug.Log(this.puntos);
        txtPuntos.text = this.puntos.ToString();
    }

    

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
        GameObject.Find("Objetivo").SetActive(false);
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