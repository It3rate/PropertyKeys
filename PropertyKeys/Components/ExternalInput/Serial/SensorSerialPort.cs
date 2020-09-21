using System;
using System.Diagnostics;
using System.IO;
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
	    public double StartTime { get; set; }
	    public Series Duration { get; } = new FloatSeries(1, 0);
	    public Series Delay { get; } = new FloatSeries(1, 0);
	    public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

        static SerialPort _serialPort;

        private float _x;
        private float _y;
        private float _penPressure;

        private float _accelX;
        private float _accelY;
        private float _accelZ;
        private float _gyroX;
        private float _gyroY;
        private float _gyroZ;
        private float _magX;
        private float _magY;
        private float _magZ;
        private float _mpuTemperature;

        private float _humidity;
        private float _airPressure;
        private float _temperature;
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
	        _accelX = 0;
	        _accelY = 0;
	        _accelZ = 0;
        }

        public void StartListening(string portName)
        {
	        bool portExists = Array.Exists(SerialPort.GetPortNames(), e => e == portName);
	        if (portExists)
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
		        try
		        {
			        _serialPort.Open();
			        _serialPort.DiscardInBuffer();
			        _readThread.Start();
			        StartTimedEvent?.Invoke(this, EventArgs.Empty);
		        }
		        catch (IOException)
		        {
			        MainForm.SkipTest();
		        }
            }
	        else
	        {
		        MainForm.SkipTest();
            }
        }

        private bool continueReading = true;
        private readonly byte[] bytes = new byte[10];
        private Thread _readThread;

        public struct MPUVector
        {
	        public int X;
	        public int Y;
	        public int Z;
	        public override string ToString()
	        {
		        return "X:" + X + "\tY:" + X + "\tZ:" + Z;
	        }
        }

        public void Read()
        {
            while (continueReading)
            {
                int len = _serialPort.BytesToRead;
                if (len > 0)
                {
                    try
                    {
                        string s = _serialPort.ReadLine();
	                    //Debug.WriteLine(s);
                        if (s.StartsWith("RT:"))
                        {
	                        string[] blobs = s.Substring(3).Split(',');
	                        int[] vals = new int[blobs.Length];
	                        for (int i = 0; i < blobs.Length; i++)
	                        {
		                        vals[i] = Convert.ToInt32(blobs[i], 16); //int.Parse(blobs[i], NumberStyles.HexNumber);
	                        }

	                        OnTouchSensorUpdated(vals[0], vals[1], vals[2]);
                        }
                        else if (s.StartsWith("TPH:"))
                        {
	                        string[] blobs = s.Substring(4).Split(',');
	                        int[] vals = new int[blobs.Length];
	                        for (int i = 0; i < blobs.Length; i++)
	                        {
		                        vals[i] = Convert.ToInt16(blobs[i], 16);
	                        }
	                        OnHumiditySensorUpdated((float)vals[0], (float)vals[1], vals[2] / 100f);
                        }
                        else if (s.StartsWith("ATGM:"))
                        {
	                        string[] blobs = s.Substring(5).Split(',');
	                        int[] vals = new int[blobs.Length];
	                        for (int i = 0; i < blobs.Length; i++)
	                        {
		                        vals[i] = Convert.ToInt16(blobs[i], 16);
	                        }
	                        OnAccelSensorUpdated(
		                        new MPUVector { X = vals[0], Y = vals[1], Z = vals[2] },
		                        new MPUVector { X = vals[3], Y = vals[4], Z = vals[5] },
		                        new MPUVector { X = vals[6], Y = vals[7], Z = vals[8] },
		                        vals[9] / 100f
	                        );
                        }
                        StepTimedEvent?.Invoke(this, EventArgs.Empty);
                    }
                    catch (TimeoutException) { }
                    catch (FormatException) { }
                    catch (IndexOutOfRangeException) { }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        protected virtual void OnTouchSensorUpdated(int x, int y, int penPressure)
        {
	        _x = (int)(y / 2.5) - 200;
	        _y = -((int)(x / 2.0) - 1000);
	        _penPressure = (penPressure - 500) / 600.0f;
        }
        protected virtual void OnAccelSensorUpdated(MPUVector accel, MPUVector gyro, MPUVector mag, float mpuTemperature)
        {
            _accelX = (accel.X / 32767.0f) + 0.5f;
	        _accelY = (accel.Y / 32767.0f) + 0.5f;
	        _accelZ = (accel.Z / 32767.0f) + 0.5f;
            _gyroX = (gyro.X / 32767.0f) + 0.5f;
	        _gyroY = (gyro.Y / 32767.0f) + 0.5f;
	        _gyroZ = (gyro.Z / 32767.0f) + 0.5f;
            _magX = (mag.X / 32767.0f) + 0.5f;
	        _magY = (mag.Y / 32767.0f) + 0.5f;
	        _magZ = (mag.Z / 32767.0f) + 0.5f;
            _mpuTemperature = (float)mpuTemperature;
        }
        protected virtual void OnHumiditySensorUpdated(float temperature, float airPressure, float humidity)
        {
	        _humidity = humidity;
	        _airPressure = airPressure;
	        _temperature = temperature;
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
                case PropertyId.PenPressure:
	                result = new ParametricSeries(1, _penPressure);
	                break;
                case PropertyId.MpuAcceleration:
	                result = new ParametricSeries(3, _accelX, _accelY, _accelZ);
	                break;
                case PropertyId.MpuGyroscope:
	                result = new ParametricSeries(3, _gyroX, _gyroY, _gyroZ);
	                break;
                case PropertyId.MpuMagnetometer:
	                result = new ParametricSeries(3, _magX, _magY, _magZ);
	                break;
                case PropertyId.MpuAccelerationX:
	                result = new ParametricSeries(1, _accelX);
	                break;
                case PropertyId.MpuAccelerationY:
	                result = new ParametricSeries(1, _accelY);
	                break;
                case PropertyId.MpuAccelerationZ:
	                result = new ParametricSeries(1, _accelZ);
	                break;
                case PropertyId.MpuGyroscopeX:
	                result = new ParametricSeries(1, _gyroX);
	                break;
                case PropertyId.MpuGyroscopeY:
	                result = new ParametricSeries(1, _gyroY);
	                break;
                case PropertyId.MpuGyroscopeZ:
	                result = new ParametricSeries(1, _gyroZ);
	                break;
                case PropertyId.MpuMagnetometerX:
	                result = new ParametricSeries(1, _magX);
	                break;
                case PropertyId.MpuMagnetometerY:
	                result = new ParametricSeries(1, _magY);
	                break;
                case PropertyId.MpuMagnetometerZ:
	                result = new ParametricSeries(1, _magZ);
	                break;
                case PropertyId.MpuTemperature:
	                result = new ParametricSeries(1, _mpuTemperature);
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
			EndTimedEvent?.Invoke(this, EventArgs.Empty);
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
