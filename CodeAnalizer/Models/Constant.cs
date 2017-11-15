using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class Constant : Token
    {
        public int ClassIndex { get; set; }

        public Constant(string value, int index, int row, int classIndex) : base(value, index, row)
        {
            ClassIndex = classIndex;
        }
    }
}
