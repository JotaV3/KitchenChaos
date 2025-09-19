using System.Runtime.CompilerServices;
using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        Hide();

        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged; 
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgessAmount = .5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgessAmount;

        if (show)
        {
            Show();
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
}
