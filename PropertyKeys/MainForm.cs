using DataArcs.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace DataArcs
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

        private int version = 0;

        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            Button b0 = new Button();
            b0.Text = "Next";
            b0.Click += B0_Click;

            this.Controls.Add(b0);

            circles = new Circles(version);

            curTime = DateTime.Now;
            timer = new System.Timers.Timer();
            timer.Elapsed += Tick;
            timer.Interval = 8;
            timer.Enabled = true;
        }

        private void B0_Click(object sender, EventArgs e)
        {
            version++;
            if(version >= Circles.versionCount)
            {
                version = 0;
            }
            circles = new Circles(version);
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            t += version == 2 ? 0.008f : 0.02f;// (e.SignalTime - curTime).Milliseconds / 3000f;
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
