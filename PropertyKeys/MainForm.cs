using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;
using DataArcs.Tests.GraphicTests;
using DataArcs.Transitions;

namespace DataArcs
{
	public partial class MainForm : Form
    {
        // Sequencer
        // convert to cuda
        // hook up to commands
        // definitions/instances
        // basic UI
        // Add algorithmic step sampler (physics, navier stokes, runge kutta, reaction diffusion etc)
        // add ML in simple 'bacteria' test
        // matrix support

	    private Player _player;
	    private ITestScreen _testScreen;
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
			_b0.Location = new Point(700, 10);
            Controls.Add(_b0);
            _ = Execute(null, 50);
        }

        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            _b0.Invalidate();
            _player = new Player(this);
            NextTest();
        }

        private int testIndex = -1;
        private int testCount = 2;
        private void NextTest()
        {
	        testIndex++;
	        if (testIndex >= testCount)
	        {
		        testIndex = 0;
	        }
	        _player.Reset();
	        switch (testIndex)
	        {
		        case 0:
			        _testScreen = new CompositeTest2(_player);
                    break;
		        case 1:
					_testScreen = new CompositeTestObjects(_player);
				break;
            }
	        _testScreen.NextVersion();
        }

        private void B0_Click(object sender, EventArgs e)
        {
	        NextTest();
        }

    }
}