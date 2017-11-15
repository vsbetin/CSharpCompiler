using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class Token
    {
        public string Value { get; set; }
        public int Index { get; set; }
        public int Row { get; set; }

        public Token(string value, int index, int row)
        {
            Value = value;
            Index = index;
            Row = row;
        }

    }
}
