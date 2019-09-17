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
		private readonly Player _player;

		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		public MainForm()
		{
			InitializeComponent();
			DoubleBuffered = true;

            var b0 = new Button();
			b0.Text = "Next";
			b0.Click += B0_Click;
			Controls.Add(b0);

			_player = new Player(this);
			B0_Click(null, null);
		}

		// Sequencer
		// convert to cuda
		// hook up to commands
		// definitions/instances
		// basic UI
		// Add algorithmic step sampler (physics, navier stokes, runge kutta, reaction diffusion etc)
		// add ML in simple 'bacteria' test
		// matrix support

        private int _version = -1;
        private void B0_Click(object sender, EventArgs e)
		{
			_version++;
			if (_version >= CompositeTestObjects.VersionCount)
			{
				_version = 0;
			}

			_player.Clear();
			switch (_version)
			{
				case 0:
					_player.AddElement(CompositeTestObjects.GetTest0());
					break;
				case 1:
					_player.AddElement(CompositeTestObjects.GetTest1());
					break;
				case 2:
					_player.AddElement(CompositeTestObjects.GetTest2());
					break;
				case 3:
					_player.AddElement(CompositeTestObjects.GetTest3());
					break;
			}
        }
	}
}