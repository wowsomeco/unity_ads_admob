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
      Banner
    }

    [Serializable]
    public class Model : WAdmobUnitModel {
      public AdSize Size {
        get {
          Dictionary<BannerSize, Func<AdSize>> handlers = new Dictionary<BannerSize, Func<AdSize>>();

          handlers[BannerSize.Banner] = () => AdSize.Banner;

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
    public float maxAspectRatio = 2f;

    IAdsProvider _provider;
    BannerView _banner;
    ObservableTimer _timer = null;

    public void InitAd(IAdsProvider provider) {
      _provider = provider;

      LoadAd();
    }

    public void HideAd() {
      if (IsLoaded.Value) {
        _banner.Hide();
      }
    }

    public bool ShowAd(Action onDone = null, Action onError = null) {
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
      float aspectRatio = ScreenExtensions.AspectRatio();

      Print.Info($"Cur Aspect Ratio : {aspectRatio}");

      if (aspectRatio > maxAspectRatio) {
        Print.Warn($"Admob banner not showing, aspect ratio is too big, {aspectRatio}");
        return;
      }

      _timer = new ObservableTimer(delayLoad);

      _timer.OnDone += () => {
        _timer = null;

        RequestAd();
      };
    }

    void RequestAd() {
      IsLoaded.Next(false);

      // load banner
      _banner = new BannerView(data.GetUnitId(_provider.IsTestMode), data.Size, data.position);

      _banner.OnBannerAdLoaded += () => {
        // hide initially on loaded
        _banner.Hide();

        IsLoaded.Next(true);
      };

      _banner.LoadAd(new AdRequest());
    }

    void Update() {
      _timer?.UpdateTimer(Time.deltaTime);
    }
  }
}