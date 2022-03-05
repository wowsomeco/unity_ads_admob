using System;

namespace Wowsome.Ads {
  [Serializable]
  public class WAdmobUnitModel {
    public string unitIdIOS;
    public string unitIdAndroid;
    public string testIdIOS;
    public string testIdAndroid;

    public string GetUnitId(bool isTestMode) {
      string unitId = PlatformUtil.GetStringByPlatform(
        isTestMode ? testIdIOS : unitIdIOS,
        isTestMode ? testIdAndroid : unitIdAndroid
      );

      return unitId.Trim();
    }
  }
}