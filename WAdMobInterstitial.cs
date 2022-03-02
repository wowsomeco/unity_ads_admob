using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobInterstitial : MonoBehaviour, IAd {
    [Serializable]
    public struct Model {
      public string unitIdIOS;
      public string unitIdAndroid;
      public string unitIdAmazon;

      public string UnitId {
        get {
          string unitId = PlatformUtil.GetStringByPlatform(unitIdIOS, unitIdAndroid, unitIdAmazon);
          return unitId.Trim();
        }
      }
    }

    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.Interstitial;

    public Model data;
    public float delayLoad;

    InterstitialAd _interstitial;
    Action _onDone = null;
    ObservableTimer _timer = null;

    public bool ShowAd(Action onDone = null) {
      if (IsLoaded.Value) {
        _onDone = onDone;

        _interstitial.Show();

        return true;
      }

      LoadAd();

      return false;
    }

    public void InitAd(IAdsProvider provider) {
      LoadAd();
    }

    public void OnDisabled() { }

    void RequestAd() {
      // destroy first if it's not null
      if (null != _interstitial) {
        _interstitial.Destroy();
      }

      _interstitial = new InterstitialAd(data.UnitId);

      _interstitial.OnAdLoaded += (_, __) => {
        IsLoaded.Next(true);
      };

      // because admob is soo special, you need to delay reload
      // otherwise they will complain with their Too many recently failed requests on some random ios devices.
      _interstitial.OnAdClosed += (object sender, EventArgs e) => {
        _onDone?.Invoke();
        _onDone = null;

        IsLoaded.Next(false);

        LoadAd();
      };
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // Load the interstitial with the request.
      _interstitial.LoadAd(request);
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
