using UnityEngine;
using UnityEngine.EventSystems;

public class RatingButton : MonoBehaviour,IPointerClickHandler
{
    public static bool Rated
    {
        get { return PrefManager.GetBool(nameof(Rated)); }
        private set { PrefManager.SetBool(nameof(Rated),value); }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenUrl();
    }

    public static void OpenUrl()
    {
        Application.OpenURL(Application.platform == RuntimePlatform.Android ? $"https://play.google.com/store/apps/details?id={Application.identifier}" :
            $"http://itunes.apple.com/app/id{GameSettings.Default.IosAppId}");
        Rated = true;
    }
}