using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class JugadorUI_Ctrl : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nombreJugador_Cabeza;

    // Start is called before the first frame update
    void Start()
    {
        nombreJugador_Cabeza.text = photonView.Owner.NickName;
    }

}
