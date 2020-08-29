using mstscOTP.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using mstscOTP.Lib;
using System.IO;
using mstscOTP.Interfaces;
using System.Diagnostics;

namespace mstscOTP
{
    public partial class Form1 : Form
    {
        #region 전역변수
        private iniProperties IniProperties = new iniProperties();
        private bool remoteSessionYn = false;
        private byte[] key = null;

        EventLog logListener = new EventLog("Security");
        #endregion

        #region 공유변수
        public static bool isSession = false;
        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IniFile pairs = new IniFile();
            try
            {
                if (new FileInfo("./mstscOTP.ini").Exists)
                {
                    pairs.Load("./mstscOTP.ini");
                    IniProperties.OTPKey = pairs["mstscOTP"]["OTPKey"].ToString2();
                    key = Encoding.UTF8.GetBytes(IniProperties.OTPKey);
                }
            }
            catch (Exception) { }

            //this.Visible = false;
            Thread t1 = new Thread(() =>
                isTerminalConnection()
            );
            t1.Start();
        }

        private void isTerminalConnection()
        {
            while (!this.IsDisposed)
            {
                remoteSessionYn = SystemInformation.TerminalServerSession;

                Console.WriteLine("원격세션 연결 상태: " + remoteSessionYn);
                Console.WriteLine("세션 Yn: " + isSession);
                if (remoteSessionYn && !isSession)
                {
                    using (var form = new EnterOTP(IniProperties.desktopID, key))
                    {
                        form.ShowDialog();
                    }
                }
                else if (!remoteSessionYn && isSession)
                {
                    isSession = false;
                }
                Thread.Sleep(1000);
            };
        }

        #region 버튼 이벤트

        private void button1_Click(object sender, EventArgs e)
        {
            remoteSessionYn = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            remoteSessionYn = false;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            key = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            String otpURI = new GoogleAuthenticator().GenerateQRCode(Environment.MachineName, key, 150, 150);
            webBrowser1.Navigate(otpURI);
            textBox1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String genPin = new GoogleAuthenticator().GeneratePin(key);
            if (textBox1.Text.Equals(genPin))
            {
                textBox1.Enabled = false;
                IniProperties.OTPKey = Encoding.UTF8.GetString(key);

                IniFile setting = new IniFile();

                setting["mstscOTP"]["desktopID"] = Environment.MachineName;
                setting["mstscOTP"]["OTPKey"] = IniProperties.OTPKey;

                setting.Save("./mstscOTP.ini");
            }
            else
            {
                MessageBox.Show(this, "유효성 검사에 실패하였습니다. QR코드 재생성 후 다시시도하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String genPin = new GoogleAuthenticator().GeneratePin(key);
            MessageBox.Show(this, genPin, "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
