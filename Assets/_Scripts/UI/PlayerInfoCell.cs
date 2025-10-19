using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoCell : UI
{
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong _steamId;

    [SerializeField] private RawImage _image;
    [SerializeField] private TextMeshProUGUI _playerNameText;

    protected Callback<AvatarImageLoaded_t> OnAvatarImageLoaded;

    #region Server
    public void SetSteamId(ulong steamId)
        => _steamId = steamId;
    #endregion

    #region Client
    public override void OnStartClient()
    {
        base.OnStartClient();

        OnAvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(HandleAvatarImageLoaded);
    }

    private void HandleAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != _steamId)
            return;

        _image.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private void HandleSteamIdUpdated(ulong oldId, ulong newId)
    {
        CSteamID steamId = new(newId);

        _playerNameText.text = SteamFriends.GetFriendPersonaName(steamId);

        int imageId = SteamFriends.GetLargeFriendAvatar(steamId);

        if (imageId == -1)
            return;

        _image.texture = GetSteamImageAsTexture(imageId);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            uint bufferSize = width * height * 4;
            byte[] imageByteArray = new byte[bufferSize];

            isValid = SteamUtils.GetImageRGBA(iImage, imageByteArray, (int)bufferSize);

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, false);
                texture.LoadRawTextureData(imageByteArray);
                texture.Apply();
            }
        }

        return texture;
    }
    #endregion
}
