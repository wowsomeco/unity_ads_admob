using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;

namespace Wowsome.Ads {
  public class WAdMobManager : WAdsProviderBase {
    public override string Id => "admob";

    public override void InitAdsProvider(WAdSystem adSystem) {
      base.InitAdsProvider(adSystem);

      if (adSystem.IsDisabled.Value || isDisabled) return;

      MobileAds.SetiOSAppPauseOnBackground(true);
      MobileAds.Initialize(HandleInitCompleteAction);

      ConsentRequestParameters request = new ConsentRequestParameters {
        /* TagForUnderAgeOfConsent = false,*/
      };

      // Check the current consent information status.
      ConsentInformation.Update(request, err => {
        if (null != err) {
          return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) => {
          if (ConsentInformation.CanRequestAds()) {
            InitAdmob();
          }
        });
      });
    }

    void InitAdmob() {
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