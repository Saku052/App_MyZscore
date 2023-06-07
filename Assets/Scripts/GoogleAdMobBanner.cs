using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class GoogleAdMobBanner : MonoBehaviour
{
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        // Load ad
        this.LoadAd();
    }

    // These ad units are configured to always serve test ads.
    #if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
    #elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
    #else
    private string _adUnitId = "unused";
    #endif

    BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x100 banner at top of the screen (can be 320x50 also)
        AdSize adSize = new AdSize(320, 100);
        _bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if(_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }


    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
}