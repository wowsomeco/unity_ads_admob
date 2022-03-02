using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobReward : MonoBehaviour, IAd {
    [Serializable]
    public struct Model {
      public string UnitId {
        get {
          string unitId = PlatformUtil.GetStringByPlatform(unitIdIOS, unitIdAndroid, unitIdAmazon);
          return unitId.Trim();
        }
      }

      public string unitIdIOS;
      public string unitIdAndroid;
      public string unitIdAmazon;
    }

    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.Rewarded;

    public Model data;
    public float delayLoad;

    Action _onDone = null;
    RewardedAd _rewardedAd;
    ObservableTimer _timer = null;

    public bool ShowAd(Action onDone = null) {
      if (_rewardedAd.IsLoaded()) {
        _onDone = onDone;

        _rewardedAd.Show();

        return true;
      } else {
        LoadAd();
      }

      return false;
    }

    public void InitAd(IAdsProvider provider) {
      _rewardedAd = new RewardedAd(data.UnitId);

      _rewardedAd.OnAdLoaded += (_, __) => {
        IsLoaded.Next(true);
      };

      _rewardedAd.OnUserEarnedReward += (sender, e) => {
        _onDone?.Invoke();

        _onDone = null;
      };

      _rewardedAd.OnAdClosed += (sender, e) => {
        RequestAd();
      };

      _rewardedAd.OnAdFailedToLoad += (sender, args) => {
        Debug.Log("admob rewarded HandleFailedToReceiveAd event received with message: " + args.ToString());
      };

      LoadAd();
    }

    public void OnDisabled() { }

    void RequestAd() {
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // load video
      _rewardedAd.LoadAd(request);
    }

    void LoadAd() {
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

