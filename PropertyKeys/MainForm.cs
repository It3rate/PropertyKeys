﻿using System;
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

namespace DataArcs
{
	public sealed partial class MainForm : Form
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
        private Button _bPause;

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

			BackColor = Color.Black;

			_b0 = new Button {Text = "Next", Location = new Point(720, 5), BackColor = Color.DarkGray};
			_b0.Click += B0_Click;
            Controls.Add(_b0);

            _bPause = new Button {Text = "Pause", BackColor = Color.DarkGray, Location = new Point(720, 30)};
            Controls.Add(_bPause);

            _ = Execute(null, 50);
        }
		
		public async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            _b0.Invalidate();
            _player = new Player(this);
            _bPause.Click += _player.OnPause;
            NextTest();
        }

		public static async void SkipTest()
		{
			await Task.Delay(50);
            MainForm form = (MainForm)Application.OpenForms[0];
			//form.Invoke((MethodInvoker)delegate {
				form.NextTest();
			//});
		}

        private static int _testCount = 10;
        private int _testIndex = 7;//_testCount;
        private void NextTest()
        {
	        _testIndex--;
	        if (_testIndex < 0)
	        {
		        _testIndex = _testCount - 1;
	        }
	        _player.Reset();
	        switch (_testIndex)
	        {
		        case 0:
			        _testScreen = new ImageCompressionTest(_player);
			        break;
		        case 1:
			        _testScreen = new BitmapTests(_player);
			        break;
		        case 2:
			        _testScreen = new AutomataTest(_player);
			        break;
                case 3:
			        _testScreen = new PhysicsTest(_player);
			        break;
                case 4:
			        _testScreen = new UserInputTest(_player);
			        break;
		        case 5:
			        _testScreen = new SerialInputTest(_player);
			        break;
                case 6:
			        _testScreen = new CompositeFlowerTest(_player);
			        break;
		        case 7:
			        _testScreen = new CompositeTestObjects(_player);
			        break;
		        case 8:
			        _testScreen = new CompositeChildSlideTest(_player);
			        break;
		        case 9:
			        _testScreen = new BitmapAutomataTest(_player);
			        break;
            }
            _player.Pause();
	        _testScreen.NextVersion();
            _player.Unpause();
        }

        private void B0_Click(object sender, EventArgs e)
        {
            NextTest();
        }

    }
}