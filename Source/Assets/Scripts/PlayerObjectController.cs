using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionId;
    [SyncVar] public int PlayerId;
    [SyncVar] public ulong PlayerSteamId;
    [SyncVar(hook = nameof(NameUpdate))] public string PlayerName;

    private CustomNetworkManager Manager => NetworkManager.singleton as CustomNetworkManager;

    public override void OnStartAuthority()
    {
        CommandSetPlayerName(SteamFriends.GetPersonaName());

        gameObject.name = "LocalPlayer";

        LobbyController.Instance.SetLocalPlayer(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command] private void CommandSetPlayerName(string name) 
    {
        NameUpdate(PlayerName, name);
    }

    private void NameUpdate(string oldValue, string newValue) 
    {
        if (isServer) PlayerName = newValue;
        else LobbyController.Instance.UpdatePlayerList();
    }
}
