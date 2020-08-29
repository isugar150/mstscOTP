using mstscOTP.Lib;

namespace mstscOTP.Interfaces
{
    internal interface iniproperties
    {
        string desktopID { get; set; }
        string OTPKey { get; set; }
    }

    public class iniProperties : iniproperties
    {
        private string _desktopID;
        private string _OTPKey;
        public string desktopID { get { return _desktopID; } set { _desktopID = value; } }
        public string OTPKey { get { return _OTPKey; } set { _OTPKey = value; } }
    }
}
