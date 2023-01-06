using Mirror;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(CustomNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }

    public ulong CurrentLobbyID;
    public const string HostAddressKey = "HostAddress";
    public const string NameKey = "name";

    public delegate void OnJoinLobbyDlegate();
    public event OnJoinLobbyDlegate OnJoinLobby;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private CustomNetworkManager manager;

    [SerializeField] private ELobbyType lobbyType;

    private void Awake()
    {
        Instance ??= this;
    }

    private void Start()
    {
        if (!SteamManager.Initialized) return;

        manager = GetComponent<CustomNetworkManager>();

        lobbyEntered = Callback<LobbyEnter_t>.Create(OnEntered);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
    }

    public void HostLobby() 
    {
        SteamMatchmaking.CreateLobby(lobbyType, manager.maxConnections);
    }

    private void OnCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby created!");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new(callback.m_ulSteamIDLobby), NameKey, SteamFriends.GetPersonaName().ToString());
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join!");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);


        OnJoinLobby?.Invoke();
    }

    private void OnEntered(LobbyEnter_t callback)
    {
        // For everyone

        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // For client

        if (NetworkServer.active) return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
    }
}