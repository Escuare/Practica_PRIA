using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Prefabs Jugadores")]
    [SerializeField] private GameObject[] prefabsJugadores;
    [SerializeField] private Transform[] spawnJugador;

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
    private float tiempo = 60f;
    private float tiempoInicio = 3f;

    [Header("Puntos")]
    public int puntos = 0;
    public bool puntosDobles = false;


    // Start is called before the first frame update
    void Start()
    {
        if( PhotonNetwork.IsConnected)
        {
            object avatarJugador = PhotonNetwork.LocalPlayer.CustomProperties["avatar"];

            Transform transforJugador = spawnJugador[(PhotonNetwork.LocalPlayer.ActorNumber)-1];
            Vector3 posSpawnJugador = new Vector3(transforJugador.position.x, transforJugador.position.y, transforJugador.position.z);

            PhotonNetwork.Instantiate( prefabsJugadores[(int) avatarJugador].name,
                posSpawnJugador, new Quaternion(0, 180, 0, 0), 0 );

            //PAUSAR PARA LA CUENTA ATRÁS
            Time.timeScale = 0f;
            StartCoroutine(IniciarCuentaAtras());

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempo > 1 && juegoOn)
        {
            //TEMPORIZADOR CUENTA ATRÁS
            tiempo -= Time.deltaTime;
            txtTiempoVar.text = Mathf.FloorToInt(tiempo % 60).ToString();

        }
        else if (tiempo < 1 && juegoOn)
        {
            AcabarJuego();
        }

        if (tiempo < 16 && !puntosDobles)
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
        photonView.RPC("ActualizarTextoPuntos", RpcTarget.AllBuffered, this.puntos);
    }

    [PunRPC]
    private void ActualizarTextoPuntos(int puntosActualizados)
    {
        // Actualiza el texto en todos los clientes
        txtPuntos.text = puntosActualizados.ToString();
    }



    private void AcabarJuego()
    {
        juegoOn = false;
        spawnFrutas.SetActive(false);
        GameObject[] todasLasFrutas = GameObject.FindGameObjectsWithTag("Fruta");
        //DESTRUYE LAS FRUTAS RESTANTES
        foreach (GameObject fruta in todasLasFrutas)
        {
            Destroy(fruta);
        }
        GameObject[] todosLosCacahuetes = GameObject.FindGameObjectsWithTag("Cacahuete");
        //DESTRUYE LOS CACAHUETES RESTANTES
        foreach (GameObject cacahuete in todosLosCacahuetes)
        {
            Destroy(cacahuete);
        }
        GameObject.Find("Objetivo").SetActive(false);

        PhotonNetwork.LoadLevel(0);


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
        //REANUDA EL JUEGO
        Time.timeScale = 1f;

    }


    #endregion
}
