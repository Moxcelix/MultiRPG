using UnityEngine;

internal class Level : MonoBehaviour
{
    [SerializeField] private MenuSwitcher menuSwitcher;

    private readonly int listPage = 1;

    private void Awake()
    {
        SteamLobby.Instance.OnJoinLobby += () => menuSwitcher.OpenPage(listPage);
    }

}
