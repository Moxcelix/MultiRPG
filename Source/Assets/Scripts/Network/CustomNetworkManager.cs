using Mirror;
using Steamworks;
using UnityEngine;
using System.Collections.Generic;

public class CustomNetworkManager : NetworkManager
{
    public List<PlayerObjectController> GamePlayers => gamePlayers;
    public bool CanAddPlayers { get; set; } = true;

    [SerializeField] private PlayerObjectController playerObjectControllerPrefab;
    private readonly List<PlayerObjectController> gamePlayers = new();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection)
    {
        if (CanAddPlayers) 
        {
            PlayerObjectController playerObjectController = Instantiate(playerObjectControllerPrefab);

            playerObjectController.ConnectionId = connection.connectionId;
            playerObjectController.PlayerId = gamePlayers.Count + 1;
            playerObjectController.PlayerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(connection, playerObjectController.gameObject);
        }
    }

}
