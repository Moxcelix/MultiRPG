using Mirror;
using Steamworks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance { get; private set; }

    [SerializeField] private Text nameText;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private PlayerListItem playerListItemPrefab;

    // To remove?
    private GameObject localPlayerObject;
    private PlayerObjectController localPlayerObjectController;

    public ulong CurrentLobbyId { get; private set; }

    private bool playerItemCreated = false;

    private readonly List<PlayerListItem> playerList = new();

    private CustomNetworkManager Manager => NetworkManager.singleton as CustomNetworkManager;

    private void Awake()
    {
        Instance ??= this;
    }

    public void SetLocalPlayer(PlayerObjectController player) 
    {
        localPlayerObjectController = player;
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyId = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        nameText.text = SteamMatchmaking.GetLobbyData((CSteamID)CurrentLobbyId, SteamLobby.NameKey);
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated) CreateHostPlayerItem();
        if (playerList.Count < Manager.GamePlayers.Count) CreateClientPlayerItem();
        if (playerList.Count > Manager.GamePlayers.Count) RemovePlayerItem();
        if (playerList.Count == Manager.GamePlayers.Count) UpdatePlayerItem();
    }

    public void CreateHostPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            AddPlayerToList(player);
        }

        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (!playerList.Any(b => b.ConnectionId == player.ConnectionId)) 
            {
                AddPlayerToList(player);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        if (playerList.Count != Manager.GamePlayers.Count)
            throw new System.Exception("Sync error");

        for (int i = 0; i < playerList.Count; i++) 
        {
            playerList[i].PlayerName = Manager.GamePlayers[i].PlayerName;
            playerList[i].SetValues();
        }
    }

    public void RemovePlayerItem()
    {
        var playerItemToRemove = new List<PlayerListItem>();

        foreach (var item in playerList) 
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionId == item.ConnectionId))
            {
                playerItemToRemove.Add(item);
            }
        }

        if (playerItemToRemove.Count > 0) 
        {
            foreach (var player in playerItemToRemove) 
            {
                var gameObject = player.gameObject;
                playerList.Remove(player);
                Destroy(gameObject);
                gameObject = null;
            }
        }
    }

    private void AddPlayerToList(PlayerObjectController player) 
    {
        PlayerListItem newItem = Instantiate(playerListItemPrefab);

        newItem.PlayerName = player.PlayerName;
        newItem.ConnectionId = player.ConnectionId;
        newItem.PlayerSteamId = player.PlayerSteamId;

        newItem.gameObject.transform.SetParent(playerListContent);
        newItem.gameObject.transform.localScale = Vector3.one;

        playerList.Add(newItem);
    }
}

