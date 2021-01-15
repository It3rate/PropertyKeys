using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Motive.Tests.GraphicTests;

namespace Motive
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

	    private Runner _runner;
	    private ITestScreen _testScreen;
        private Button _b0;
        private Button _bPause;
        private TrackBar _slider0;
        private TrackBar _slider1;

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

            _slider0 = new TrackBar { Location = new Point(20, 320), Size = new Size(150, 20), Minimum = 0, Maximum = 100, TickFrequency = 10};
            _slider0.Scroll += _slider0_Scroll;
            Controls.Add(_slider0);

            _slider1 = new TrackBar { Location = new Point(20, 370), Size = new Size(150, 20), Minimum = 0, Maximum = 100, TickFrequency = 10 };
            _slider1.Scroll += _slider1_Scroll;
            Controls.Add(_slider1);

            _slider0.Hide();
            _slider1.Hide();

            _ = Execute(null, 50);
        }

        private void _slider0_Scroll(object sender, EventArgs e)
        {
	        _runner.ExternalValue0.FloatDataRef[0] = _slider0.Value / 100f;
        }

        private void _slider1_Scroll(object sender, EventArgs e)
        {
	        _runner.ExternalValue1.FloatDataRef[0] = _slider1.Value / 100f;
        }

        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            _b0.Invalidate();
            _runner = new Runner(this);
            _bPause.Click += _runner.OnPause;
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

        private static int _testCount = 9;//13;
        private int _testIndex = 0;//_testCount;
        private void NextTest()
        {
            _testIndex--;
	        if (_testIndex < 0)
	        {
		        _testIndex = _testCount - 1;
	        }
	        _runner.Reset();
	        switch (_testIndex)
	        {
		        case 0:
			        _testScreen = new ImageCompressionTest(_runner);
			        break;
		        case 1:
			        _testScreen = new BitmapTests(_runner);
			        break;
		        case 2:
			        _testScreen = new AutomataTest(_runner);
			        break;
                case 3:
			        _testScreen = new PhysicsTest(_runner);
			        break;
                case 4:
			        _testScreen = new UserInputTest(_runner);
			        break;
		        case 5:
			        _testScreen = new SerialInputTest(_runner);
			        break;
                case 6:
			        _testScreen = new CompositeFlowerTest(_runner);
			        break;
		        case 7:
			        _testScreen = new CompositeTestObjects(_runner);
			        break;
		        case 8:
			        _testScreen = new CompositeChildSlideTest(_runner);
			        break;
		        case 9:
			        _testScreen = new BitmapAutomataTest(_runner);
			        break;
		        case 10:
			        _slider0.Show();
			        _slider1.Show();
			        _testScreen = new CommandTest(_runner);
			        break;
		        case 11:
			        _testScreen = new BezierTest(_runner);
			        break;
		        case 12:
			        _testScreen = new LayoutTest(_runner);
			        break;
            }
            _runner.Pause();
	        _testScreen.NextVersion();
            _runner.Unpause();
        }

        private void B0_Click(object sender, EventArgs e)
        {
	        _slider0.Hide();
	        _slider1.Hide();

            NextTest();
        }

    }
}