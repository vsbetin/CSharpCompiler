using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class Automate
    {
        public string FirstState //Начальное состояние
        {
            set; get;
        }
        public string Lexem //Лексема, поступающая на вход
        {
            set; get;
        }
        public string SecondState //Следующее состояние
        {
            set; get;
        }
        public string StackWrite //Состояние, которое записывается в стек
        {
            set; get;
        }
        public bool Error //Возбудить ошибку в случае несравнения
        {
            set; get;
        }
        public bool Exit //Выйти в случае сравнения/несравнения
        {
            set; get;
        }
        public bool Inc //Перейти к следующей лексеме из выходной таблицы
        {
            set; get;
        }
        public Automate(string firstState, string lexem, string secondState, string stackWrite, bool error, bool exit, bool inc)
        {
            FirstState = firstState;
            Lexem = lexem;
            SecondState = secondState;
            StackWrite = stackWrite;
            Error = error;
            Exit = exit;
            Inc = inc;
        }

        public string GetInfo()
        {
            return FirstState + " " + Lexem + " " + SecondState + " " + StackWrite + " " + Error + " " + Exit + " " + Inc + "\r\n";
        }
    }
}
