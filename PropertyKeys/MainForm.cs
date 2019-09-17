using System;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Tests.GraphicTests;

namespace DataArcs
{
	public partial class MainForm : Form
	{
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		//private Circles circles;
		private System.Timers.Timer timer;
		private DateTime curTime;
		private float t = 0;

		private int version = 3;

		public MainForm()
		{
			InitializeComponent();
			DoubleBuffered = true;

			var b0 = new Button();
			b0.Text = "Next";
			b0.Click += B0_Click;

			Controls.Add(b0);

			Player player = new Player(this);
			player.AddElement(CompositeTestObjects.GetTest0());
			player.AddElement(CompositeTestObjects.GetTest3());

            //circles = new Circles(version);

            curTime = DateTime.Now;
			timer = new System.Timers.Timer();
			timer.Elapsed += Tick;
			timer.Interval = 8;
			timer.Enabled = true;
            this.Paint += MainForm_Paint;
		}

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
	        //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
	        //circles.Draw(e.Graphics, t);
        }

        private void B0_Click(object sender, EventArgs e)
		{
			version++;
			if (version >= Circles.versionCount)
			{
				version = 0;
			}

			//circles = new Circles(version);
		}

		private void Tick(object sender, ElapsedEventArgs e)
		{
			t += version == 2 ? 0.008f : 0.01f; // (e.SignalTime - curTime).Milliseconds / 3000f;
			curTime = e.SignalTime;
			Invalidate();
		}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//	e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		//	circles.Draw(e.Graphics, t);
		//}
	}
}