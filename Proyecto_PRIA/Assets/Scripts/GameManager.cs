using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Prefabs Jugadores")]
    [SerializeField] private GameObject[] prefabsJugadores;
    [SerializeField] private Transform[] spawnJugador;


    private GameObject jugador;

    // Start is called before the first frame update
    void Start()
    {
        if( PhotonNetwork.IsConnected)
        {
            object avatarJugador = PhotonNetwork.LocalPlayer.CustomProperties["avatar"];

            Transform transforJugador = spawnJugador[(PhotonNetwork.LocalPlayer.ActorNumber)-1];
            Vector3 posSpawnJugador = new Vector3(transforJugador.position.x, transforJugador.position.y, transforJugador.position.z);

            jugador = PhotonNetwork.Instantiate( prefabsJugadores[(int) avatarJugador].name,
                posSpawnJugador, Quaternion.identity, 0 );

            // seria el momento de ponerla como hija del personaje
            //Camera.main.transform.SetParent(jugador.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
