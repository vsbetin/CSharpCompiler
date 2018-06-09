using System;

namespace CodeAnalizer.Models
{
    public class AscentSAInfo
    {
        public String Input { get; set; }
        public String Sign { get; set; }
        public String Stack { get; set; }
        public String Base { get; set; }
        public string PolishNote { get; set; }

        public AscentSAInfo()
        {
            Base = "";
        }
    }
}