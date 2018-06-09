using System;

namespace CodeAnalizer.Models
{
    class Sentence
    {
        public String Head { get; set; }
        public String[] Tail { get; set; }

        public Sentence(String head, String tail)
        {
            Head = head;
            Tail = tail.Split(' ');
        }
    }
}