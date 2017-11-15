using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class LexicalAnalyzer
    {
        private static List<string> terminalTokens = new List<string>
        {   "prog", "int", "float", "double", "read", "write", "if", "then",
            "for", "do", "or", "and", "not", "{", "}", ";", ",", "=", "(",
            ")", ":", "+", "-", "*", "/", "[", "]", "<", "<=", ">", ">=", "==", "!=", "idn", "con"};


        private static List<char> tokenSeparators = new List<char>
        { '{', '}', ';', ',', '=', '(', ')', ':', '+', '-', '*', '/', '[', ']', '<', '>'};

        private static List<char> whiteSeparators = new List<char>
        { '\n', '\t', ' ', '\r' };

        private List<Token> tokens;
        private List<Constant> constants;
        private List<Identifier> identifiers;

        private bool varDeclareFlag;

        private StringBuilder outputLexems;
        private StringBuilder outputConstants;
        private StringBuilder outputIdentifiers;

        private int rowCount;

        public (string lexemeText, string identifiersText, string constantText) Run(string programText)
        {
            rowCount = 1;
            varDeclareFlag = false;
            tokens = new List<Token>(programText.Split(' ').Length);
            constants = new List<Constant>();
            identifiers = new List<Identifier>();

            bool result = true;

            string lex = string.Empty;
            outputLexems = new StringBuilder();
            outputConstants = new StringBuilder();
            outputIdentifiers = new StringBuilder();


            for (int i = 0; i < programText.Length; i++)
            {
                if (tokenSeparators.Contains(programText[i]))
                {
                    if ((programText[i] == '-' || programText[i] == '+') &&
                        (programText[i - 1] == 'E' || programText[i - 1] == 'e'))
                    {
                        lex += programText[i];
                        continue;
                    }
                    if (lex != "")
                        result = checkLex(lex);
                    if (!result)
                        break;
                    switch (programText[i])
                    {
                        case '=':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("==");
                                i++;
                            }
                            else
                                checkLex("=");
                            break;

                        case '<':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("<=");
                                i++;
                            }
                            else if (programText[i + 1] == '>')
                            {
                                checkLex("<>");
                                i++;
                            }
                            else
                                checkLex("<");
                            break;

                        case '>':
                            if (programText[i + 1] == '=')
                            {
                                checkLex(">=");
                                i++;
                            }
                            else
                                checkLex(">");
                            break;

                        case '!':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("!=");
                                i++;
                            }
                            else
                                checkLex("!");
                            break;

                        default:
                            result = checkLex(programText[i].ToString());
                            if (!result)
                                break;
                            break;
                    }

                    lex = string.Empty; ;
                }
                else if (whiteSeparators.Contains(programText[i]))
                {
                    if (programText[i] == '\r' || programText[i] == '\n')
                        rowCount++;
                    if (lex != "")
                        result = checkLex(lex.ToString());
                    if (!result)
                        break;
                    lex = string.Empty;
                }
                else
                {
                    lex += programText[i];
                }
            }

            if (result)
            {
                string tokenClassIndex = string.Empty;
                outputLexems.Append("LEXEMS TABLE:\r\n");
                foreach (var token in tokens)
                {
                    if (token is Constant)
                        tokenClassIndex = ((Constant)token).ClassIndex.ToString();
                    if (token is Identifier)
                        tokenClassIndex = ((Identifier)token).ClassIndex.ToString();
                    outputLexems.Append(token.Row + "\t" + token.Value + "\t\t" + token.Index + "\t\t" +
                        (string.IsNullOrEmpty(tokenClassIndex) ? null : tokenClassIndex) + "\r\n");
                    tokenClassIndex = string.Empty;
                }

                outputIdentifiers.Append("IDENTIFIERS TABLE:\r\n");
                foreach (var identifier in identifiers)
                {
                    outputIdentifiers.Append(identifier.Value + "\t\t" + identifier.ClassIndex + "\r\n");
                }

                outputConstants.Append("CONSTANTS TABLE:\r\n");
                foreach (var constant in constants)
                {
                    outputConstants.Append(constant.Value + "\t\t" + constant.ClassIndex + "\r\n");
                }
            }
            return (outputLexems.ToString(), outputIdentifiers.ToString(), outputConstants.ToString());
        }

        private bool checkLex(string lex)
        {
            if (terminalTokens.Contains(lex))
            {
                if (lex == "-")
                {
                    Token lastToken = (Token)tokens[tokens.Count - 1];
                    if ((lastToken.Value == "idn") || (lastToken.Value == "con"))
                        lex = "-B";
                    else lex = "-U";
                }
                tokens.Add(new Token(lex, getTokenIndex(lex), rowCount));
                if (!varDeclareFlag && (lex == "strict" || lex == "int" || lex == "double"))
                    varDeclareFlag = true;
                else if ((varDeclareFlag) && (lex == ";"))
                    varDeclareFlag = false;
                return true;
            }

            else if (Regex.IsMatch(lex, "^[0-9]+[.]$|^[0-9]*[.]?[0-9]+$|^[0-9]*[.]?[0-9]*[eE][-+]?[0-9]+$"))
            {
                Constant constant = constants.Find(cons => cons.Value == lex);
                if (constant != null)
                    tokens.Add(new Constant(constant.Value, constant.Index, rowCount, constant.ClassIndex));
                else
                {
                    constants.Add(new Constant(lex, terminalTokens.IndexOf("con"), rowCount, constants.Count + 1));
                    tokens.Add(constants.Last());
                }
                return true;
            }

            else if (Regex.IsMatch(lex, "^[a-zA-Z][a-zA-Z0-9]*$"))
            {
                if ((varDeclareFlag) || (identifiers.Count == 0))
                {
                    if (IsContainsIdentifire(lex))
                    {
                        error(1, lex);
                        return false;
                    }
                    identifiers.Add(new Identifier(lex, terminalTokens.IndexOf("idn"), rowCount, identifiers.Count + 1));
                    tokens.Add(identifiers.Last());
                    return true;
                }

                if (IsContainsIdentifire(lex) && !IsIdentifireProgrmaName(lex))
                {
                    Identifier ident = identifiers.First(idn => idn.Value.Equals(lex));
                    tokens.Add(new Identifier(ident.Value, ident.Index, rowCount, ident.ClassIndex));
                    return true;

                }
                return false;
            }
            else
            {
                error(0, lex);
                return false;
            }

        }

        private bool IsContainsIdentifire(string lex)
        {
            try
            {
                Identifier identifier = identifiers.First(idn => idn.Value.Equals(lex));
                return identifiers.Contains(identifier);
            }
            catch
            {
                return false;
            }

        }

        private bool IsIdentifireProgrmaName(string lex)
            => identifiers.First().Value.Equals(lex);

        private int getTokenIndex(string token)
        {
            if (token == "-U")
                return terminalTokens.IndexOf("-");
            if (token == "-B")
                return terminalTokens.IndexOf("-") + 1;
            for (int i = 0; i < terminalTokens.IndexOf("-") - 1; i++)
            {
                if (terminalTokens[i].Equals(token)) return i + 1;
            }
            for (int i = terminalTokens.IndexOf("-"); i < terminalTokens.Count; i++)
            {
                if (terminalTokens[i].Equals(token)) return i + 2;
            }
            return 0;
        }

        private void error(int code, String lex)
        {
            switch (code)
            {
                case 0:
                    outputLexems.Append("Wrong input: " + lex + Environment.NewLine + "Row: " + rowCount);
                    break;
                case 1:
                    outputLexems.Append("ID was already declared: " + lex + Environment.NewLine + "Row: " + rowCount);
                    break;
            }
        }
    }
}
