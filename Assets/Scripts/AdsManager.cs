using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

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
    [SerializeField] Image _errorBanner;
    [SerializeField] Button _reloadBtn;
    [SerializeField] Button _cancelBtn;
    [SerializeField] string _androidGameId;
    [SerializeField] InterstitialIdAds _androidAdUnitId;
    [SerializeField] RewardedIdAds _androidAdUnitIdRewarded;
    [SerializeField] BannerIdAds _androidAdUnitIdBanner;
    [SerializeField] string _iOSGameId;
    [SerializeField] InterstitialIdAds _iOSAdUnitId;
    [SerializeField] RewardedIdAds _iOSAdUnitIdRewarded;
    [SerializeField] BannerIdAds _iOSAdUnitIdBanner;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] bool _testMode = true;
    private string _adUnitId;
    private string _adUnitIdRewarded;
    private string _adUnitIdBanner;
    private string _gameId;
    private UnityAction buttonCallback;
 
    void Awake(){
        _cancelBtn.onClick.AddListener(()=> _errorBanner.gameObject.SetActive(false));

        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitId.ToString(): _androidAdUnitId.ToString();
        _adUnitIdRewarded = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitIdRewarded.ToString(): _androidAdUnitIdRewarded.ToString();
        _adUnitIdBanner = (Application.platform == RuntimePlatform.IPhonePlayer)? _iOSAdUnitIdBanner.ToString(): _androidAdUnitIdBanner.ToString();
        _showAdButton.interactable = false;
        _errorBanner.gameObject.SetActive(false);
        if(Advertisement.isInitialized) {
            LoadAd();
            loadRewardedAd();
            LoadBanner();
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
        LoadBanner();
    }

    public void loadRewardedAd() {
        _showAdButton.interactable = false;
        Advertisement.Load(_adUnitIdRewarded, this);
    }
 
    public void OnInitializationComplete()
    {
        // LoadAd();
        loadRewardedAd();
        LoadBanner();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        string mgs = $"Unity Ads Initialization Failed: {error.ToString()} - {message}";
        HandledError(mgs, InitializeAds);
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
        // if(adUnitId.Equals(_adUnitId)) {
        //     ShowAd();
        // }
        ConfigureBtnShowAd(adUnitId);
    }
 
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        string mgs = $"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}";
        HandledError(adUnitId, mgs, loadRewardedAd);
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        string mgs = $"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}";
        HandledError(adUnitId, mgs, loadRewardedAd);

        Time.timeScale = 1;
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
        Time.timeScale = 1;

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
            CurrencyManager.Instance.updateCurrency(150,"Gold");
            _showAdButton.onClick.RemoveListener(showAdRewarded);
            // Load another ad:
            Advertisement.Load(_adUnitIdRewarded, this);
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
        Advertisement.Banner.Show(_adUnitIdBanner);
    }
 
    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
        LoadBanner();
    }

    private void showErrorBanner(string ErrorMessage) {
        _errorBanner.gameObject.SetActive(true);
        _errorBanner.GetComponentInChildren<TextMeshProUGUI>().text = ErrorMessage;
    }

    private void addCallbackButton(UnityAction Callback) {
         if( buttonCallback != null ) {
            _reloadBtn.onClick.RemoveListener(buttonCallback);
         }

         buttonCallback = Callback;
         _reloadBtn.onClick.AddListener(()=> {
            buttonCallback();
            _errorBanner.gameObject.SetActive(false);
         });
    }
    private void HandledError(string message, UnityAction callback) {
        showErrorBanner(message);
        addCallbackButton(()=> {
            callback();
        });
    }
    private void HandledError(string adUnitId, string message, UnityAction callback) {
        showErrorBanner(message);
        addCallbackButton(()=> {
            callback();
        });

        if(adUnitId.Equals(_adUnitIdRewarded) && adUnitId != null) {
            if(_showAdButton.onClick != null) {
                _showAdButton.onClick.RemoveListener(showAdRewarded);
            }
            _showAdButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }

}
