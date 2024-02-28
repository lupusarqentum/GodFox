using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaRules
{
    internal sealed class VoiceStepEventArgs : EventArgs
    {
        public List<int> allowList;

        public VoiceStepEventArgs(List<int> allowList)
        {
            this.allowList = allowList;
        }
    }
}
