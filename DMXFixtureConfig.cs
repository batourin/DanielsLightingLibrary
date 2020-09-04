using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace Daniels.Lighting
{
    public class DMXFixtureConfig : ICloneable
    {
        public uint Id;
        public string Name;

        //[JsonObject(IsReference = true)]
        public DMXFixtureProfileConfig Profile;

        private ushort baseDMXChannel;
        public ushort BaseDMXChannel
        {
            get { return baseDMXChannel; }
            set
            {
                if (value <= 512)
                    baseDMXChannel = value;
                else
                    throw new ArgumentOutOfRangeException("baseDMXChannel", "DMX channelNumber address must be between 0 and 512");
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\tId: " + Id);
            sb.AppendLine("\tName: " + Name);
            sb.AppendLine("\tProfile: " + Profile.Name);
            sb.AppendLine("\tDMXChannel: " + BaseDMXChannel);
            return sb.ToString();
        }
    }
}