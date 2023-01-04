using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

[RequireComponent(typeof(CustomNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _joinRequest;
    protected Callback<LobbyEnter_t> _lobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private const string NameKey = "name";
    private CustomNetworkManager _manager;

    [SerializeField] private GameObject _hostButton;
    [SerializeField] private Text _lobbyNameText;
    [SerializeField] private ELobbyType _lobbyType;

    private void Start()
    {
        if (!SteamManager.Initialized) return;

        _manager = GetComponent<CustomNetworkManager>();

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnCreated);
        _joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        _lobbyEntered = Callback<LobbyEnter_t>.Create(OnEntered);
    }

    public void HostLobby() 
    {
        SteamMatchmaking.CreateLobby(_lobbyType, _manager.maxConnections);
    }

    private void OnCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby created!");

        _manager.StartHost();

        SteamMatchmaking.SetLobbyData(new(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new(callback.m_ulSteamIDLobby), NameKey, SteamFriends.GetPersonaName().ToString());


    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join!");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnEntered(LobbyEnter_t callback)
    {
        // For everyone

        _hostButton.SetActive(false);
        _lobbyNameText.gameObject.SetActive(true);
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new(callback.m_ulSteamIDLobby), NameKey);

        // For client

        if (NetworkServer.active) return;

        _manager.networkAddress = SteamMatchmaking.GetLobbyData(new(callback.m_ulSteamIDLobby), HostAddressKey);

        _manager.StartClient();
    }
}