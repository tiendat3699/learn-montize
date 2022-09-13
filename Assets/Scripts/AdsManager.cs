using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public enum InterstitialIdAds {
    Interstitial_Android,
    Interstitial_iOS,
}
public enum RewardedIdAds {
    Rewarded_Android,
    Rewarded_iOS,
}
public enum BannerIdAds {
    Banner_Android,
    Banner_iOS,
}
public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] InterstitialIdAds _androidAdUnitId;
    [SerializeField] InterstitialIdAds _iOSAdUnitId;
    [SerializeField] RewardedIdAds _androidAdUnitIdRewarded;
    [SerializeField] RewardedIdAds _iOSAdUnitIdRewarded;
    [SerializeField] BannerIdAds _androidAdUnitIdBanner;
    [SerializeField] BannerIdAds _iOSAdUnitIdBanner;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] bool _testMode = true;
    private string _adUnitId;
    private string _adUnitIdRewarded;
    private string _adUnitIdBanner;
    private string _gameId;
 
    void Awake()
    {
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitId.ToString(): _androidAdUnitId.ToString();
        _adUnitIdRewarded = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitIdRewarded.ToString(): _androidAdUnitIdRewarded.ToString();
        _adUnitIdBanner = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitIdBanner.ToString(): _androidAdUnitIdBanner.ToString();
        _showAdButton.interactable = false;
        if(Advertisement.isInitialized) {
            LoadAd();
        } else {
            InitializeAds();
        }
    }
 
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSGameId: _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Advertisement.Load(_adUnitId, this);
        Advertisement.Load(_adUnitIdRewarded, this);
        LoadBanner();
    }
 
    public void OnInitializationComplete()
    {
        LoadAd();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Advertisement.Show(_adUnitId, this);
        Advertisement.Banner.Hide();
    }
 
    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        if(adUnitId.Equals(_adUnitId)) {
            ShowAd();
        }
        ConfigureBtnShowAd(adUnitId);
    }
 
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Time.timeScale = 0;
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        GetRewarded(adUnitId, showCompletionState);
        Advertisement.Banner.Show(_adUnitIdBanner);
        Time.timeScale = 0;

    }

    private void ConfigureBtnShowAd(string adUnitId) {
        if (adUnitId.Equals(_adUnitIdRewarded))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(showAdRewarded);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }

    private void showAdRewarded() {
        Advertisement.Show(_adUnitIdRewarded, this);
        Advertisement.Banner.Hide();
    }

    private void GetRewarded(string adUnitId, UnityAdsShowCompletionState showCompletionState) {
        if (adUnitId.Equals(_adUnitIdRewarded) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            // Grant a reward.
            CurrencyManager.Instance.updateCurrency(150, "Gold");
            // Load another ad:
            // Advertisement.Load(_adUnitIdRewarded, this);
        }
    }

    public void LoadBanner()
    {
        Advertisement.Banner.SetPosition(_bannerPosition);
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitIdBanner, options);
    }
 
    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        // Advertisement.Banner.Show(_adUnitIdBanner);
    }
 
    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }


    private void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}
