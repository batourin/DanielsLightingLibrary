using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace Daniels.Lighting
{
    [JsonObject(IsReference = true)]
    public class DMXFixtureProfileConfig: ICloneable
    {
        public uint Id;
        public string Name;

        public Dictionary<DMXChannel, ushort> DMXChannels;

        public DMXFixtureProfileConfig() { }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\tId: " + Id);
            sb.AppendLine("\tName: " + Name);
            sb.AppendLine("\tDMXChannels:");
            foreach (DMXChannel channel in DMXChannels.Keys)
            {
                sb.AppendLine("\t\t" + channel.ToString() + ": " + DMXChannels[channel]);
            }
            return sb.ToString();
        }

    }
}