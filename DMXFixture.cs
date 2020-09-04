using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Daniels.Common;

namespace Daniels.Lighting
{
    public class DMXFixture: LightFixture
    {
        protected DMXFixtureConfig _config;
        private BasicTriList _transport;

        public DMXFixtureProfileConfig Profile { get { return _config.Profile; } }

        public DMXFixture(LightGroup group, DMXFixtureConfig config, BasicTriList transport):base(config.Id, config.Name, group)
        {
            _config = config.Clone() as DMXFixtureConfig;
            _transport = transport;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine("\tConfig: " + _config.ToString());
            return sb.ToString();
        }

        public override bool Muted
        {
            get { return base.Muted; }
            set
            {
                if (value != base.Muted)
                {
                    base.Muted = value;
                    if(this.EffectiveMute)
                        setTransportValue(DMXChannel.Intensity, 0);
                    else
                        setTransportValue(DMXChannel.Intensity, EffectiveIntensity);
                }
            }
        }

        public override ushort Intensity
        {
            get { return base.Intensity; }
            set
            {
                if (value != base.Intensity)
                {
                    base.Intensity = value;
                    if (!this.EffectiveMute)
                        setTransportValue(DMXChannel.Intensity, EffectiveIntensity);
                }
            }
        }

        protected override void masterIntensityChanged(object sender, ReadOnlyEventArgs<ushort> e)
        {
            if (!this.EffectiveMute)
                setTransportValue(DMXChannel.Intensity, EffectiveIntensity);
            base.masterIntensityChanged(sender, e);
        }

        protected override void masterMuteChanged(object sender, ReadOnlyEventArgs<bool> e)
        {
            if (this.EffectiveMute)
                setTransportValue(DMXChannel.Intensity, 0);
            else
                setTransportValue(DMXChannel.Intensity, EffectiveIntensity);
            base.masterMuteChanged(sender, e);
        }

        protected void setTransportValue(DMXChannel channel, ushort value)
        {
            ushort transportScaledValue = (ushort)(256*value/65536);
            uint transportChannel = (uint)(_config.BaseDMXChannel + Profile.DMXChannels[channel]-1);
            _transport.UShortInput[transportChannel].UShortValue = transportScaledValue;
            CrestronConsole.PrintLine("DMXFixture(\"{0}\"): setTransportValue: {1}({2}):{3}", this.Name, channel.ToString(), transportChannel, transportScaledValue);
        }

    }
}