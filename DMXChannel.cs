using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Daniels.Lighting
{
    public enum DMXChannel
    {
        Unknown = 0,
        Intensity,
        WhitePoint,
        Pan,
        PanFine,
        Tilt,
        TiltFine,
        Zoom,
        Iris,
        Focus,
        Blade1,
        Blade1Rotate,
        Blade2,
        Blade2Rotate,
        Blade3,
        Blade3Rotate,
        Blade4,
        Blade4Rotate,
    }
}