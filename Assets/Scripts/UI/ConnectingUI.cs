using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameMultiplayerManager.Instance.OnTryingToJoinGame += GameMultiplayerManager_OnTryingToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;

        Hide();
    }

    private void GameMultiplayerManager_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
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
        GameMultiplayerManager.Instance.OnTryingToJoinGame -= GameMultiplayerManager_OnTryingToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;    
    }
}
