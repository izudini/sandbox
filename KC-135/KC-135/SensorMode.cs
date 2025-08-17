using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC_135
{
    public enum SensorMode
    {
        Off,  //gray
        Initializing, //yellow
        Operate,  //light green
        Degraded,  //red
        Declaring  //flashing dark red
    }
}
