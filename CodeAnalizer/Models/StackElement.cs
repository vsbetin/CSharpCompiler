using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    public class StackElement
    {
        public String Value { get; set; }
        public List<String> AssociatedLabels { get; set; }
        public int Priority { get; set; }
        public int StackPriority { get; set; }

        public StackElement(String value, int priority)
        {
            Value = value;
            Priority = priority;
            StackPriority = priority;
        }
        public StackElement(String value, int priority, int stackPriority)
        {
            Value = value;
            Priority = priority;
            StackPriority = stackPriority;
        }
    }
}
