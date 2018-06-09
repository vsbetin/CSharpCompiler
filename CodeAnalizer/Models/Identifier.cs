using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class Identifier : Token
    {
        public Double NumberValue { get; set; }

        public Identifier(string value, int index, int row, int? classIndex) : base(value, index, row, classIndex)
        {
            ClassIndex = classIndex;
        }
    }
}
