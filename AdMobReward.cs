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

    RewardedAd _rewardedAd;

    public void InitReward() {
      _rewardedAd = new RewardedAd(data.UnitId);

      _rewardedAd.OnUserEarnedReward += (sender, e) => {
        OnRewarded?.Invoke();
      };

      _rewardedAd.OnAdClosed += (sender, e) => {
        LoadAd();
      };

      _rewardedAd.OnAdFailedToLoad += (sender, args) => {
        Debug.Log("admob rewarded HandleFailedToReceiveAd event received with message: " + args.ToString());
      };

      LoadAd();
    }

    public bool ShowReward() {
      if (_rewardedAd.IsLoaded()) {
        _rewardedAd.Show();

        return true;
      }

      return false;
    }

    void LoadAd() {
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // load video
      _rewardedAd.LoadAd(request);
    }
  }
}

