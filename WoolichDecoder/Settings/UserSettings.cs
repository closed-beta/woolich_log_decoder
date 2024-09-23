using System.Configuration;

namespace WoolichDecoder.Settings
{
    internal class UserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        // [DefaultSettingValue("white")]
        public string LogDirectory
        {
            get
            {
                return ((string)this["LogDirectory"]);
            }
            set
            {
                this["LogDirectory"] = (string)value;
            }
        }
        [UserScopedSetting()]
        [DefaultSettingValue("10.2")]
        public string AFRdivisor
        {
            get { return (string)this["AFRdivisor"]; }
            set { this["AFRdivisor"] = value; }
        }
        [UserScopedSetting()]
        [DefaultSettingValue("1200")]
        public string idleRPM
        {
            get { return (string)this["idleRPM"]; }
            set { this["idleRPM"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("1000")]
        public string minRPM
        {
            get { return (string)this["minRPM"]; }
            set { this["minRPM"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("4500")]
        public string maxRPM
        {
            get { return (string)this["maxRPM"]; }
            set { this["maxRPM"] = value; }
        }

    }
}
