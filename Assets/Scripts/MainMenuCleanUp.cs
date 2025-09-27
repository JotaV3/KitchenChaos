using UnityEngine;
using Unity.Netcode;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(GameMultiplayerManager.Instance != null)
        {
            Destroy(GameMultiplayerManager.Instance.gameObject);
        }
    }
}
