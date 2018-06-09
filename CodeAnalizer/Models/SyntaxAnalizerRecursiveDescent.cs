using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class SyntaxAnalizerRecursiveDescent
    {
        private List<Token> lexemes;
        private string result;
        private int currentRow;
        private int i;
        private bool erroreFlag = false;

        public SyntaxAnalizerRecursiveDescent()
        {
            result = "";
            currentRow = 1;
            i = 0;
        }

        public string Process(List<Token> lexemes)
        {
            this.lexemes = lexemes;
            result = "";
            currentRow = 1;
            i = 0;
            Program();
            return result;
        }

        private bool Program()
        {
            try
            {
                if (CheckLexeme("Prog") &&
                    CheckLexeme("id") &&
                    DeclarationList() &&
                    CheckLexeme("{") &&
                    OperatorsList() &&
                    CheckLexeme("}")
                    )
                {
                    while ((i < lexemes.Count) && lexemes[i].GeneralizedValue.Equals("NL"))
                    {
                        i++;
                    }
                    if (i < lexemes.Count)
                    {
                        Errore("Code after programm end");
                        return false;
                    }
                    else
                    {
                        if (!erroreFlag)
                            result = "Success";
                        return true;
                    }
                }
                else
                {
                    currentRow--;
                    Errore("Errore");
                }
            }
            catch (Exception)
            {
                ErroreOut("Errore");
            }
            return false;
        }


        private bool DeclarationList()
        {
            try
            {
                if (Declaration())
                {
                    if (CheckLexeme(";"))
                    {
                        int savedI = i;
                        while (Declaration())
                        {
                            if (!CheckLexeme(";"))
                                return false;
                            else
                            {
                                savedI = i;
                            }
                        }
                        i = savedI;
                        return true;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            Errore("Errore in declaration list");
            return false;
        }

        private bool Declaration()
        {
            try
            {
                if (CheckLexeme("var") && (CheckLexeme("int") || CheckLexeme("float") || CheckLexeme("double")))
                {
                    if (IdentifiersList())
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                currentRow--;
                ErroreOut("Errore");
            }
            return false;
        }

        private bool IdentifiersList()
        {
            try
            {
                if (CheckLexeme("id"))
                {
                    int savedI = i;
                    while (CheckLexeme(","))
                    {
                        if (!CheckLexeme("id"))
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                currentRow--;
                ErroreOut("Errore");
            }
            currentRow--;
            Errore("Errore in declaration");
            return false;
        }

        private bool OperatorsList()
        {
            try
            {
                if (Operator() && CheckLexeme(";"))
                {
                    int savedI = i;
                    while (Operator())
                    {
                        if (!CheckLexeme(";"))
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            return false;
        }


        private bool Operator()
        {
            try
            {
                int saveID = i;
                if (CheckPrisv())
                {
                    return true;
                }
                else
                {
                    i = saveID;
                    if (CheckRead())
                    {
                        return true;
                    }
                    else
                    {
                        i = saveID;
                        if (ChekWrite())
                        {
                            return true;
                        }
                        else
                        {
                            i = saveID;
                            if (CheckFor())
                            {
                                return true;
                            }
                            else
                            {
                                i = saveID;
                                if (CheckIf())
                                {
                                    return true;
                                }
                            }
                        }

                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            return false;
        }

        private bool CheckPrisv()
        {
            if (CheckLexeme("id"))
            {
                if (CheckLexeme("=") && Expression())
                    return true;
                else
                {
                    Errore("Wrong code after identifier");
                    return false;
                }
            }
            else
                return false;
        }

        private bool ChekWrite()
        {
            if (CheckLexeme("write"))
            {
                if (CheckLexeme("(") && IdentifiersList() && CheckLexeme(")"))
                    return true;
                else
                {
                    Errore("Wrong code after write function");
                    return false;
                }
            }
            else
                return false;
        }

        private bool CheckRead()
        {
            if (CheckLexeme("read"))
            {
                if (CheckLexeme("(") && IdentifiersList() && CheckLexeme(")"))
                    return true;
                else
                {
                    Errore("Wrong code after read function");
                    return false;
                }
            }
            else
                return false;
        }

        private bool CheckFor()
        {
            if (CheckLexeme("for"))
            {
                if (CheckLexeme("id") && CheckLexeme("=") && Expression()
                    && CheckLexeme("to") && Expression()
                    && CheckLexeme("step") && Expression()
                    && CheckLexeme("{"))
                {
                    if (OperatorsList())
                    {
                        if (CheckLexeme("}"))
                            return true;
                        else
                        {
                            Errore("Errore in FOR definition");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    Errore("Errore in FOR definition");
                    return false;
                }

            }
            else
                return false;
        }

        private bool CheckIf()
        {
            if (CheckLexeme("if"))
            {
                if (CheckLexeme("(") && LogicalExpression() && CheckLexeme(")") && CheckLexeme("{"))
                {
                    if (OperatorsList())
                    {
                        if (CheckLexeme("}"))
                            return true;
                        else
                        {
                            Errore("Errore in IF definition");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    Errore("Errore in IF definition");
                    return false;
                }
            }
            else
                return false;
        }

        private bool Expression()
        {
            try
            {
                CheckLexeme("-");
                if (Term())
                {
                    int savedI = i;
                    while (CheckLexeme("+") || (CheckLexeme("-")))
                    {
                        if (!Term())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore in expression");
            }
            return false;
        }


        private bool Term()
        {
            try
            {
                if (Mult())
                {
                    int savedI = i;
                    while (CheckLexeme("*") || (CheckLexeme("/")))
                    {
                        if (!Mult())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore in expression");
            }
            return false;
        }

        private bool Mult()
        {
            try
            {
                if (CheckLexeme("con") || CheckLexeme("id"))
                {
                    return true;
                }
                else
                {
                    if (CheckLexeme("(") && Expression() && CheckLexeme(")"))
                        return true;
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            return false;
        }

        private bool RelationSigns()
        {
            try
            {
                if (CheckLexeme("<") || CheckLexeme(">") || CheckLexeme("==")
                        || CheckLexeme("<>") || CheckLexeme("<=") || CheckLexeme(">="))
                    return true;

            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            Errore("Wrong sing in logical expression");
            return false;
        }

        private bool LogicalExpression()
        {
            try
            {
                if (LogicalTerm())
                {
                    int savedI = i;
                    while (CheckLexeme("or"))
                    {
                        if (!LogicalTerm())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
                return false;
            }
            Errore("Errore in logical expression");
            return false;
        }

        private bool LogicalTerm()
        {
            try
            {
                if (LogicalMult())
                {
                    int savedI = i;
                    while (CheckLexeme("and"))
                    {
                        if (!LogicalMult())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
                return false;
            }
            return false;
        }
        private bool LogicalMult()
        {
            try
            {
                int savedI = i;
                while (CheckLexeme("not"))
                {
                    savedI = i;
                }
                i = savedI;
                if (Relation())
                {
                    return true;
                }
                i = savedI;
                if (CheckLexeme("[") && LogicalExpression() && CheckLexeme("]"))
                    return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
                return false;
            }
            return false;
        }

        private bool Relation()
        {
            try
            {
                if (Expression() && RelationSigns() && Expression())
                    return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore");
            }
            return false;
        }


        private bool CheckLexeme(string str)
        {
            if (Next().Equals(str))
            {
                i++;
                return true;
            }
            return false;

        }

        private string Next()
        {
            currentRow = lexemes[i].Row;
            return lexemes[i].GeneralizedValue;
        }

        private void Errore(string str)
        {
            if (!erroreFlag)
            {
                result = str + Environment.NewLine;
                result += "Row " + currentRow + Environment.NewLine;
                erroreFlag = true;
            }
        }

        private void ErroreOut(string str)
        {
            if (!erroreFlag)
            {
                result = str + Environment.NewLine;
                result += "Row " + (currentRow + 1) + Environment.NewLine;
                erroreFlag = true;
            }
        }
    }
}
