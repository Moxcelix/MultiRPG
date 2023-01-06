using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName { get; set; }
    public int ConnectionId { get; set; }
    public ulong PlayerSteamId { get; set; }

    private bool avatarReceived;


    [SerializeField] private Text nameText;
    [SerializeField] private RawImage avatarImage;

    protected Callback<AvatarImageLoaded_t> imageLoaded;

    private void Start()
    {
        imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback) 
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamId)
        {
            avatarImage.texture = GetSteamImageAsTexture(callback.m_iImage);
            avatarReceived = true;
        }
    }

    public void SetValues() 
    {
        nameText.text = PlayerName;

        if (!avatarReceived)
            GetAvatar();
    }

    private void GetAvatar() 
    {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamId);

        if (imageId == -1) return;

        avatarImage.texture = GetSteamImageAsTexture(imageId);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }
}
