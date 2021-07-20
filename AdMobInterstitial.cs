using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;

namespace Wowsome.Ads {
  public class AdMobInterstitial : MonoBehaviour, IInterstitial {
    [Serializable]
    public struct Model {
      public string unitIdIOS;
      public string unitIdAndroid;
      public string unitIdAmazon;
      public int showOrder;

      public string UnitId {
        get {
          string unitId = PlatformUtil.GetStringByPlatform(unitIdIOS, unitIdAndroid, unitIdAmazon);
          return unitId.Trim();
        }
      }
    }

    public Model data;

    InterstitialAd _interstitial;
    Timer _delayLoad = null;

    #region IInterstitial
    public int Order => data.showOrder;

    public bool ShowInterstitial() {
      if (null != _delayLoad) return false;

      if (_interstitial.IsLoaded()) {
        Debug.Log("show admob ads");
        _interstitial.Show();
        return true;
      }
      ReloadAd();
      return false;
    }
    #endregion

    public void InitInterstitial() {
      // load ad
      ReloadAd();
    }

    public void UpdateInterstitial(float dt) {
      if (null != _delayLoad && !_delayLoad.UpdateTimer(dt)) {
        _delayLoad = null;
        ReloadAd();
      }
    }

    void InitInternal() {
      _interstitial = new InterstitialAd(data.UnitId);
      // because admob is soo special, you need to delay reload
      // otherwise they will complain with their Too many recently failed requests on some random ios devices.
      _interstitial.OnAdClosed += (object sender, System.EventArgs e) => {
        _delayLoad = new Timer(3f);
      };
    }

    void ReloadAd() {
      Debug.Log("load admob ads");
#if UNITY_ANDROID
      if (null == _interstitial) InitInternal();
#elif UNITY_IPHONE
      // re init intestitial on ios because it's a one time object
      InitInternal();
#endif
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // Load the interstitial with the request.
      _interstitial.LoadAd(request);
    }
  }
}
