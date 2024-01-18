using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] prefabJugador;
    [SerializeField] private Transform[] spawnJugador;


    private GameObject jugador;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Actor Number " + PhotonNetwork.LocalPlayer.ActorNumber);
            object avatarJugador = PhotonNetwork.LocalPlayer.CustomProperties["avatar"];

            Transform transformJugador = spawnJugador[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Vector3 spawnPoint = new Vector3(transformJugador.position.x, transformJugador.position.y, transformJugador.position.z);
            jugador = PhotonNetwork.Instantiate(prefabJugador[(int)avatarJugador].name,
                spawnPoint, Quaternion.identity, 0);
            //Camera.main.transform.SetParent(jugador.transform);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

}
