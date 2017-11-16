using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class SyntaxAnalizer
    {
        private List<Token> lexemes;
        private string result;
        private int currentRow;
        private int i;
        private bool erroreFlag = false;

        public SyntaxAnalizer()
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
                if (Check("prog") &&
                    Check("idn") &&
                    DeclarationList() &&
                    Check("{") &&
                    OperatorsList() &&
                    Check("}")
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
                    Errore("Errore in PROG definition");
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
                    if (Check(";"))
                    {
                        int savedI = i;
                        while (Declaration())
                        {
                            if (!Check(";"))
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
                ErroreOut("Errore in declaration");
            }
            Errore("Errore in declaration list");
            return false;
        }

        private bool Declaration()
        {
            try
            {


                if (Check("int") || Check("float") || Check("double"))
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
                ErroreOut("Errore in declaration");
            }
            return false;
        }

        private bool IdentifiersList()
        {
            try
            {
                if (Check("idn"))
                {
                    int savedI = i;
                    while (Check(","))
                    {
                        if (!Check("idn"))
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
                ErroreOut("Errore in list of identifiers");
            }
            currentRow--;
            Errore("Errore in declaration");
            return false;
        }

        private bool OperatorsList()
        {
            try
            {
                if (Operator() && Check(";"))
                {
                    int savedI = i;
                    while (Operator())
                    {
                        if (!Check(";") && lexemes[i - 1].Value != "}")
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
                ErroreOut("Wrong list of operators");
            }
            return false;
        }


        private bool Operator()
        {
            try
            {
                int saveID = i;
                if (Check("idn"))
                {
                    if (Check("=") && Expression())
                        return true;
                    else
                    {
                        Errore("Wrong code after identifier");
                        return false;
                    }
                }
                else
                {
                    i = saveID;
                    if (Check("read"))
                    {
                        if (Check("(") && IdentifiersList() && Check(")"))
                            return true;
                        else
                        {
                            Errore("Wrong code after read function");
                            return false;
                        }
                    }
                    else
                    {
                        i = saveID;
                        if (Check("write"))
                        {
                            if (Check("(") && IdentifiersList() && Check(")"))
                                return true;
                            else
                            {
                                Errore("Wrong code after write function");
                                return false;
                            }
                        }
                        else
                        {
                            i = saveID;
                            if (Check("for"))
                            {
                                if (Check("(") &&
                                Check("idn") && Check("=") && Expression() && Check(";") &&
                                Relation() && Check(";") && Expression() && Check(")") &&
                                Check("{"))
                                {
                                    if (OperatorsList())
                                    {
                                        if (Check("}"))
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
                            {
                                i = saveID;
                                if (Check("if"))
                                {
                                    if (Check("(") && Relation() && Check(")") && Check("{"))
                                    {
                                        if (OperatorsList())
                                        {
                                            if (Check("}"))
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
                            }
                        }

                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore in list of operators");
            }
            return false;
        }


        private bool Expression()
        {
            try
            {
                Check("-");
                if (Term())
                {
                    int savedI = i;
                    while (Check("+") || (Check("-")))
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
            Errore("Errore in expresion");
            return false;
        }


        private bool Term()
        {
            try
            {
                if (Mult())
                {
                    int savedI = i;
                    while (Check("*") || (Check("/")))
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
            Errore("Errore in expression");
            return false;
        }

        private bool Mult()
        {
            try
            {
                if (Check("con") || Check("idn"))
                {
                    return true;
                }
                else
                {
                    if (Check("(") && Expression() && Check(")"))
                        return true;
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore in expression");
            }
            Errore("Errore in expression");
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
                ErroreOut("Errore in logical expression");
            }
            Errore("Errore in logical expression");
            return false;
        }

        private bool RelationSigns()
        {
            try
            {
                if (Check("<") || Check(">") || Check("==")
                        || Check("<>") || Check("<=") || Check(">="))
                    return true;

            }
            catch (ArgumentOutOfRangeException)
            {
                ErroreOut("Errore in relation signs");
            }
            Errore("Wrong sing in logical expression");
            return false;
        }

        //private bool LogicalExpression()
        //{
        //    try
        //    {
        //        if (LogicalTerm())
        //        {
        //            int savedI = i;
        //            while (Check("or"))
        //            {
        //                if (!LogicalTerm())
        //                    return false;
        //                else
        //                {
        //                    savedI = i;
        //                }
        //            }
        //            i = savedI;
        //            return true;
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException)
        //    {
        //        result = "UNEXPECTED PROGRAM END\r\n";
        //        return false;
        //    }
        //    result += "ERROR IN LOGICAL EXPRESSION\r\n";
        //    return false;
        //}
        //private bool LogicalTerm()
        //{
        //    try
        //    {
        //        if (LogicalMult())
        //        {
        //            int savedI = i;
        //            while (Check("and"))
        //            {
        //                if (!LogicalMult())
        //                    return false;
        //                else
        //                {
        //                    savedI = i;
        //                }
        //            }
        //            i = savedI;
        //            return true;
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException)
        //    {
        //        result = "UNEXPECTED PROGRAM END\r\n";
        //        return false;
        //    }
        //    return false;
        //}
        //private bool LogicalMult()
        //{
        //    try
        //    {
        //        int savedI = i;
        //        while (Check("not"))
        //        {
        //            savedI = i;
        //        }
        //        i = savedI;
        //        if (Relation())
        //        {
        //            return true;
        //        }
        //        i = savedI;
        //        if (Check("[") && LogicalExpression() && Check("]"))
        //            return true;


        //    }
        //    catch (ArgumentOutOfRangeException )
        //    {
        //        result = "UNEXPECTED PROGRAM END\r\n";
        //        return false;
        //    }
        //    return false;
        //}

        private bool Check(string str)
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
