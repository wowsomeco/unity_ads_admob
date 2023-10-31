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
    RewardedAd _rewardedAd;
    ObservableTimer _timer = null;

    public bool ShowAd(Action onDone = null) {
      if (IsLoaded.Value && _rewardedAd.CanShowAd()) {
        _rewardedAd.Show(_ => {
          onDone?.Invoke();
        });

        return true;
      }

      return false;
    }

    public void InitAd(IAdsProvider provider) {
      _provider = provider;

      RequestAdWithDelay();
    }

    public void OnDisabled() { }

    void RequestAd() {
      IsLoaded.Next(false);

      RewardedAd.Load(data.GetUnitId(_provider.IsTestMode), new AdRequest(),
        (RewardedAd ad, LoadAdError loadError) => {
          if (loadError != null) {
            Debug.Log("Rewarded ad failed to load with error: " + loadError.GetMessage());
            return;
          } else if (ad == null) {
            Debug.Log("Rewarded ad failed to load.");
            return;
          }

          Debug.Log("Rewarded ad loaded.");
          _rewardedAd = ad;

          _rewardedAd.OnAdFullScreenContentClosed += () => {
            RequestAdWithDelay();
          };

          IsLoaded.Next(true);
        }
      );
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