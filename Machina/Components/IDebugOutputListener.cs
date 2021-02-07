using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    interface IDebugOutputListener
    {
        public void OnMessageLog(string line);
    }
}
