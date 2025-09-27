using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;

        Hide();
    }

    private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "Failed to connect to the server!";
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
        GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;    
    }
}
