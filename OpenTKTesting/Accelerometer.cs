using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTKTesting
{
    [Serializable]
    public struct Accelerometer
    {
        public uint XRaw;
        public uint XAcceleration;
        public uint YRaw;
        public uint YAcceleration;
    }
}
