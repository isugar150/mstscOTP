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
        private bool enableDispose = false;
        private iniProperties IniProperties = new iniProperties();
        private bool remoteSessionYn = false;
        private byte[] key = null;
        private string desktopID = "";

        //int sessionCnt = 0;
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
                    IniProperties.desktopID = pairs["mstscOTP"]["desktopID"].ToString2();
                    key = Encoding.UTF8.GetBytes(IniProperties.OTPKey);
                    desktopID = IniProperties.desktopID;
                }
            }
            catch (Exception) { }

            Thread t1 = new Thread(() =>
                isTerminalConnection()
            );
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();
        }

        private void isTerminalConnection()
        {
            try
            {
                //int preSessionCnt = 0;
                while (!this.IsDisposed)
                {
                    remoteSessionYn = SystemInformation.TerminalServerSession;
                    //sessionCnt = getSession(3389);
                    //Console.WriteLine("연결된 세션 수: " + sessionCnt);

                    Console.WriteLine("원격세션 연결 상태: " + remoteSessionYn);
                    Console.WriteLine("세션 Yn: " + isSession);
                    if (remoteSessionYn && !isSession)
                    {
                        using (var form = new EnterOTP(desktopID, key))
                        {
                            form.ShowDialog();

                            enableDispose = true;
                            Application.ExitThread();
                            Environment.Exit(0);
                        }
                    }
                    /* else if (sessionCnt != preSessionCnt)
                    {
                        isSession = false;
                    } */

                    //preSessionCnt = sessionCnt;
                    Thread.Sleep(1000);
                };
            } catch(Exception e1)
            {
                MessageBox.Show(this, e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* private int getSession(int number)
        {
            var ip = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            var cnt = 0;

            foreach (var tcp in ip.GetActiveTcpConnections()) // alternative: ip.GetActiveTcpListeners()
            {
                if (tcp.LocalEndPoint.Port == number || tcp.RemoteEndPoint.Port == number)
                {
                    ++cnt;
                }
            }
            return cnt;
        } */

        #region 버튼 이벤트

        private void button1_Click(object sender, EventArgs e)
        {
            if (!key.Equals("") || key != null)
            {
                using (var form = new EnterOTP(desktopID, key))
                {
                    form.ShowDialog();
                }
            }
            else MessageBox.Show(this, "OTP 등록 후 사용 가능합니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            key = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            String otpURI = new GoogleAuthenticator().GenerateQRCode(Environment.MachineName, key, 150, 150);
            webBrowser1.Navigate(otpURI);
            textBox1.Enabled = true;
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String genPin = new GoogleAuthenticator().GeneratePin(key);
            if (textBox1.Text.Equals(genPin))
            {
                textBox1.Enabled = false;
                IniProperties.OTPKey = Encoding.UTF8.GetString(key);
                IniProperties.desktopID = Environment.MachineName;

                IniFile setting = new IniFile();

                setting["mstscOTP"]["desktopID"] = Environment.MachineName;
                setting["mstscOTP"]["OTPKey"] = IniProperties.OTPKey;

                setting.Save("./mstscOTP.ini");
            }
            else
            {
                MessageBox.Show(this, "유효성 검사에 실패하였습니다. QR코드 재생성 후 다시시도하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button4.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String genPin = new GoogleAuthenticator().GeneratePin(key);
            MessageBox.Show(this, genPin, "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!enableDispose)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == false)
            {
                this.Visible = true;
                this.Activate();
            }
            else
                this.Visible = false;
        }

        private void 설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Activate();
        }

        private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("프로그램을 종료하시겠습니까?", "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                enableDispose = true;
                Application.ExitThread();
                Environment.Exit(0);
            }
        }
    }
}
