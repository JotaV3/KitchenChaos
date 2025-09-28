using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Unity.Netcode;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            GameMultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        GameMultiplayerManager.Instance.OnPlayerDataListChanged += GameMultiplayerManager_OnPlayerDataListChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayerVisual();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayerVisual();
    }

    private void GameMultiplayerManager_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayerVisual();
    }

    private void UpdatePlayerVisual()
    {
        if (GameMultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerVisual.SetPlayerColor(GameMultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(GameMultiplayerManager.Instance != null)
        {
            GameMultiplayerManager.Instance.OnPlayerDataListChanged -= GameMultiplayerManager_OnPlayerDataListChanged;
        }
    }
}
