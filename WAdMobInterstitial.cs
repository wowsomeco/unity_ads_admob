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

      InterstitialAd.Load(data.GetUnitId(_provider.IsTestMode), new AdRequest(),
        (InterstitialAd ad, LoadAdError loadAdError) => {
          if (loadAdError != null) {
            Debug.Log("Interstitial ad failed to load with error: " + loadAdError.GetMessage());
            return;
          } else if (ad == null) {
            Debug.Log("Interstitial ad failed to load.");
            return;
          }

          IsLoaded.Next(true);

          _interstitial = ad;

          _interstitial.OnAdFullScreenContentClosed += () => {
            _onDone?.Invoke();
            _onDone = null;

            RequestAdWithDelay();
          };
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
