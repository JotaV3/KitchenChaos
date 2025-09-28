using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameMultiplayerManager.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        GameMultiplayerManager.Instance.OnPlayerDataListChanged += GameMultiplayerManager_OnPlayerDataListChanged;

        image.color = GameMultiplayerManager.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void GameMultiplayerManager_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if(GameMultiplayerManager.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if(GameMultiplayerManager.Instance != null)
        {
            GameMultiplayerManager.Instance.OnPlayerDataListChanged -= GameMultiplayerManager_OnPlayerDataListChanged;
        }
    }
}
