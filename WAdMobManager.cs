using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace Wowsome.Ads {
  public class WAdMobManager : WAdsProviderBase {
    public override string Id => "admob";

    public override void InitAdsProvider(WAdSystem adSystem) {
      base.InitAdsProvider(adSystem);

      if (adSystem.IsDisabled.Value || isDisabled) return;

      MobileAds.SetiOSAppPauseOnBackground(true);
      MobileAds.Initialize(HandleInitCompleteAction);
    }

    void HandleInitCompleteAction(InitializationStatus initstatus) {
      MobileAdsEventExecutor.ExecuteInUpdate(() => {
        InitAds();
      });
    }
  }
}