using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using DataArcs.SeriesData;

namespace DataArcs.Components.ExternalInput.Serial
{
	/// <summary>
    /// Detects X/Y touches on a resistive touch screen. More generally can be hooked up to any serial input from sensors.
    /// </summary>
    class SensorSerialPort : BaseComposite, ITimeable, IDisposable
    {
	    public float InterpolationT { get; set; }
	    public float StartTime { get; set; }
	    public Series Duration { get; } = new FloatSeries(1, 0);
	    public Series Delay { get; } = new FloatSeries(1, 0);
	    public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

        static SerialPort _serialPort;

        private float _x;
        private float _y;
        private IComposite _container;

        public static RectFSeries MainFrameRect
        {
	        get
	        {
		        RectFSeries result;
		        if (Application.OpenForms.Count > 0)
		        {
			        var form = Application.OpenForms[0];
			        result = new RectFSeries(0, 0, form.ClientSize.Width, form.ClientSize.Height);
		        }
		        else
		        {
			        result = new RectFSeries(0, 0, 800, 600);
		        }
		        return result;
	        }
        }

        public SensorSerialPort(IComposite container = null)
        {
	        _container = container;
	        Application.ApplicationExit += new EventHandler(ApplicationExit);

        }

        protected virtual void OnSensorUpdated(int x, int y)
        {
	        _x = (int)(y / 2.5) - 200;
	        _y = -((int)(x / 2.0) - 1000);
	        Debug.WriteLine(_x + ", " + _y);
        }

        public void StartListening(string portName)
        {
            _readThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();


            _serialPort.PortName = portName;

            _serialPort.BaudRate = 38400; //9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _readThread.Start();
        }

        private bool continueReading = true;
        private readonly byte[] bytes = new byte[10];
        private int bytesIndex = 0;
        private Thread _readThread;

        public void Read()
        {
            while (continueReading)
            {
                int len = _serialPort.BytesToRead;
                if (len > 0)
                {
                    try
                    {
                        _serialPort.Read(bytes, bytesIndex, len);
                        bytesIndex += len;
                        if (bytesIndex >= 10)
                        {
                            bytesIndex = 0;
                            int valX = BitConverter.ToInt16(bytes, 0);
                            int valY = BitConverter.ToInt16(bytes, 2);
                            OnSensorUpdated(valX, valY);
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
        {
            //todo: accomodate seriesT, maybe?
            ParametricSeries result;
            switch (propertyId)
            {
                case PropertyId.MouseX:
                    result = new ParametricSeries(1, _x / MainFrameRect.FloatDataAt(2));
                    break;
                case PropertyId.MouseY:
                    result = new ParametricSeries(1, _y / MainFrameRect.FloatDataAt(3));
                    break;
                case PropertyId.MouseLocationT:
                    result = new ParametricSeries(2, _x / MainFrameRect.FloatDataAt(2), _y / MainFrameRect.FloatDataAt(3));
                    break;
                case PropertyId.Mouse:
                case PropertyId.MouseLocation:
                default:
                    result = new ParametricSeries(2, _x / MainFrameRect.FloatDataAt(2), _y / MainFrameRect.FloatDataAt(3));
                    break;
            }
            return result;
        }

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
            return GetSeriesAtT(propertyId, 1f, null);
        }
        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
            FloatSeries result; // return current mouse atm, eventually will be able to scrub history if saved.
            switch (propertyId)
            {
                case PropertyId.MouseX:
                    result = new FloatSeries(1, _x);
                    break;
                case PropertyId.MouseY:
                    result = new FloatSeries(1, _y);
                    break;
                case PropertyId.MouseLocation:
                    result = new FloatSeries(2, _x, _y);
                    break;
                case PropertyId.EasedT:
                case PropertyId.EasedTCombined:
                case PropertyId.SampleAtT:
                case PropertyId.SampleAtTCombined:
                case PropertyId.MouseLocationT:
                    result = new ParametricSeries(2, _x / MainFrameRect.FloatDataAt(2), _y / MainFrameRect.FloatDataAt(3));
                    break;
                case PropertyId.Mouse:
                default:
                    result = new ParametricSeries(2, _x, _y);
                    break;
            }
            return result;
        }

        public override void OnActivate()
        {
	        base.OnActivate();
	        StartListening("COM16");
        }

        public override void OnDeactivate()
        {
			base.OnDeactivate();
			ReleaseUnmanagedResources();
        }

        public void Restart()
        {
	        throw new NotImplementedException();
        }

        public void Reverse()
        {
	        throw new NotImplementedException();
        }

        public void Pause()
        {

        }
        public void Resume()
        {

        }

        private void ApplicationExit(object sender, EventArgs e)
        {
	        ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
	        _readThread?.Abort();
	        _serialPort?.Close();
	        Application.ApplicationExit -= new EventHandler(ApplicationExit);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
	        GC.SuppressFinalize(this);
        }

        ~SensorSerialPort()
        {
	        ReleaseUnmanagedResources();
        }
    }
}
