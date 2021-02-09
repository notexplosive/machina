using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    interface IDebugOutputRenderer
    {
        public void OnMessageLog(string line);
    }
}
