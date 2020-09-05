using Microsoft.Win32;
using mstscOTP.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mstscOTP.Forms
{
    public partial class EnterOTP : Form
    {
        private bool enableDispose = false;
        private String desktopID = null;
        private byte[] key = null;

        public EnterOTP(String desktopID, byte[] key)
        {
            InitializeComponent();
            this.desktopID = desktopID;
            this.key = key;
        }

        private void EnterOTP_Load(object sender, EventArgs e)
        {
            //Process.Start("taskkill", "/f /im explorer.exe");
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            label2.Text = "DesktopID: " + desktopID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //테스트 코드
            Form1.isSession = true;
            enableDispose = true;
            this.Dispose();
            /*String genPin = new GoogleAuthenticator().GeneratePin(key);
            if (textBox1.Text.Equals(genPin))
            {
                Form1.isSession = true;
                enableDispose = true;
                this.Dispose();
            }
            else
            {
                MessageBox.Show(this, "OTP가 일치하지 않습니다. 확인 후 다시시도하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void EnterOTP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!enableDispose)
            {
                e.Cancel = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }
    }
}
