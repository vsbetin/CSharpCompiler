using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalizer.Models
{
    class IntermediateCodeGenerator
    {
        private List<Token> lexemes;
        public List<String> Result { set; get; }
        private int i;
        private List<StackElement> stack;
        public List<ICGInfo> Information { set; get; }
        private int lastLabelNumber = 1;
        private String[] hiddenSymbols = { "(", "[", "if", "for", "read", "write", "?" };


        public IntermediateCodeGenerator(List<Token> lexemes)
        {
            this.lexemes = lexemes;
            Result = new List<String>();
            i = 0;
            stack = new List<StackElement>();
            Information = new List<ICGInfo>();
        }

        public List<String> process()
        {
            int forCount = 0, ifCount = 0, ternaryCount = 0;
            bool forFlag = false, forVarFlag = false, readFlag = false, writeFlag = false,
                ifFlag = false, ternaryFlag = false, declarationFlag = true;
            List<int> workCells = new List<int>();
            List<String> loopCounters = new List<string>();
            int tokenIndex = -1;
            foreach (Token token in lexemes)
            {
                tokenIndex++;
                if (declarationFlag)
                {
                    if (token.GeneralizedValue.Equals("{"))
                        declarationFlag = false;
                    continue;
                }
                if (token.GeneralizedValue.Equals("id"))
                {
                    Result.Add(token.Value);
                    if (readFlag)
                        Result.Add("rd");
                    else
                        if (writeFlag)
                        Result.Add("wr");
                    else
                        if (forVarFlag)
                    {
                        loopCounters.Add(token.Value);
                        forVarFlag = false;
                    }
                }
                else
                if (token.GeneralizedValue.Equals("con"))
                {
                    Result.Add(token.Value);
                }
                else
                if (token.GeneralizedValue.Equals("+") || token.GeneralizedValue.Equals("-"))
                {
                    if (lexemes[tokenIndex - 1].GeneralizedValue.Equals("con")
                        || lexemes[tokenIndex - 1].GeneralizedValue.Equals("id"))
                    {
                        stack.Add(new StackElement(token.GeneralizedValue, 7));
                    }
                    else
                    {
                        stack.Add(new StackElement("@", 7));
                    }
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("*") || token.GeneralizedValue.Equals("/"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 8));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("("))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                }
                else
                if (token.GeneralizedValue.Equals(")"))
                {
                    extruseUntil("(");
                    if (readFlag)
                        readFlag = false;
                    else
                        if (writeFlag)
                        writeFlag = false;
                }
                else
                if (token.GeneralizedValue.Equals("<") || token.GeneralizedValue.Equals("<=")
                    || token.GeneralizedValue.Equals(">") || token.GeneralizedValue.Equals(">=")
                    || token.GeneralizedValue.Equals("==") || token.GeneralizedValue.Equals("!="))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 6));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("not"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 5));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("and"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 4));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("or"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 3));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("["))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                }
                else
                if (token.GeneralizedValue.Equals("]"))
                {
                    extruseUntil("[");
                }
                else
                if (token.GeneralizedValue.Equals("="))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 10, 2));
                    extruse();
                }
                else
                if (token.GeneralizedValue.Equals("if"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                    ifFlag = true;
                    ifCount++;
                }
                else
                if (ifFlag && token.GeneralizedValue.Equals("{"))
                {
                    extruseUntilExclusively("if");
                    stack[stack.Count - 1].AssociatedLabels = new List<String>();
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    Result.Add(stack[stack.Count - 1].AssociatedLabels[0]);
                    Result.Add("УПЛ");
                }
                else
                if (ifFlag && token.GeneralizedValue.Equals("}"))
                {
                    extruseUntilExclusively("if");
                    Result.Add(stack[stack.Count - 1].AssociatedLabels[0] + ":");
                    extruseUntil("if");
                    ifCount--;
                    if (ifCount <= 0)
                        ifFlag = false;
                }
                else
                if (token.GeneralizedValue.Equals("for"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                    stack[stack.Count - 1].AssociatedLabels = new List<String>();
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    forCount++;
                    forFlag = true;
                    forVarFlag = true;
                    workCells.Add(workCells.Count + 1);
                    workCells.Add(workCells.Count + 1);
                    workCells.Add(workCells.Count + 1);
                }
                else
                if (token.GeneralizedValue.Equals("to"))
                {
                    extruseUntilExclusively("for");
                    Result.Add("R" + workCells[workCells.Count - 3].ToString());
                    Result.Add("1");
                    Result.Add("=");
                    String firstLabel
                        = stack[stack.Count - 1].AssociatedLabels[stack[stack.Count - 1].AssociatedLabels.Count - 3];
                    Result.Add(firstLabel + ":");
                    Result.Add("R" + workCells[workCells.Count - 2].ToString());
                }
                else
                if (token.GeneralizedValue.Equals("step"))
                {
                    extruseUntilExclusively("for");
                    Result.Add("=");
                    Result.Add("R" + workCells[workCells.Count - 1].ToString());
                }
                else
                if (forFlag && token.GeneralizedValue.Equals("{"))
                {
                    extruseUntilExclusively("for");
                    Result.Add("=");
                    Result.Add("R" + workCells[workCells.Count - 3].ToString());
                    Result.Add("0");
                    Result.Add("==");
                    String middleLabel
                        = stack[stack.Count - 1].AssociatedLabels[stack[stack.Count - 1].AssociatedLabels.Count - 2];
                    Result.Add(middleLabel);
                    Result.Add("УПЛ");
                    Result.Add(loopCounters[loopCounters.Count - 1]);
                    Result.Add(loopCounters[loopCounters.Count - 1]);
                    Result.Add("R" + workCells[workCells.Count - 1].ToString());
                    Result.Add("+");
                    Result.Add("=");
                    Result.Add(middleLabel + ":");
                    Result.Add("R" + workCells[workCells.Count - 3].ToString());
                    Result.Add("0");
                    Result.Add("=");
                    Result.Add(loopCounters[loopCounters.Count - 1]);
                    Result.Add("R" + workCells[workCells.Count - 2].ToString());
                    Result.Add("-");
                    Result.Add("R" + workCells[workCells.Count - 1].ToString());
                    Result.Add("*");
                    Result.Add("0");
                    Result.Add("<=");
                    String lastLabel
                        = stack[stack.Count - 1].AssociatedLabels[stack[stack.Count - 1].AssociatedLabels.Count - 1];
                    Result.Add(lastLabel);
                    Result.Add("УПЛ");
                }
                else
                if (forFlag && token.GeneralizedValue.Equals("}"))
                {
                    extruseUntilExclusively("for");
                    String firstLabel
                        = stack[stack.Count - 1].AssociatedLabels[stack[stack.Count - 1].AssociatedLabels.Count - 3];
                    Result.Add(firstLabel);
                    Result.Add("БП");
                    String lastLabel
                        = stack[stack.Count - 1].AssociatedLabels[stack[stack.Count - 1].AssociatedLabels.Count - 1];
                    Result.Add(lastLabel + ":");
                    extruseUntil("for");
                    forCount--;
                    if (forCount <= 0)
                        forFlag = false;
                }
                else
                if (token.GeneralizedValue.Equals("read"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                    readFlag = true;
                }
                else
                if (token.GeneralizedValue.Equals("write"))
                {
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                    writeFlag = true;
                }
                else
                if (token.GeneralizedValue.Equals("?"))
                {
                    extruseUntilExclusively("=");
                    stack.Add(new StackElement(token.GeneralizedValue, 0));
                    stack[stack.Count - 1].AssociatedLabels = new List<String>();
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    stack[stack.Count - 1].AssociatedLabels.Add("M" + lastLabelNumber.ToString());
                    lastLabelNumber++;
                    Result.Add(stack[stack.Count - 1].AssociatedLabels[0]);
                    Result.Add("УПЛ");
                    ternaryCount++;
                    ternaryFlag = true;
                }
                else if (ternaryFlag && token.GeneralizedValue.Equals(":"))
                {
                    extruseUntilExclusively("?");
                    Result.Add(stack[stack.Count - 1].AssociatedLabels[1]);
                    Result.Add("БП");
                    Result.Add(stack[stack.Count - 1].AssociatedLabels[0] + ":");
                }
                else

                 if (token.GeneralizedValue.Equals(";"))
                {
                    StackElement innerElement = null;
                    if (stack.Count > 0)
                        innerElement = stack[stack.Count - 1];
                    while (stack.Count > 0 &&
                        !(innerElement.Value.Equals("if")
                        || innerElement.Value.Equals("for")
                        || innerElement.Value.Equals("?")))
                    {
                        if (!isHiddenSymbol(innerElement.Value))
                            Result.Add(innerElement.Value);
                        stack.RemoveAt(stack.Count - 1);
                        if (stack.Count > 0)
                        {
                            innerElement = stack[stack.Count - 1];
                        }
                    }
                    if (innerElement != null)
                    {
                        if (innerElement.Value.Equals("?"))
                        {
                            extruseUntilExclusively("?");
                            Result.Add(stack[stack.Count - 1].AssociatedLabels[1] + ":");
                            extruseUntil("?");
                            ternaryCount--;
                            if (ternaryCount == 0)
                                ternaryFlag = false;
                        }
                    }

                }



                notifyStats(token.GeneralizedValue);
            }
            extruseAll();
            notifyStats("");
            return Result;
        }

        private void extruse()
        {
            if (stack.Count > 1)
            {
                StackElement lastElement = stack[stack.Count - 1];
                StackElement innerElement = stack[stack.Count - 2];
                while (innerElement.StackPriority >= lastElement.Priority)
                {
                    if (!isHiddenSymbol(innerElement.Value))
                        Result.Add(innerElement.Value);
                    stack.RemoveAt(stack.Count - 2);
                    if (stack.Count > 1)
                    {
                        innerElement = stack[stack.Count - 2];
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        private void extruseUntil(String lastSign)
        {
            StackElement innerElement = stack[stack.Count - 1];
            while (stack.Count > 1 && !innerElement.Value.Equals(lastSign))
            {
                if (!isHiddenSymbol(innerElement.Value))
                    Result.Add(innerElement.Value);
                stack.RemoveAt(stack.Count - 1);
                if (stack.Count > 0)
                {
                    innerElement = stack[stack.Count - 1];
                }
            }

            if (!isHiddenSymbol(innerElement.Value))
                Result.Add(innerElement.Value);
            stack.RemoveAt(stack.Count - 1);

        }
        private void extruseUntilExclusively(String lastSign)
        {
            StackElement innerElement = null;
            if (stack.Count > 1)
                innerElement = stack[stack.Count - 1];
            else
                return;
            while (stack.Count > 1 && !innerElement.Value.Equals(lastSign))
            {
                if (!isHiddenSymbol(innerElement.Value))
                    Result.Add(innerElement.Value);
                stack.RemoveAt(stack.Count - 1);
                if (stack.Count > 0)
                {
                    innerElement = stack[stack.Count - 1];
                }
            }

        }
        private void extruseAll()
        {
            while (stack.Count > 0)
            {
                StackElement el = stack[stack.Count - 1];
                if (!isHiddenSymbol(el.Value))
                    Result.Add(el.Value);
                stack.RemoveAt(stack.Count - 1);
            }
        }
        private void notifyStats(String input)
        {
            ICGInfo info = new ICGInfo();
            info.Input = input;
            info.Result = "";
            foreach (String el in Result)
            {
                info.Result += el + " ";
            }
            info.Stack = "";
            foreach (StackElement el in stack)
            {
                if (el.AssociatedLabels == null)
                    info.Stack += el.Value + " ";
                else
                {
                    String labels = "";
                    foreach (String lbl in el.AssociatedLabels)
                    {
                        labels += lbl + " ";
                    }
                    info.Stack += el.Value + " " + labels;
                }
            }
            Information.Add(info);
        }
        private bool isHiddenSymbol(String symbol)
        {
            foreach (String hiddenSymbol in hiddenSymbols)
            {
                if (symbol.Equals(hiddenSymbol))
                    return true;
            }
            return false;
        }
    }
}
