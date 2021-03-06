using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobReward : MonoBehaviour, IAd {
    public int Priority => priority;
    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.Rewarded;

    public WAdmobUnitModel data;
    public float delayLoad;
    public int priority;

    IAdsProvider _provider;
    Action _onDone = null;
    RewardedAd _rewardedAd;
    ObservableTimer _timer = null;

    public bool ShowAd(Action onDone = null) {
      if (IsLoaded.Value) {
        _onDone = onDone;

        _rewardedAd.Show();

        return true;
      }

      return false;
    }

    public void InitAd(IAdsProvider provider) {
      _provider = provider;

      _rewardedAd = new RewardedAd(data.GetUnitId(_provider.IsTestMode));

      _rewardedAd.OnAdLoaded += (_, __) => {
        IsLoaded.Next(true);
      };

      _rewardedAd.OnUserEarnedReward += (sender, e) => {
        _onDone?.Invoke();

        _onDone = null;
      };

      _rewardedAd.OnAdClosed += (sender, e) => {
        RequestAdWithDelay();
      };

      _rewardedAd.OnAdFailedToLoad += (sender, args) => {
        Debug.Log("admob rewarded HandleFailedToReceiveAd event received with message: " + args.ToString());
      };

      RequestAdWithDelay();
    }

    public void OnDisabled() { }

    void RequestAd() {
      IsLoaded.Next(false);
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // load video
      _rewardedAd.LoadAd(request);
    }

    void RequestAdWithDelay() {
      IsLoaded.Next(false);

      _timer = new ObservableTimer(delayLoad);
      _timer.OnDone += () => {
        RequestAd();

        _timer = null;
      };
    }

    void Update() {
      _timer?.UpdateTimer(Time.deltaTime);
    }
  }
}

