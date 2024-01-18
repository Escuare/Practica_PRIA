using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

using ExitGames.Client.Photon;

public class CtrlChat : MonoBehaviour, IChatClientListener
{
    [Header("Propiedades Chat")]
    [SerializeField] private PhotonView PV;
    [SerializeField] private TMP_InputField inpMensaje;
    [SerializeField] private TMP_Text txtContenidoChat;
    [SerializeField] private TMP_Text txtEstado;
    [SerializeField] private Button btnEnviar;
    [SerializeField] private string[] canales;
    [SerializeField] private string[] amigos;

    [SerializeField] private GameObject chatPanel;

    [SerializeField] private CtrlJugador controlJugador;

    private bool chatHabilitado = false;
    private ChatClient clienteChat;
    private string nombreCanalActual;




    //---------------------------------------------------------------------------------
    void Awake()
    {
        controlJugador = GetComponent<CtrlJugador>();
        PV = controlJugador.photonView;

        inpMensaje = GameObject.Find("InpMensajeAEnviar").GetComponent<TMP_InputField>();
        txtContenidoChat = GameObject.Find("TxtListaMensajes").GetComponent<TMP_Text>();
        txtEstado = GameObject.Find("TxtEstado").GetComponent<TMP_Text>();
        btnEnviar = GameObject.Find("BtnEnviarMensaje").GetComponent<Button>();
        chatPanel = GameObject.Find("Panel_Chat");

        chatPanel.SetActive(false);
    }

    //---------------------------------------------------------------------------------

    void Start()
    {
        Debug.Log($"Start - preguntar si es mi instancia de photon");

        if(PV.IsMine)
        {
            inpMensaje.onEndEdit.AddListener(Enter_InpEnviarMensaje);
            btnEnviar.onClick.AddListener(Pulsar_BtnEnviarMensaje);


        }
    }


    //---------------------------------------------------------------------------------
    // Update is called once per frame
    /// <summary>
    /// Habilitar el panel del chat cuando se pulse la tecla ALT izquierda, 
    /// el clienteChat debe estar siempre en servicio
    /// </summary>
    void Update()
    {
        /*
        if (PV.IsMine)
        {
            if( Input.GetKeyDown( KeyCode.LeftAlt))
            {
                chatHabilitado = !chatHabilitado;

                controlJugador.HabilitarMovimiento = !controlJugador.HabilitarMovimiento ;

                Debug.Log($"Tecla ALT izquierda presionada {chatPanel.activeSelf}");
                chatPanel.SetActive(!chatPanel.activeSelf);
                Debug.Log($"Tecla ALT izquierda presionada {chatPanel.activeSelf}");
            }

            if (clienteChat != null)
                clienteChat.Service();  // llamando constantemente para mantenerse conectado el chat
        }
        */
    }


    //---------------------------------------------------------------------------------
    /// <summary>
    /// Enviar el mensaje cuando se pulsa el botón Enviar
    /// </summary>
    public void Pulsar_BtnEnviarMensaje()
    {
        Debug.Log($" Pulsar_BtnEnviarMensaje");
        PrepararMensaje();
    }


    //---------------------------------------------------------------------------------
    /// <summary>
    /// Envia el mensaje cuando pulsa ENTER desde la caja de texto
    /// </summary>
    public void Enter_InpEnviarMensaje( string _datos)
    {
        Debug.Log($" Enter_EnviarMensaje {_datos}");
        PrepararMensaje();
    }

    //---------------------------------------------------------------------------------
    private void PrepararMensaje()
    {
        string mensaje = inpMensaje.text;

        Debug.Log($"{mensaje}");

        if( mensaje.Length > 0 && mensaje != null)
        {
            EnviarMensajePun(mensaje);
            inpMensaje.text = string.Empty;
        }
    }

    //---------------------------------------------------------------------------------
    private void EnviarMensajePun( string _mensaje)
    {
        Debug.Log( $"EnviarMensajePun - {_mensaje}");

        clienteChat.PublishMessage(nombreCanalActual, _mensaje);
    }

    //---------------------------------------------------------------------------------
    /// <summary>
    /// Método que conecta el CHAT
    /// </summary>
    public void Conectarse()
    {
        Debug.Log($"Conectarse - {controlJugador.name}");
        clienteChat = new ChatClient(this);
        clienteChat.UseBackgroundWorkerForSending = true;
        clienteChat.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                       PhotonNetwork.AppVersion, new AuthenticationValues(controlJugador.Nombre));
    }
    
    //---------------------------------------------------------------------------------
    private void Desconectarse()
    {
        if (clienteChat != null)
            clienteChat.Disconnect();
    }


    //---------------------------------------------------------------------------------
    private void MostrarCanal( string _canal)
    {
        if( !string.IsNullOrEmpty( _canal))
        {
            ChatChannel canal = null;
            bool canalEncontrado = clienteChat.TryGetChannel(_canal, out canal);

            if (!canalEncontrado)
            {
                Debug.Log($"Canal no encontrado {_canal}");
            }
            else
            {
                nombreCanalActual = _canal;
                txtContenidoChat.text = canal.ToStringMessages();

                Debug.Log($"MostrarCanal - Nombre canal = {nombreCanalActual}  Contenido = {canal.ToStringMessages()}");
            }
        }
    }

    //---------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------
    /// <summary>
    /// Se ejecuta cuando se produzca un error lo visualizamos en el Debug
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public void DebugReturn(DebugLevel level, string message)
    {

        switch( level)
        {
            case DebugLevel.ERROR:
                Debug.LogError(message);
                break;

            case DebugLevel.WARNING:
                Debug.LogWarning(message);
                break;

            default:
                Debug.Log(message);
                break;
        }
    }

    //---------------------------------------------------------------------------------
    /// <summary>
    /// Método que se ejecuta cuando se desconecta el CHAT
    /// Vamos a publicar en los canales que el jugador sale de ellos
    /// </summary>
    public void OnDisconnected()
    {
        // throw new System.NotImplementedException();

        Debug.Log("Desconectado del chat");

        foreach (string canal in canales)
        {
            clienteChat.PublishMessage(canal, $"<color=#E07B00>{controlJugador.Nombre} ha salido del canal. </color>");
        }
    }
    //---------------------------------------------------------------------------------
    /// <summary>
    ///  si nos conectamos al CHAT, 
    ///  nos subscribimos a los canales y añadimos las amistades
    /// </summary>
    public void OnConnected()
    {
        if( canales != null && canales.Length >0)
        {
            clienteChat.Subscribe(canales);
        }

        if( amigos != null && amigos.Length > 0)
        {
            clienteChat.AddFriends(amigos);
        }

        clienteChat.SetOnlineStatus(ChatUserStatus.Online);
    }

    //---------------------------------------------------------------------------------
    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
        // cambiando el estado
        txtEstado.text = state.ToString();
        Debug.Log($"OnChatStateChange - {state.ToString()}");
    }

    //---------------------------------------------------------------------------------
    /// <summary>
    /// Se ejecuta caundo se recibe un mensaje
    /// </summary>
    /// <param name="channelName"></param>
    /// <param name="senders"></param>
    /// <param name="messages"></param>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        MostrarCanal(channelName);
    }


    //---------------------------------------------------------------------------------
    /// <summary>
    /// Se ejecuta cuando se reciben mensajes privados
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="channelName"></param>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Mensajes Privados");
    }


    //---------------------------------------------------------------------------------
    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();

        foreach( string canal in channels)
        {
            clienteChat.PublishMessage(canal, $"{controlJugador.Nombre} se ha unido al canal.");
        }

        Debug.Log($" Suscrito a {string.Join(", ", channels)}");
        MostrarCanal(channels[0]);  // mostar el primer canal
    }


    //---------------------------------------------------------------------------------
    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Darse de baja de los canales");
    }

    //---------------------------------------------------------------------------------
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
        Debug.Log($"OnStatusUpdate: usuario = {user} estado = {status}");
        txtEstado.text = status.ToString();
        Debug.Log($"OnStatusUpdate - {status.ToString()}");
    }

    //---------------------------------------------------------------------------------
    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
        Debug.Log($"OnUserSubscribed: canal = {channel} usuario = {user}");
    }

    //---------------------------------------------------------------------------------
    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
        Debug.Log($"OnUserUnSubscribed: canal = {channel} usuario = {user}");
    }
}
