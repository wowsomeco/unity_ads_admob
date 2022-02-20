using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using Wowsome.Chrono;

namespace Wowsome.Ads {
  public class AdMobBanner : MonoBehaviour, IBanner {
    [Serializable]
    public enum BannerSize {
      Banner, SmartBanner
    }

    [Serializable]
    public struct Model {
      public string unitIdIOS;
      public string unitIdAndroid;
      public string unitIdAmazon;
      public AdPosition position;
      public BannerSize size;

      public string UnitId {
        get {
          string unitId = PlatformUtil.GetStringByPlatform(unitIdIOS, unitIdAndroid, unitIdAmazon);
          return unitId.Trim();
        }
      }

      public AdSize Size {
        get {
          Dictionary<BannerSize, Func<AdSize>> handlers = new Dictionary<BannerSize, Func<AdSize>>();

          handlers[BannerSize.Banner] = () => AdSize.Banner;
          handlers[BannerSize.SmartBanner] = () => AdSize.SmartBanner;

          return handlers[size]();
        }
      }
    }

    public Model data;

    BannerView _banner;
    bool _loading = false;
    bool _loaded = false;
    Action _onLoaded;
    Timer _loadTimer = null;

    public void InitBanner(Action onLoaded) {
      _onLoaded = onLoaded;
      LoadBanner();
    }

    public void UpdateBanner(float dt) {
      if (null != _loadTimer && !_loadTimer.UpdateTimer(dt)) {
        _loadTimer = null;
        LoadBanner();
      }
    }

    public void ShowBanner(bool flag) {
      // this stupid banner seems unable to toggle visibility,
      // the workaround is when hiding, we destroy it
      // then reload it again
      if (!flag) {
        if (_loaded) {
          _banner.Hide();
          _banner.Destroy();
          _loaded = false;
        }
      } else {
        if (_loaded) {
          _banner.Show();
        } else if (!_loading) {
          LoadBanner();
        }
      }
    }

    void LoadBanner() {
      _loading = true;
      MobileAds.Initialize(OnInit);
    }

    void OnInit(InitializationStatus initstatus) {
      // Callbacks from GoogleMobileAds are not guaranteed to be called on
      // main thread.
      // In this example we use MobileAdsEventExecutor to schedule these calls on
      // the next Update() loop.
      MobileAdsEventExecutor.ExecuteInUpdate(() => {
        //load banner
        AdRequest request = new AdRequest.Builder().Build();
        _banner = new BannerView(data.UnitId, data.Size, data.position);
        _banner.OnAdLoaded += (sender, args) => {
          _loading = false;
          _loaded = true;
          _onLoaded();
        };
        _banner.OnAdFailedToLoad += (sender, e) => {
          // on failed, delay 2 secs before re-loading again.
          _loadTimer = new Timer(2f);
        };
        _banner.LoadAd(request);
      });
    }
  }
}
