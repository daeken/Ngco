using System;
using System.Collections.Generic;
using System.Text;
using Ngco.Widgets;

namespace Ngco
{
    public struct MutualExclusionToken
    {
        private BaseWidget _owner;

        public BaseWidget Owner { get => _owner; set => _owner = value; }
    }
}
