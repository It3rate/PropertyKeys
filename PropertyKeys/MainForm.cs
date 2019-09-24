using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Tests.GraphicTests;
using DataArcs.Transitions;

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
			_b0.Location = new Point(700, 10);
            Controls.Add(_b0);
            _ = Execute(null, 50);
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

        private int _version = 2;
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

            BlendTransition comp;
            switch (_version)
            {
	            case 0:
		            comp = CompositeTestObjects.GetTest0(0, _player.CurrentMs, 1000f);
		            break;
	            case 1:
		            comp = CompositeTestObjects.GetTest1(0, _player.CurrentMs, 1000f);
		            break;
	            case 2:
		            comp = CompositeTestObjects.GetTest2(0, _player.CurrentMs, 1000f);
		            break;
	            default:
		            comp = CompositeTestObjects.GetTest3(0, _player.CurrentMs, 3000f);
		            break;
            }
		    _count = 0;
            comp.EndTransitionEvent += CompOnEndTransitionEvent;
            _player.AddElement(comp);
        }

		private int _count = 0;
        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
	        if (_count++ < 3)
	        {
		        BlendTransition bt = (BlendTransition) sender;
		        bt.Reverse();
		        bt.Restart();
	        }
	        else
	        {
		        NextVersion();
            }
        }
    }
}