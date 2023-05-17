using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdMobOpenAd : MonoBehaviour, IAd {
    public int Priority => 1;
    public WObservable<bool> IsLoaded { get; private set; } = new WObservable<bool>(false);
    public AdType Type => AdType.OpenAd;
    public bool IsAvailable => null != _openAd && IsLoaded.Value && !HasExpired;
    public bool HasExpired => DateTime.Now > _expireTime;

    public WAdmobUnitModel data;
    public float delayLoad;

    IAdsProvider _provider;
    ObservableTimer _timer = null;
    AppOpenAd _openAd;
    DateTime _expireTime;

    public bool ShowAd(Action onDone = null) {
      if (!IsLoaded.Value) return false;

      _openAd.Show();

      return true;
    }

    public void InitAd(IAdsProvider provider) {
      _provider = provider;

      ScheduleLoad();
    }

    public void OnDisabled() {
      Clear();
    }

    void OnApplicationPause(bool isPaused) {
      if (isPaused && IsLoaded.Value) {
        ShowAd();
      }
    }

    void Load() {
      var adRequest = new AdRequest.Builder().Build();

      AppOpenAd.LoadAd(data.GetUnitId(_provider.IsTestMode), ScreenOrientation.AutoRotation, adRequest, (ad, error) => {
        // if error is not null, the load request failed.
        if (error != null || ad == null) {
          return;
        }

        _openAd = ad;

        _openAd.OnAdDidDismissFullScreenContent += (_, __) => {
          ScheduleLoad();
        };

        _expireTime = DateTime.Now + TimeSpan.FromHours(4);

        IsLoaded.Next(true);
      });
    }

    void ScheduleLoad() {
      IsLoaded.Next(false);

      _timer = new ObservableTimer(delayLoad);
      _timer.OnDone += () => {
        _timer = null;

        Load();
      };
    }

    void Clear() {
      if (IsLoaded.Value) {
        _openAd?.Destroy();
        _openAd = null;
      }
    }

    void Update() {
      float dt = Time.deltaTime;
      _timer?.UpdateTimer(dt);
    }
  }
}