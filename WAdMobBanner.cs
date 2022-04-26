using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobBanner : MonoBehaviour, IAd {
    [Serializable]
    public enum BannerSize {
      Banner, SmartBanner
    }

    [Serializable]
    public class Model : WAdmobUnitModel {
      public AdSize Size {
        get {
          Dictionary<BannerSize, Func<AdSize>> handlers = new Dictionary<BannerSize, Func<AdSize>>();

          handlers[BannerSize.Banner] = () => AdSize.Banner;
          handlers[BannerSize.SmartBanner] = () => AdSize.SmartBanner;

          return handlers[size]();
        }
      }

      public AdPosition position;
      public BannerSize size;
    }

    public int Priority => priority;
    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.Banner;

    public Model data;
    public float delayLoad;
    public int priority;

    IAdsProvider _provider;
    BannerView _banner;
    ObservableTimer _timer = null;

    public void InitAd(IAdsProvider provider) {
      _provider = provider;

      LoadAd();
    }

    public void HideAd() {
      // this stupid banner seems unable to toggle visibility,
      // the workaround is when hiding, we destroy it
      // then reload it again
      if (IsLoaded.Value) {
        _banner.Hide();
        _banner.Destroy();

        RequestAd();
      }
    }

    public bool ShowAd(Action onDone = null) {
      if (IsLoaded.Value) {
        _banner.Show();

        return true;
      }

      return false;
    }

    public void OnDisabled() {
      HideAd();
    }

    void LoadAd() {
      _timer = new ObservableTimer(delayLoad);

      _timer.OnDone += () => {
        _timer = null;

        RequestAd();
      };
    }

    void RequestAd() {
      IsLoaded.Next(false);

      // load banner
      AdRequest request = new AdRequest.Builder().Build();

      _banner = new BannerView(data.GetUnitId(_provider.IsTestMode), data.Size, data.position);

      _banner.OnAdLoaded += (sender, args) => {
        // hide initially on loaded
        _banner.Hide();

        IsLoaded.Next(true);
      };

      _banner.LoadAd(request);
    }

    void Update() {
      _timer?.UpdateTimer(Time.deltaTime);
    }
  }
}
