using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    public class Token
    {
        public string Value { get; set; }
        public int Index { get; set; }
        public int Row { get; set; }
        public int? ClassIndex { get; set; }

        public string GeneralizedValue
        {
            get
            {
                if (Index == 35)
                {
                    return "id";
                }
                if (Index == 36)
                {
                    return "con";
                }
                if (Value == "-U" || Value == "-B")
                    return "-";
                return Value;
            }

        }

        public Token(string value, int index, int row, int? classIndex = null)
        {
            Value = value;
            Index = index;
            Row = row;
            ClassIndex = classIndex;
        }

    }
}
