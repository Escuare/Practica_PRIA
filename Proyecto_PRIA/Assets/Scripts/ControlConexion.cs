using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;


using UnityEngine.SceneManagement;

public class ControlConexion : MonoBehaviourPunCallbacks
{

    #region Variables privadas

    [Header("Paneles")]
    [SerializeField] private GameObject panelConexion;
    [SerializeField] private GameObject panelBienvenida;
    [SerializeField] private GameObject panelCreacionSala;
    [SerializeField] private GameObject panelUnirseSala;
    [SerializeField] private GameObject panelDeSala;
    [SerializeField] private GameObject panelAvatar;


    [Header("Panel Conexi�n")]
    [SerializeField] private Text inputNombreJugador ;
    [SerializeField] private TextMeshProUGUI txtNombreJugador; // si no funciona el atributo anterior
    [SerializeField] private Button btnConectar;

    [Header("Panel Bienvenida")]
    [SerializeField] private TextMeshProUGUI txtBienvenida;

    [Header("Panel Barra de Estado")]
    [SerializeField] private TextMeshProUGUI txtBarraEstado;

    [Header("Panel Creaci�n Sala")]
    [SerializeField] private Text txtNombreSala;
    [SerializeField] private Text txtMinJugadores;
    [SerializeField] private Text txtMaxJugadores;
    [SerializeField] private Toggle tgVisibilidad;


    [Header("Panel Unirse a una Sala")]
    [SerializeField] private Text txtNombreSalaAUnirse;

    [SerializeField] private GameObject elemSala;
    [SerializeField] private GameObject contenedorSala;

    Dictionary<string, RoomInfo> listaSalas;


    [Header("Panel Sala con Jugadores")]
    [SerializeField] private TextMeshProUGUI txtNombreSalaPanelSala;
    [SerializeField] private TextMeshProUGUI txtCapacidadPanelSala;
    [SerializeField] private TextMeshProUGUI txtListadoJugadores;
    [SerializeField] private Button btnComenzarJuego;

    [SerializeField] private GameObject elemJugador;
    [SerializeField] private GameObject contenedorJugador;


    ExitGames.Client.Photon.Hashtable propiedadesJugador;

    #endregion

    [Header("Panel Avatar")]
    static public ControlConexion conex;
    public int avatarSeleccionado;

  

    // Start is called before the first frame update
    void Start()
    {
        
        conex = this;
        avatarSeleccionado = -1;

        listaSalas = new Dictionary<string, RoomInfo>();

        propiedadesJugador = new ExitGames.Client.Photon.Hashtable();

        ActivarPaneles(panelConexion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //-----------------------------------------------------------------------------------------------
    //                           C�digo OnClick  --- 
    //-----------------------------------------------------------------------------------------------
    #region CODIGO_BOTONES
    public void Pulsar_BtnConectar()
    {
        if( !string.IsNullOrEmpty( inputNombreJugador.text) 
            && !string.IsNullOrWhiteSpace(inputNombreJugador.text))

        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.NickName = inputNombreJugador.text;

            Estado("Conectando a Photon");
        }
        else
        {
            Estado("Introduzca un nombre de jugador v�lido");
        }
    }

    //----------------------------------------------------------------------------------------
    public void Pulsar_BtnCrearNuevaSala()
    {
        ActivarPaneles(panelCreacionSala);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Al pulsar el bot�n Conectar a sala desde el panel de Bienvenida
    /// </summary>
    public void Pulsar_BtnUnirseSala()
    {
        ActivarPaneles( panelUnirseSala);
    }

    //----------------------------------------------------------------------------------------
    public void Pulsar_BtnCreacionSala()
    {
        byte minJugadores;
        byte maxJugadores;

        
        minJugadores = byte.Parse(txtMinJugadores.text);
        maxJugadores = byte.Parse(txtMaxJugadores.text);

        if( !string.IsNullOrEmpty( txtNombreSala.text ) ) 
        { 
            if( !(minJugadores > maxJugadores || maxJugadores > 20)  
                || minJugadores > 20 || maxJugadores < 2
                || minJugadores < 2 )
            {
                RoomOptions opcionesSala = new RoomOptions();

                opcionesSala.MaxPlayers = maxJugadores;
                opcionesSala.IsVisible = !(tgVisibilidad.isOn);  //true

                Estado("Creando la nueva sala: " + txtNombreSala.text);

               
                PhotonNetwork.CreateRoom(txtNombreSala.text, 
                    opcionesSala, TypedLobby.Default);

                Estado( "Creando la nueva sala: " + txtNombreSala.text );
            }
            else
            {
                Estado("Valores de capacidad de sala incorrectos");
            }
        }
        else 
        {
            Estado("Introduzca un nombre de sala correcto.");
        }

    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Conectarse a una sala ya creada
    /// </summary>
    public void Pulsar_BtnUnirSala()
    {
        if (!string.IsNullOrEmpty(txtNombreSalaAUnirse.text))
        {
            PhotonNetwork.JoinRoom(txtNombreSalaAUnirse.text);
        }

        else
            Estado("Introduzca un nombre correcto para la sala");
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Salir del juego
    /// </summary>
    public void Pulsar_BtnSalir()
    {
        Application.Quit();
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Estamos en la pantalla de bienvenida, con lo cual
    /// debemos desconectar de Photon y volver a la pantalla 
    /// de conexi�n
    /// </summary>
    public void Pulsar_BtnDesconectar()
    {
        PhotonNetwork.Disconnect();
        ActivarPaneles(panelConexion);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Estamos en la pantalla de Creaci�n de Sala o de Unirse a la Sala
    /// y queremos volver a la pantalla de Bienvenida
    /// </summary>
    public void Pulsar_BtnVolverIntermedio()
    {
        ActivarPaneles(panelBienvenida);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Estamos en la pantalla de Empezar el juego y queremos
    /// volver a la pantalla de Bienvenida.
    /// Tenemos que abandonar la sala antes de volver
    /// </summary>
    public void Pulsar_BtnVolver()
    {
        PhotonNetwork.LeaveRoom();
        ActivarPaneles(panelBienvenida);
    }

    //----------------------------------------------------------------------------------------
    public void Pulsar_BtnAbandonar()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby(TypedLobby.Default); // unirse al lobby por defecto


        ActivarPaneles(panelBienvenida);
        

    }


    //----------------------------------------------------------------------------------------
    /// <summary>
    /// LLamada desde el bot�n Seleccionar  Avatar del panel de Bienvenida
    /// </summary>
    public void Pulsar_BtnSeleccionarAvatar()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); // unirse al lobby por defecto
        ActivarPaneles(panelAvatar);
    }
    //----------------------------------------------------------------------------------------
    public void Pulsar_BtnVolverDesdeAvatar()
    {
        ActivarPaneles(panelBienvenida);

        if (avatarSeleccionado >= 0)
        {
            Estado("Seleccionado avatar " + avatarSeleccionado);

            //Activamos los botones si tenemos un avatar seleccionado
            panelBienvenida.transform.Find("btnConectarSala").GetComponent<Button>().interactable = true;
            panelBienvenida.transform.Find("btnCrearSala").GetComponent<Button>().interactable = true;


            propiedadesJugador["avatar"] = avatarSeleccionado;

            PhotonNetwork.LocalPlayer.SetCustomProperties(propiedadesJugador);
        }
        else
        {
            Estado("No hay avatar seleccionado");
        }
    }

    //----------------------------------------------------------------------------------------
    public void Pulsar_BtnSeleccionarAvatar_Avatar()
    {
       
    }


    #endregion


    //-----------------------------------------------------------------------------------------------
    //                           CALLBACKS 
    //-----------------------------------------------------------------------------------------------
    #region callbacks
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Estado("Conectado a Photon");
        

        ActivarPaneles(panelBienvenida);

        txtBienvenida.text = "Bienvenido al juego: " + PhotonNetwork.NickName;

       // PhotonNetwork.JoinLobby(TypedLobby.Default); // unirse al lobby por defecto
    }

    //----------------------------------------------------------------------------------------
    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);

        Estado("Desconecado Photon: " + cause);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Se ejecuta si NO se ha creado la sala correctamente
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        Estado("No ha sido posible crear la sala: " + message);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Se ejecuta si se ha creado la Sala correctamente
    /// </summary>
    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        string mensaje = PhotonNetwork.NickName + " se ha conectado a " 
            + PhotonNetwork.CurrentRoom.Name ;

        Estado(mensaje);

        ActivarPaneles(panelDeSala);

        ActualizarPanelDeJugadores();

    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// NO ha podido conectar a una sala ya creada
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);
        Estado("No ha sido posible unirse a la sala: " + message);
    }


    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Conectado a una sala ya creada
    /// </summary>
    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        string mensaje = PhotonNetwork.NickName + " se ha unido a "
            + PhotonNetwork.CurrentRoom.Name;

        Estado(mensaje);

        ActivarPaneles(panelDeSala);
        ActualizarPanelDeJugadores();
    }

    //----------------------------------------------------------------------------------------
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        ActualizarPanelDeJugadores();
    }


    //----------------------------------------------------------------------------------------
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        ActualizarPanelDeJugadores();
    }


    //----------------------------------------------------------------------------------------
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);

        // borrar la sala de la lista que no se encuentra o no es visible en este momento
        foreach (RoomInfo sala in roomList)
        {
            if (sala.RemovedFromList || !sala.IsOpen || !sala.IsVisible)
            {
                listaSalas.Remove(sala.Name);
            }

            // comprobando que la sala se ha modificado
            if (listaSalas.ContainsKey(sala.Name))
            {
                if (sala.PlayerCount > 0)
                    listaSalas[sala.Name] = sala;

                else // si se ha quedado sin jugadores, la borramos
                    listaSalas.Remove(sala.Name);
            }
            else // es una nueva sala
                if (sala.PlayerCount > 0)
                    listaSalas.Add(sala.Name, sala);

        }

        ActualizarPanelUnirseASala();
        //photonView.RPC("EliminarSalasViejas", RpcTarget.All);

    }
    #endregion


    //-----------------------------------------------------------------------------------------------
    //                           M�TODOS PROPIOS
    //-----------------------------------------------------------------------------------------------
    #region M�todos privados
    /// <summary>
    /// Activa el panel que entra por par�metro
    /// </summary>
    /// <param name="_panel"> nombre del panel a activar</param>
    private void ActivarPaneles(GameObject _panel)
    {
        panelConexion.SetActive(false);
        panelBienvenida.SetActive(false);
        panelCreacionSala.SetActive(false);
        panelUnirseSala.SetActive(false);
        panelDeSala.SetActive(false);
        panelAvatar.SetActive(false);

        _panel.SetActive(true);
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Mostrar mensajes en la barra de estado y consola
    /// </summary>
    /// <param name="_mensaje"></param>
    private void Estado( string _mensaje)
    {
        txtBarraEstado.text = _mensaje;
        Debug.Log( _mensaje);
    }


    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Actualiza la lista de jugadores del panel Sala
    /// </summary>
    private void ActualizarPanelDeJugadores()
    {
        //Actualizaci�n del nombre de sala y su capacidad
        txtNombreSalaPanelSala.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
        txtCapacidadPanelSala.text = "Capacidad: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers;

        txtListadoJugadores.text = "";

        while( contenedorJugador.transform.childCount > 0 ) 
        {
            DestroyImmediate( contenedorJugador.transform.GetChild(0).gameObject );
        }


        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            txtListadoJugadores.text = txtListadoJugadores.text + "\n" + jugador.NickName;

            GameObject nuevoElemento = Instantiate(elemJugador);
            nuevoElemento.transform.SetParent(contenedorJugador.transform);

            nuevoElemento.transform.Find("txtNickName").
                GetComponent<TextMeshProUGUI>().text = jugador.NickName;

            //nuevoElemento.transform.Find("txtNumActor").
            //    GetComponent<TextMeshProUGUI>().text = avatarSeleccionado.ToString(); 

            // mas adelanta vamos a colocar el nombre del personaje seleccionado
            object avatarJugador = jugador.CustomProperties["avatar"];
            string avatar = "";

            switch ((int)avatarJugador)
            {
                case 0:
                    avatar = "Humano";
                    break;

                case 1:
                    avatar = "Agricola";
                    break;

                case 2:
                    avatar = "Robot";
                    break;

                case 3:
                    avatar = "Malvado";
                    break;
            }
            nuevoElemento.transform.Find("txtNumActor").
                GetComponent<TextMeshProUGUI>().text = avatar;
        }

        //Activacion del boton Comenzar Juego si el n�mero minimo de jugadores esta en la sala
        // y eres Master
        if (PhotonNetwork.CurrentRoom.PlayerCount >= int.Parse(txtMinJugadores.text) && 
            PhotonNetwork.IsMasterClient)
        {
            btnComenzarJuego.gameObject.SetActive(true);
        }
        else
        {
            btnComenzarJuego.gameObject.SetActive(false);
        }
    }

    //----------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
  
    public void ActualizarPanelUnirseASala()
    {
        // borrar el contenido de los prefabs que hacen referencia a la salas

        while (contenedorSala.transform.childCount > 0)
        {
            DestroyImmediate(contenedorSala.transform.GetChild(0).gameObject);
        }

        foreach (RoomInfo sala in listaSalas.Values)
        {
            GameObject nuevoElemento = Instantiate(elemSala);
            nuevoElemento.transform.SetParent(contenedorSala.transform, false);

            // localizar las etiquetas y las actualizamos
            nuevoElemento.transform.Find("TxtNombreSala")
                .GetComponent<TextMeshProUGUI>().text = sala.Name;

            nuevoElemento.transform.Find("TxtCapacidadSala")
                .GetComponent<TextMeshProUGUI>().text = sala.PlayerCount + "/" + sala.MaxPlayers;

            nuevoElemento.GetComponent<Button>().onClick.AddListener(()
                =>
            { Pulsar_BtnUnirseASalaDesdeLista(sala.Name); });


        }
    }



    //----------------------------------------------------------------------------------------
    //
    [PunRPC]
    public void EliminarSalasViejas()
    {

    }

    //----------------------------------------------------------------------------------------
    //
    void Pulsar_BtnUnirseASalaDesdeLista( string _sala)
    {
        PhotonNetwork.JoinRoom(_sala);
    }

    //----------------------------------------------------------------------------------------
    //
    public void Pulsar_BtnComenzarJuego()
    {
        PhotonNetwork.LoadLevel(1);
    }



    #endregion

}