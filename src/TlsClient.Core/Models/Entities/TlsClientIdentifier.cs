namespace TlsClient.Core.Models.Entities
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/profiles/profiles.go
    public sealed class TlsClientIdentifier
    {
        #region Chrome Profiles
        public static readonly TlsClientIdentifier Chrome103 = new TlsClientIdentifier("chrome_103");
        public static readonly TlsClientIdentifier Chrome104 = new TlsClientIdentifier("chrome_104");
        public static readonly TlsClientIdentifier Chrome105 = new TlsClientIdentifier("chrome_105");
        public static readonly TlsClientIdentifier Chrome106 = new TlsClientIdentifier("chrome_106");
        public static readonly TlsClientIdentifier Chrome107 = new TlsClientIdentifier("chrome_107");
        public static readonly TlsClientIdentifier Chrome108 = new TlsClientIdentifier("chrome_108");
        public static readonly TlsClientIdentifier Chrome109 = new TlsClientIdentifier("chrome_109");
        public static readonly TlsClientIdentifier Chrome110 = new TlsClientIdentifier("chrome_110");
        public static readonly TlsClientIdentifier Chrome111 = new TlsClientIdentifier("chrome_111");
        public static readonly TlsClientIdentifier Chrome112 = new TlsClientIdentifier("chrome_112");
        public static readonly TlsClientIdentifier Chrome116Psk = new TlsClientIdentifier("chrome_116_PSK");
        public static readonly TlsClientIdentifier Chrome116PskPq = new TlsClientIdentifier("chrome_116_PSK_PQ");
        public static readonly TlsClientIdentifier Chrome117 = new TlsClientIdentifier("chrome_117");
        public static readonly TlsClientIdentifier Chrome120 = new TlsClientIdentifier("chrome_120");
        public static readonly TlsClientIdentifier Chrome124 = new TlsClientIdentifier("chrome_124");
        public static readonly TlsClientIdentifier Chrome130Psk = new TlsClientIdentifier("chrome_130_PSK");
        public static readonly TlsClientIdentifier Chrome131 = new TlsClientIdentifier("chrome_131");
        public static readonly TlsClientIdentifier Chrome132 = new TlsClientIdentifier("chrome_132");
        public static readonly TlsClientIdentifier Chrome131Psk = new TlsClientIdentifier("chrome_131_PSK");
        public static readonly TlsClientIdentifier Chrome133 = new TlsClientIdentifier("chrome_133");
        public static readonly TlsClientIdentifier Chrome133Psk = new TlsClientIdentifier("chrome_133_PSK");
        #endregion

        #region Safari Profiles
        public static readonly TlsClientIdentifier Safari1561 = new TlsClientIdentifier("safari_15_6_1");
        public static readonly TlsClientIdentifier Safari160 = new TlsClientIdentifier("safari_16_0");
        public static readonly TlsClientIdentifier SafariIpad156 = new TlsClientIdentifier("safari_ipad_15_6");
        public static readonly TlsClientIdentifier SafariIos155 = new TlsClientIdentifier("safari_ios_15_5");
        public static readonly TlsClientIdentifier SafariIos156 = new TlsClientIdentifier("safari_ios_15_6");
        public static readonly TlsClientIdentifier SafariIos160 = new TlsClientIdentifier("safari_ios_16_0");
        public static readonly TlsClientIdentifier SafariIos170 = new TlsClientIdentifier("safari_ios_17_0");
        public static readonly TlsClientIdentifier SafariIos180 = new TlsClientIdentifier("safari_ios_18_0");
        public static readonly TlsClientIdentifier SafariIos185 = new TlsClientIdentifier("safari_ios_18_5");
        #endregion

        #region Firefox Profiles
        public static readonly TlsClientIdentifier Firefox102 = new TlsClientIdentifier("firefox_102");
        public static readonly TlsClientIdentifier Firefox104 = new TlsClientIdentifier("firefox_104");
        public static readonly TlsClientIdentifier Firefox105 = new TlsClientIdentifier("firefox_105");
        public static readonly TlsClientIdentifier Firefox106 = new TlsClientIdentifier("firefox_106");
        public static readonly TlsClientIdentifier Firefox108 = new TlsClientIdentifier("firefox_108");
        public static readonly TlsClientIdentifier Firefox110 = new TlsClientIdentifier("firefox_110");
        public static readonly TlsClientIdentifier Firefox117 = new TlsClientIdentifier("firefox_117");
        public static readonly TlsClientIdentifier Firefox120 = new TlsClientIdentifier("firefox_120");
        public static readonly TlsClientIdentifier Firefox123 = new TlsClientIdentifier("firefox_123");
        public static readonly TlsClientIdentifier Firefox132 = new TlsClientIdentifier("firefox_132");
        public static readonly TlsClientIdentifier Firefox133 = new TlsClientIdentifier("firefox_133");
        public static readonly TlsClientIdentifier Firefox135 = new TlsClientIdentifier("firefox_135");
        #endregion

        #region Opera Profiles
        public static readonly TlsClientIdentifier Opera89 = new TlsClientIdentifier("opera_89");
        public static readonly TlsClientIdentifier Opera90 = new TlsClientIdentifier("opera_90");
        public static readonly TlsClientIdentifier Opera91 = new TlsClientIdentifier("opera_91");
        #endregion

        #region MMS Profiles
        public static readonly TlsClientIdentifier MmsIos = new TlsClientIdentifier("mms_ios");
        public static readonly TlsClientIdentifier MmsIos1 = new TlsClientIdentifier("mms_ios_1");
        public static readonly TlsClientIdentifier MmsIos2 = new TlsClientIdentifier("mms_ios_2");
        public static readonly TlsClientIdentifier MmsIos3 = new TlsClientIdentifier("mms_ios_3");
        #endregion

        #region Mesh Profiles
        public static readonly TlsClientIdentifier MeshIos = new TlsClientIdentifier("mesh_ios");
        public static readonly TlsClientIdentifier MeshIos1 = new TlsClientIdentifier("mesh_ios_1");
        public static readonly TlsClientIdentifier MeshIos2 = new TlsClientIdentifier("mesh_ios_2");
        public static readonly TlsClientIdentifier MeshAndroid = new TlsClientIdentifier("mesh_android");
        public static readonly TlsClientIdentifier MeshAndroid1 = new TlsClientIdentifier("mesh_android_1");
        public static readonly TlsClientIdentifier MeshAndroid2 = new TlsClientIdentifier("mesh_android_2");
        #endregion

        #region Confirmed Profiles
        public static readonly TlsClientIdentifier ConfirmedIos = new TlsClientIdentifier("confirmed_ios");
        public static readonly TlsClientIdentifier ConfirmedAndroid = new TlsClientIdentifier("confirmed_android");
        #endregion

        #region OKHttp Profiles
        public static readonly TlsClientIdentifier Okhttp4Android7 = new TlsClientIdentifier("okhttp4_android_7");
        public static readonly TlsClientIdentifier Okhttp4Android8 = new TlsClientIdentifier("okhttp4_android_8");
        public static readonly TlsClientIdentifier Okhttp4Android9 = new TlsClientIdentifier("okhttp4_android_9");
        public static readonly TlsClientIdentifier Okhttp4Android10 = new TlsClientIdentifier("okhttp4_android_10");
        public static readonly TlsClientIdentifier Okhttp4Android11 = new TlsClientIdentifier("okhttp4_android_11");
        public static readonly TlsClientIdentifier Okhttp4Android12 = new TlsClientIdentifier("okhttp4_android_12");
        public static readonly TlsClientIdentifier Okhttp4Android13 = new TlsClientIdentifier("okhttp4_android_13");
        #endregion

        #region Other Profiles
        public static readonly TlsClientIdentifier ZalandoAndroidMobile = new TlsClientIdentifier("zalando_android_mobile");
        public static readonly TlsClientIdentifier ZalandoIosMobile = new TlsClientIdentifier("zalando_ios_mobile");
        public static readonly TlsClientIdentifier NikeIosMobile = new TlsClientIdentifier("nike_ios_mobile");
        public static readonly TlsClientIdentifier NikeAndroidMobile = new TlsClientIdentifier("nike_android_mobile");
        public static readonly TlsClientIdentifier Cloudscraper = new TlsClientIdentifier("cloudscraper");
        #endregion
        private readonly string Value = string.Empty;
        public override string ToString() => Value;

        public TlsClientIdentifier(string value)
        {
            Value = value;
        }
    }
}
