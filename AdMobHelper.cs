using UnityEngine;

namespace Wowsome.Ads {
  /// <summary>
  /// Automagically sets GoogleMobileAdsSettings.adMobAndroidAppId according to the AndroidPlatform defined in WAppSettings.cs  
  /// Useful when you have different admob app id for each android platform e.g. 1 for google and 1 for amazon.
  /// 
  /// When 'Switch Android Platform' is clicked, it
  /// 1. finds WAppSettings.cs defined in appSettingsPath, updates the AndroidPlatform from google to amazon and vice versa
  /// 2. finds GoogleMobileAdsSettings.asset defined in admobSettingsPath, updates the admob app id accordingly.
  /// </summary>
  [CreateAssetMenu(fileName = "AdmobConfig", menuName = "Wowsome/Admob Config")]
  public class AdMobHelper : ScriptableObject {
    public string appSettingsPath;
    public string admobSettingsPath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
    public string googleAppId;
    public string amazonAppId;
    public string googlePackageName;
    public string amazonPackageName;
  }
}

