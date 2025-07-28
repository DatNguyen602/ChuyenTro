using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Events;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager instance;

    [SerializeField] private string bannerId;
    [SerializeField] private string interId;
    [SerializeField] private string rewardedId;

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    [Header("Ads Events For Game")]
    public UnityAction interRewardEvent;
    public UnityAction RewardedEndEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        interRewardEvent += GiveInterReward;
        RewardedEndEvent += GiveRewardedReward;
    }

    #region Banner Ads

    public void LoadBannerAd()
    {
        if (string.IsNullOrEmpty(bannerId))
        {
            Debug.LogWarning("Banner Ad Unit ID is not set.");
            return;
        }

        if (_bannerView == null)
        {
            CreateBannerView();
        }

        ListenToBannerAdEvents();
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private void CreateBannerView()
    {
        DestroyBannerView();
        _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void ListenToBannerAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner loaded: " + _bannerView.GetResponseInfo());
        };

        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner load failed: " + error);
        };

        _bannerView.OnAdClicked += () => Debug.Log("Banner clicked.");
        _bannerView.OnAdImpressionRecorded += () => Debug.Log("Banner impression recorded.");
        _bannerView.OnAdPaid += (AdValue value) => Debug.Log($"Banner paid {value.Value} {value.CurrencyCode}");
        _bannerView.OnAdFullScreenContentOpened += () => Debug.Log("Banner fullscreen opened.");
        _bannerView.OnAdFullScreenContentClosed += () => Debug.Log("Banner fullscreen closed.");
    }

    #endregion

    #region Interstitial Ads

    public void LoadInterstitialAd()
    {
        if (string.IsNullOrEmpty(interId))
        {
            Debug.LogWarning("Interstitial Ad Unit ID is not set.");
            return;
        }

        _interstitialAd?.Destroy();
        _interstitialAd = null;

        Debug.Log("Loading interstitial ad.");
        var adRequest = new AdRequest();

        InterstitialAd.Load(interId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial load failed: " + error);
                return;
            }

            Debug.Log("Interstitial loaded: " + ad.GetResponseInfo());
            _interstitialAd = ad;
            RegisterInterstitialEvents(_interstitialAd);
        });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd?.CanShowAd() == true)
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial not ready, reloading.");
            LoadInterstitialAd();
        }
    }

    private void RegisterInterstitialEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
            interRewardEvent?.Invoke();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial failed to open: " + error);
            LoadInterstitialAd();
        };
    }

    private void GiveInterReward() => Debug.Log("Interstitial reward granted.");

    public bool IsInterstitialAdReady() => _interstitialAd?.CanShowAd() == true;

    #endregion

    #region Rewarded Ads

    private void LoadRewardedAd()
    {
        if (string.IsNullOrEmpty(rewardedId))
        {
            Debug.LogWarning("Rewarded Ad Unit ID is not set.");
            return;
        }

        _rewardedAd?.Destroy();
        _rewardedAd = null;

        Debug.Log("Loading rewarded ad.");
        var adRequest = new AdRequest();

        RewardedAd.Load(rewardedId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded load failed: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded: " + ad.GetResponseInfo());
            _rewardedAd = ad;
            RegisterRewardedEvents(_rewardedAd);
        });
    }

    public void ShowRewardedAd()
    {
        System.Action LastShow = () =>
        {
           Debug.Log("Rewarded ad shown.");
        };
        if (_rewardedAd?.CanShowAd() == true)
        {
            _rewardedAd.Show(reward =>
            {
                Debug.Log($"Reward received: {reward.Type}, {reward.Amount}");
                RewardedEndEvent?.Invoke();
            });
            _rewardedAd.OnAdFullScreenContentOpened -= LastShow;
            _rewardedAd.OnAdFullScreenContentOpened += LastShow;
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready, reloading.");
            LoadRewardedAd();
        }
    }

    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => LoadRewardedAd();
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded failed to open: " + error);
            LoadRewardedAd();
        };
    }

    private void GiveRewardedReward() => Debug.Log("Rewarded reward granted.");

    #endregion
}
