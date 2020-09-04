using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;
using Daniels.Common;

namespace Daniels.Lighting
{
    public class DMXPTZFixture : DMXFixture
    {
        public DMXPTZFixture(LightGroup group, DMXFixtureConfig config, BasicTriList transport)
            : base(group, config, transport)
        {
            Shutter = 65535;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine("\tPan: " + Pan);
            sb.AppendLine("\tTilt: " + Tilt);
            sb.AppendLine("\tZoom: " + Zoom + " (" + (1f * Zoom  / ushort.MaxValue).ToString("P2") + ")");
            sb.AppendLine("\tIris: " + Iris + " (" + (1f * Iris / ushort.MaxValue).ToString("P2") + ")");
            sb.AppendLine("\tFocus: " + Focus + " (" + (1f * Focus / ushort.MaxValue).ToString("P2") + ")");
            sb.AppendLine("\tBlade1: " + Blade1);
            sb.AppendLine("\tBlade1Rotate: " + Blade1Rotate);
            sb.AppendLine("\tBlade2: " + Blade2);
            sb.AppendLine("\tBlade2Rotate: " + Blade2Rotate);
            sb.AppendLine("\tBlade3: " + Blade3);
            sb.AppendLine("\tBlade3Rotate: " + Blade3Rotate);
            sb.AppendLine("\tBlade4: " + Blade4);
            sb.AppendLine("\tBlade4Rotate: " + Blade4Rotate);
            return sb.ToString();
        }

        #region Pan
        /*
         * Pan
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> PanChanged;
        protected virtual void OnPanChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = PanChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _pan;
        [JsonProperty]
        public ushort Pan
        {
            get { return _pan; }
            set
            {
                _pan = value;
                setTransportValue(DMXChannel.Pan, value);
                OnPanChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Pan

        #region Tilt
        /*
         * Tilt
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> TiltChanged;
        protected virtual void OnTiltChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = TiltChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _tilt;
        [JsonProperty]
        public ushort Tilt
        {
            get { return _tilt; }
            set
            {
                _tilt = value;
                setTransportValue(DMXChannel.Tilt, value);
                OnTiltChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Tilt
        
        #region Zoom
        /*
         * Zoom
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> ZoomChanged;
        protected virtual void OnZoomChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = ZoomChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _zoom;
        [JsonProperty]
        public ushort Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                setTransportValue(DMXChannel.Zoom, value);
                OnZoomChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Zoom

        #region Iris
        /*
         * Iris
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> IrisChanged;
        protected virtual void OnIrisChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = IrisChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _iris;
        [JsonProperty]
        public ushort Iris
        {
            get { return _iris; }
            set
            {
                _iris = value;
                setTransportValue(DMXChannel.Iris, value);
                OnIrisChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Iris

        #region Focus
        /*
         * Focus
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> FocusChanged;
        protected virtual void OnFocusChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = FocusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _focus;
        [JsonProperty]
        public ushort Focus
        {
            get { return _focus; }
            set
            {
                _focus = value;
                setTransportValue(DMXChannel.Focus, value);
                OnFocusChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Focus

        #region Blade1
        /*
         * Blade1
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade1Changed;
        protected virtual void OnBlade1Changed(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade1Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade1;
        [JsonProperty]
        public ushort Blade1
        {
            get { return _blade1; }
            set
            {
                _blade1 = value;
                setTransportValue(DMXChannel.Blade1, value);
                OnBlade1Changed(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade1

        #region Blade1Rotate
        /*
         * Blade1Rotate
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade1RotateChanged;
        protected virtual void OnBlade1RotateChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade1RotateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade1Rotate;
        [JsonProperty]
        public ushort Blade1Rotate
        {
            get { return _blade1Rotate; }
            set
            {
                _blade1Rotate = value;
                setTransportValue(DMXChannel.Blade1Rotate, value);
                OnBlade1RotateChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade1Rotate

        #region Blade2
        /*
         * Blade2
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade2Changed;
        protected virtual void OnBlade2Changed(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade2Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade2;
        [JsonProperty]
        public ushort Blade2
        {
            get { return _blade2; }
            set
            {
                _blade2 = value;
                setTransportValue(DMXChannel.Blade2, value);
                OnBlade2Changed(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade2

        #region Blade2Rotate
        /*
         * Blade2Rotate
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade2RotateChanged;
        protected virtual void OnBlade2RotateChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade2RotateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade2Rotate;
        [JsonProperty]
        public ushort Blade2Rotate
        {
            get { return _blade2Rotate; }
            set
            {
                _blade2Rotate = value;
                setTransportValue(DMXChannel.Blade2Rotate, value);
                OnBlade2RotateChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade2Rotate

        #region Blade3
        /*
         * Blade3
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade3Changed;
        protected virtual void OnBlade3Changed(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade3Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade3;
        [JsonProperty]
        public ushort Blade3
        {
            get { return _blade3; }
            set
            {
                _blade3 = value;
                setTransportValue(DMXChannel.Blade3, value);
                OnBlade3Changed(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade3

        #region Blade3Rotate
        /*
         * Blade3Rotate
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade3RotateChanged;
        protected virtual void OnBlade3RotateChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade3RotateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade3Rotate;
        [JsonProperty]
        public ushort Blade3Rotate
        {
            get { return _blade3Rotate; }
            set
            {
                _blade3Rotate = value;
                setTransportValue(DMXChannel.Blade3Rotate, value);
                OnBlade3RotateChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade3Rotate

        #region Blade4
        /*
         * Blade4
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade4Changed;
        protected virtual void OnBlade4Changed(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade4Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade4;
        [JsonProperty]
        public ushort Blade4
        {
            get { return _blade4; }
            set
            {
                _blade4 = value;
                setTransportValue(DMXChannel.Blade4, value);
                OnBlade4Changed(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade4

        #region Blade4Rotate
        /*
         * Blade4Rotate
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> Blade4RotateChanged;
        protected virtual void OnBlade4RotateChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = Blade4RotateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _blade4Rotate;
        [JsonProperty]
        public ushort Blade4Rotate
        {
            get { return _blade4Rotate; }
            set
            {
                _blade4Rotate = value;
                setTransportValue(DMXChannel.Blade4Rotate, value);
                OnBlade4RotateChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Blade43Rotate

        #region Shutter
        /*
         * Shutter
         */
        public event EventHandler<ReadOnlyEventArgs<ushort>> ShutterChanged;
        protected virtual void OnShutterChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = ShutterChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort _shutter;
        [JsonProperty]
        public ushort Shutter
        {
            get { return _shutter; }
            set
            {
                _shutter = value;
                setTransportValue(DMXChannel.Shutter, value);
                OnShutterChanged(new ReadOnlyEventArgs<ushort>(value));
            }
        }
        #endregion Shutter



    }
}