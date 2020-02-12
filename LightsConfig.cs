using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;

namespace Daniels.Lighting
{
    public class LightsConfig : ICloneable
    {
        public Dictionary<string, DMXFixtureProfileConfig> Profiles;// = new Dictionary<string,DMXFixtureProfileConfig>();

        public Dictionary<string, List<DMXFixtureConfig>> Lights;// = new Dictionary<string, List<DMXFixtureConfig>>();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}