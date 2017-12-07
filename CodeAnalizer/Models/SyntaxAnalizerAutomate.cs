using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class SyntaxAnalizerAutomate
    {
        public Automate[] automates =
       {
            new Automate("1", "prog",   "2",    null,   true,   false,  true),
            new Automate("2", "idn",    "3",    null,   true,   false,  true),
            new Automate("3", "int",    "4",    null,   true,   false,  true),
            new Automate("4", "idn",    "5",    null,   true,   false,  true),
            new Automate("5", ",",      "4",    null,   false,  false,  true),
            new Automate("5", ";",      "6",    null,   true,   false,  true),
            new Automate("6", "int",    "4",    null,   false,  false,  true),
            new Automate("6", "{",      "1-1",  "7",    true,   false,  true),
            new Automate("7", ";",      "8",    null,   true,   false,  true),
            new Automate("8", "}",      null,   null,   false,  true,   false),
            new Automate("8", null,     "1-1",  "7",    false,  false,  false),

            

            new Automate("1-1", "idn",      "1-2",      null,   false,  false,  true),
            new Automate("1-1", "read",     "1-4",      null,   false,  false,  true),
            new Automate("1-1", "write",    "1-4",      null,   false,  false,  true),
            new Automate("1-1", "if",       "1-7",      null,   false,  false,  true),
            new Automate("1-1", "for",      "1-12",     null,   true,   false,  true),

            new Automate("1-2", "=",        "2-1",      "1-3",  true,   false,  true),
            new Automate("1-3", null,       null,       null,   false,  true,   false),

            new Automate("1-4", "(",        "1-5",      null,   true,   false,  true),
            new Automate("1-5", "idn",      "1-6",      null,   true,   false,  true),
            new Automate("1-6", ",",        "1-5",      null,   false,  false,  true),
            new Automate("1-6", ")",        null,       null,   true,   true,   true),

            new Automate("1-7", "(",        "3-1",      "1-8",  true,   false,  true),
            new Automate("1-8", ")",        "1-9",      null,   true,   false,  true),
            new Automate("1-9", "{",        "1-1",      "1-10", true,   false,  true),
            new Automate("1-10",";",        "1-11",     null,   true,   false,  true),
            new Automate("1-11","}",        "7",        null,   false,   true,   true),
            new Automate("1-11",null,       "1-1",      "1-10", false,  false,  false),

            new Automate("1-12", "(",       "1-13",     null,   true,   false,  true),
            new Automate("1-13", "idn",     "1-14",     null,   true,   false,  true),
            new Automate("1-14", "=",       "2-1",      "1-15", true,   false,  true),
            new Automate("1-15", ";",       "2-1",      "1-16", true,   false,  true),
            new Automate("1-16", ">",       "2-1",      "1-17", false,  false,  true),
            new Automate("1-16", "<",       "2-1",      "1-17", false,  false,  true),
            new Automate("1-16", ">=",      "2-1",      "1-17", false,  false,  true),
            new Automate("1-16", "<=",      "2-1",      "1-17", false,  false,  true),
            new Automate("1-16", "==",      "2-1",      "1-17", false,  false,  true),
            new Automate("1-16", "<>",      "2-1",      "1-17", true,   false,  true),
            new Automate("1-17", ";",       "2-1",      "1-18", true,   false,  true),
            new Automate("1-18", ")",       "1-19",     null,   true,   false,  true),
            new Automate("1-19", "{",       "1-1",      "1-20", true,   false,  true),
            new Automate("1-20", ";",       "1-21",     null,   true,   false,  true),
            new Automate("1-21", "}",       "7",        null,   false,  true,   true),
            new Automate("1-21", null,      "1-1",      "1-20", false,  false,  false),



            new Automate("2-1", "-",    "2-2",  null,   false,  false,  true),
            new Automate("2-1", "(",    "2-1",  "2-3",  false,  false,  true),
            new Automate("2-1", "con",  "2-4",  null,   false,  false,  true),
            new Automate("2-1", "idn",  "2-4",  null,   true,   false,  true),
            new Automate("2-2", "(",    "2-1",  "2-3",  false,  false,  true),
            new Automate("2-2", "con",  "2-4",  null,   false,  false,  true),
            new Automate("2-2", "id",   "2-4",  null,   true,   false,  true),
            new Automate("2-3", ")",    "2-4",  null,   true,   false,  true),
            new Automate("2-4", "*",    "2-1",  null,   false,  false,  true),
            new Automate("2-4", "/",    "2-1",  null,   false,  false,  true),
            new Automate("2-4", "+",    "2-1",  null,   false,  false,  true),
            new Automate("2-4", "-",    "2-1",  null,   false,  true,  true),
            new Automate("2-4", null,   null,   null,   false,  true,   false),
            


            new Automate("3-1", "not",  "3-1",  null,   false,  false,  true),
            new Automate("3-1", "[",    "3-1",  "3-2",  false,  false,  true),
            new Automate("3-1", null,   "2-1",  "3-3",  false,  false,  false),
            new Automate("3-2", "]",    "3-4",  null,   true,   false,  true),
            new Automate("3-3", ">",    "2-1",  "3-4",  false,  false,  true),
            new Automate("3-3", "<",    "2-1",  "3-4",  false,  false,  true),
            new Automate("3-3", ">=",   "2-1",  "3-4",  false,  false,  true),
            new Automate("3-3", "<=",   "2-1",  "3-4",  false,  false,  true),
            new Automate("3-3", "==",   "2-1",  "3-4",  false,  false,  true),
            new Automate("3-3", "<>",   "2-1",  "3-4",  true,   false,  true),
            new Automate("3-4", "and",  "3-1",  null,   false,  false,  true),
            new Automate("3-4", "or",   "3-1",  null,   false,  false,  true),
            new Automate("3-4", null,   null,   null,   false,  true,   false),
        };

        private List<Token> lexemes; 
        private string Result { set; get; }
        private int currentRow;
        private int i;
        private Stack<string> stack; 
        private string debug; 

        public SyntaxAnalizerAutomate()
        {
            stack = new Stack<string>();
            Result = "";
            currentRow = 1;
            i = 0;
        }

        public string Process(List<Token> lexemes)
        {            
            string firstState = "1";
            stack = new Stack<string>();
            this.lexemes = lexemes;
            Result = "";
            debug = "";
            currentRow = 1;
            i = 0;
            do
            {
                currentRow = lexemes[i].Row;                
                Result = ChangeState(firstState, lexemes[i].GeneralizedValue);
                if (Result[0] == 'E')
                {
                    if (i + 1 == lexemes.Count)
                        return "PROGRAM HAS CORRECT SYNTAX!\r\n";
                    else
                    {
                        return "CODE AFTER THE PROGRAM END\r\n";
                    }
                }
                if (Result[0] == 'U' || Result[0] == 'S')
                {
                    return Result;// + debug;
                }
                firstState = Result;
                debug += Result + " " + lexemes[i].GeneralizedValue + "\r\n";
            } while (true);
        }

        
        private string ChangeState(string state, string lex)
        {
            debug += lex + ": ";
            foreach (Automate row in automates)
            {
                if (row.FirstState.Equals(state))
                {
                    debug += row.GetInfo();
                    if (row.Lexem == null || row.Lexem.Equals(lex))
                    {
                        if (row.StackWrite != null)
                            stack.Push(row.StackWrite);
                        if (row.Inc)
                        {
                            i++;
                            if (i >= lexemes.Count)
                                return "UNEXPECTED END OF PROGRAM AT LINE " + currentRow + "\r\n";
                        }
                        if (row.Exit)
                        {
                            if (stack.Count > 0)
                            {
                                return stack.Pop();
                            }
                            else
                                return "EMPTY STACK";
                        }
                        if (row.Lexem == null && row.Error)
                            return "SYNTAX ERROR AT LINE " + currentRow + "\r\n";
                        return row.SecondState;
                    }
                    else
                    {
                        if (row.Error)
                            return "SYNTAX ERROR AT LINE " + currentRow + "\r\n";
                    }
                }
            }
            return "?";
        }
    }
}
