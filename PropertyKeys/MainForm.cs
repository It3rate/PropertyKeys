using PropertyKeys.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace PropertyKeys
{
    public partial class MainForm : Form
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private Circles circles;
        System.Timers.Timer timer;
        DateTime curTime;
        private float t = 0;

        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            circles = new Circles();

            curTime = DateTime.Now;
            timer = new System.Timers.Timer();
            timer.Elapsed += Tick;
            timer.Interval = 8;
            timer.Enabled = true;
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            t += 0.008f;// (e.SignalTime - curTime).Milliseconds / 3000f;
            curTime = e.SignalTime;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            circles.Draw(e.Graphics, t);
        }
    }
}
