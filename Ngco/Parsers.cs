using System;
using System.Collections.Generic;
using System.Text;

namespace Ngco
{
    public static class Parsers
    {
        public static bool ParseBool(string value)
        {
            switch (value.ToLower())
            {
                case "true":
                case "1": return true;

                default: return false;
            }
        }
    }
}
