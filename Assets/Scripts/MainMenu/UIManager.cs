using UnityEngine;

namespace MainMenu
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private LevelsPanel _levelsPanel;
        [SerializeField] private ShopHandler _shopView;
        [SerializeField] private PopupUtils _popupUtils;


        public ShopHandler ShopView => _shopView;
        public LevelsPanel LevelsPanel => _levelsPanel;
        public PopupUtils PopupUtils => _popupUtils;


        private void Awake()
        {
            Instance = this;

        }


        public void OnShowMessage(string msg)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (unityActivity == null) return;
            var toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, msg,
                    2);
                toastObject.Call("show");
            }));
#endif
        }

    }
}