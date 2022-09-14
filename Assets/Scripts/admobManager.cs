using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class admobManager : MonoBehaviour
{
    [SerializeField] Button _showAdbtn;
    [SerializeField] string adUnitIdBanner = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] string adUnitIdInterstitial = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] string adUnitIdRewarded = "ca-app-pub-3940256099942544/5224354917";

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    // Start is called before the first frame update
    void Start()
    {
        _showAdbtn.interactable = false;
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
            .SetSameAppKeyEnabled(true)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(InitializationStatus=> {});


        RequestRewarded();

        RequestBanner();
        // RequestInterstitial();
        // if (this.interstitial.IsLoaded()) {
        //     this.interstitial.Show();
        // }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitIdBanner, AdSize.Banner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitIdInterstitial);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    private void RequestRewarded() {
        this.rewardedAd = new RewardedAd(adUnitIdRewarded);

        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);


    }

    public void UserChoseToWatchAd() {
        this.rewardedAd.Show();
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        _showAdbtn.interactable = true;
        _showAdbtn.onClick.AddListener(UserChoseToWatchAd);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        _showAdbtn.interactable = false;
        _showAdbtn.onClick.RemoveListener(UserChoseToWatchAd);
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        CurrencyManager.Instance.updateCurrency(150,"Gold");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewarded();
    }

}
