using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Events;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager instance;

    [Header("Ad Unit IDs")]
    [SerializeField] private string bannerId;
    [SerializeField] private string interId;
    [SerializeField] private string rewardedId;

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    [Header("Ad Events")]
    public UnityAction interRewardEvent;
    public UnityAction rewardedEndEvent;

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
            return;
        }

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        interRewardEvent += () => Debug.Log("Interstitial reward granted.");
        rewardedEndEvent += () => Debug.Log("Rewarded reward granted.");
    }

    #region Banner Ads

    public void LoadBannerAd()
    {
        if (string.IsNullOrEmpty(bannerId)) return;

        if (_bannerView == null)
        {
            _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
            RegisterBannerEvents();
        }

        AdRequest request = new AdRequest();
        request.Keywords.Add("unity-admob-sample");
        _bannerView.LoadAd(request);
    }

    public void DestroyBannerAd()
    {
        _bannerView?.Destroy();
        _bannerView = null;
    }

    private void RegisterBannerEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner ad loaded.");
        };
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner ad failed: " + error);
        };
        _bannerView.OnAdClicked += () => Debug.Log("Banner clicked.");
        _bannerView.OnAdImpressionRecorded += () => Debug.Log("Banner impression recorded.");
        _bannerView.OnAdPaid += (AdValue value) =>
        {
            Debug.Log($"Banner paid: {value.Value} {value.CurrencyCode}");
        };
        _bannerView.OnAdFullScreenContentOpened += () => Debug.Log("Banner fullscreen opened.");
        _bannerView.OnAdFullScreenContentClosed += () => Debug.Log("Banner fullscreen closed.");
    }

    #endregion

    #region Interstitial Ads

    public void LoadInterstitialAd()
    {
        if (string.IsNullOrEmpty(interId)) return;

        _interstitialAd?.Destroy();
        _interstitialAd = null;

        AdRequest request = new AdRequest();
        InterstitialAd.Load(interId, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial failed: " + error);
                return;
            }

            _interstitialAd = ad;
            RegisterInterstitialEvents(ad);
        });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd?.CanShowAd() == true)
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready.");
            LoadInterstitialAd();
        }
    }

    private void RegisterInterstitialEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial closed.");
            interRewardEvent?.Invoke();
            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial failed to show: " + error);
            LoadInterstitialAd();
        };
    }

    public bool IsInterstitialAdReady() => _interstitialAd?.CanShowAd() == true;

    #endregion

    #region Rewarded Ads

    public void LoadRewardedAd()
    {
        if (string.IsNullOrEmpty(rewardedId)) return;

        _rewardedAd?.Destroy();
        _rewardedAd = null;

        AdRequest request = new AdRequest();
        RewardedAd.Load(rewardedId, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded failed: " + error);
                return;
            }

            _rewardedAd = ad;
            RegisterRewardedEvents(ad);
        });
    }

    public void ShowRewardedAd()
    {
        if (_rewardedAd?.CanShowAd() == true)
        {
            _rewardedAd.Show(reward =>
            {
                Debug.Log($"Reward received: {reward.Type}, {reward.Amount}");
                rewardedEndEvent?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded not ready.");
            LoadRewardedAd();
        }
    }

    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed.");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded failed to show: " + error);
            LoadRewardedAd();
        };
    }

    public bool IsRewardedAdReady() => _rewardedAd?.CanShowAd() == true;

    #endregion
}
