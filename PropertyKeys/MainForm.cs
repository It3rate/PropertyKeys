using System;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Tests.GraphicTests;

namespace DataArcs
{
	public partial class MainForm : Form
    {
        private Player _player;
        private Button _b0;

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

            _b0 = new Button();
            _b0.Text = "Next";
            _b0.Click += B0_Click;
            Controls.Add(_b0);
            Execute(null, 50);
        }

        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            _b0.Invalidate();
            _player = new Player(this);
            NextVersion();
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
            NextVersion();
        }
        private void NextVersion()
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
                    _player.AddElement(CompositeTestObjects.GetTest0(0, _player.CurrentMs, 1000f));
                    break;
                case 1:
                    _player.AddElement(CompositeTestObjects.GetTest1(0, _player.CurrentMs, 1000f));
                    break;
                case 2:
                    _player.AddElement(CompositeTestObjects.GetTest2(0, _player.CurrentMs, 1000f));
                    break;
                case 3:
                    _player.AddElement(CompositeTestObjects.GetTest3(0, _player.CurrentMs, 3000f));
                    break;
            }
        }
    }
}