using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobInterstitial : MonoBehaviour, IAd {
    public int Priority => priority;
    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.Interstitial;

    public WAdmobUnitModel data;
    public float delayLoad;
    public int priority;

    IAdsProvider _provider;
    InterstitialAd _interstitial;
    Action _onDone = null;
    ObservableTimer _timer = null;

    public bool ShowAd(Action onDone = null) {
      if (IsLoaded.Value) {
        _onDone = onDone;

#if UNITY_EDITOR
        Print.Log(() => "cyan", "Show admob interstitial");
#else
        _interstitial.Show();
#endif

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
      // destroy first if it's not null
      if (null != _interstitial) {
        _interstitial.Destroy();
      }

      _interstitial = new InterstitialAd(data.GetUnitId(_provider.IsTestMode));

      _interstitial.OnAdLoaded += (_, __) => {
        IsLoaded.Next(true);
      };

      // because admob is soo special, you need to delay reload
      // otherwise they will complain with their Too many recently failed requests on some random ios devices.
      _interstitial.OnAdClosed += (object sender, EventArgs e) => {
        _onDone?.Invoke();
        _onDone = null;

        RequestAdWithDelay();
      };
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // Load the interstitial with the request.
      _interstitial.LoadAd(request);
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
