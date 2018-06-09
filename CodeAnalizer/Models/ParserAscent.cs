using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class ParserAscent
    {
        private List<Token> lexemes;
        public List<AscentSAInfo> Information { set; get; }
        private int currentRow;
        private List<Token> inputLine;
        private List<String> workLine;
        PrecedenceTableBuilder builder;
        String[,] table;

        public Boolean Errors { set; get; }

        public String process(List<Token> lexemes)
        {
            builder = new PrecedenceTableBuilder();
            table = builder.process();
            Errors = false;
            this.lexemes = new List<Token>(lexemes);
            Information = new List<AscentSAInfo>();
            currentRow = 1;
            inputLine = new List<Token>(lexemes);
            inputLine.Add(new Token("#", 0, lexemes[lexemes.Count - 1].Row, 0));
            workLine = new List<String>();
            workLine.Add("#");
            while (inputLine.Count != 0)
            {
                AscentSAInfo info = new AscentSAInfo();
                if ((workLine.Count == 2) && (workLine[1].Equals("@программа")))
                {
                    return "У ПРОГРАММЫ ПРАВИЛЬНЫЙ СИНТАКСИС!\r\n";
                }
                info.Input = "";
                foreach (Token lex in inputLine)
                {
                    info.Input += lex.GeneralizedValue + " ";
                }
                info.Stack += "";
                foreach (String el in workLine)
                {
                    info.Stack += el + " ";
                }
                String sign = getSign(workLine[workLine.Count - 1], inputLine[0].GeneralizedValue);
                info.Sign = workLine[workLine.Count - 1] + " " + sign + " "
                    + inputLine[0].GeneralizedValue;
                if (sign.Contains("<")
                    || sign.Contains("=")
                    || sign.Equals(" "))
                {
                    currentRow = inputLine[0].Row;
                    workLine.Add(inputLine[0].GeneralizedValue);
                    inputLine.Remove(inputLine[0]);
                    Information.Add(info);
                    continue;
                }
                if (sign.Contains(">"))
                {
                    List<String> baseList = new List<String>();
                    int i = workLine.Count;
                    do
                    {
                        i--;
                        baseList.Add(workLine[i]);
                    }
                    while (getSign(workLine[i - 1], workLine[i]).Contains("="));
                    baseList.Reverse();
                    info.Base = "";
                    foreach (String el in baseList)
                    {
                        info.Base += el + " ";
                    }
                    workLine.RemoveRange(i, workLine.Count - i);
                    workLine.Add(change(baseList));
                    if (workLine[workLine.Count() - 1].Equals(" "))
                        return "ОШИБКА ПРИ ПРОХОЖДЕНИИ СТРОКИ №" + currentRow + "\r\nУ ПРОГРАММЫ НЕПРАВИЛЬНЫЙ СИНТАКСИС\r\n";
                    Information.Add(info);
                    continue;
                }
                {
                    Information.Add(info);
                    return "ОШИБКА ПРИ ПРОХОЖДЕНИИ СТРОКИ №" + currentRow + "\r\nУ ПРОГРАММЫ НЕПРАВИЛЬНЫЙ СИНТАКСИС\r\n";
                }
            }
            if ((workLine.Count == 2) && (workLine[1].Equals("@программа")))
            {
                return "У ПРОГРАММЫ ПРАВИЛЬНЫЙ СИНТАКСИС!\r\n";
            }
            else
            {
                return "ОШИБКА ПРИ ПРОХОЖДЕНИИ СТРОКИ №" + currentRow + "\r\nУ ПРОГРАММЫ НЕПРАВИЛЬНЫЙ СИНТАКСИС\r\n";
            }
        }



        private String getSign(String sign1, String sign2)
        {
            if (sign1.Equals("#"))
                return "<";
            if (sign2.Equals("#"))
                return ">";
            return table[builder.getSymbolIndex(sign1), builder.getSymbolIndex(sign2)];
        }
        private String change(List<String> symbols)
        {
            return builder.findAndChange(symbols);
        }
    }
}
