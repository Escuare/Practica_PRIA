using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] prefabJugador;

    private GameObject jugador;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            object avatarJugador = PhotonNetwork.LocalPlayer.CustomProperties["avatar"];
            Debug.Log((int)avatarJugador);
            jugador = PhotonNetwork.Instantiate(prefabJugador[(int)avatarJugador].name, 
                new Vector3(0, 1, 0), Quaternion.identity, 0);
            //Camera.main.transform.SetParent(jugador.transform);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

}
