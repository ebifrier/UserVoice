using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserVoice.Views
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.elementHost1.Child = new MainControl();

            this.FormClosing += (sender, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();

                    AppSettings.Save();
                }
            };

            this.FormClosed += (sender, e) =>
            {
                Global.MainForm = null;
            };

            Global.MainForm = this;
        }
    }
}
