using System;
using UnityEngine;

namespace Wowsome.Ads {
  [Serializable]
  public class WAdmobUnitModel {
    public string unitIdIOS;
    public string unitIdAndroid;
    public string testIdIOS;
    public string testIdAndroid;

    public string GetUnitId(bool isTestMode) {
      string unitId = string.Empty;

      if (Application.platform == RuntimePlatform.IPhonePlayer) {
        unitId = isTestMode ? testIdIOS : unitIdIOS;
      } else {
        unitId = isTestMode ? testIdAndroid : unitIdAndroid;
      }

      return unitId.Trim();
    }
  }
}