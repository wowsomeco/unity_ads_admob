using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Wowsome.Ads {
  public class AdMobReward : MonoBehaviour, IReward {
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
      public int showOrder;
    }

    public Action OnRewarded { get; set; }
    public int Order => data.showOrder;

    public Model data;

    public void InitReward() {
      RewardBasedVideoAd.Instance.OnAdRewarded += (sender, e) => {
        OnRewarded?.Invoke();
      };

      RewardBasedVideoAd.Instance.OnAdClosed += (sender, e) => {
        LoadAd();
      };

      RewardBasedVideoAd.Instance.OnAdFailedToLoad += (sender, args) => {
        Debug.Log("admob rewarded HandleFailedToReceiveAd event received with message: " + args.Message);
      };

      LoadAd();
    }

    public bool ShowReward() {
      if (RewardBasedVideoAd.Instance.IsLoaded()) {
        RewardBasedVideoAd.Instance.Show();

#if UNITY_EDITOR
        OnRewarded?.Invoke();
#endif

        return true;
      }

      LoadAd();
      return false;
    }

    void LoadAd() {
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // load video
      RewardBasedVideoAd.Instance.LoadAd(request, data.UnitId);
    }
  }
}

