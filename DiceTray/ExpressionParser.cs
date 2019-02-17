using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiceTray.Expressions;

namespace DiceTray
{
    public class ExpressionParser
    {
        public static IExpression Parse(string formula, Random random = null)
        {
            var substitutionDictionary = new Dictionary<string, string>();
            formula = formula.Replace(" ", "");
            formula = Substitute(formula, substitutionDictionary);
            return ParseInternal(formula, random ?? new Random(), substitutionDictionary);
        }

        private static string Substitute(string formula, Dictionary<string, string> substitutionDictionary)
        {
            if (formula.IndexOf("(", StringComparison.InvariantCulture) == -1)
                return formula;
            var formulaChars = formula.ToCharArray();
            var openBrackets = new Stack<int>();
            for (int i = 0; i < formulaChars.Length; i++)
            {
                var curChar = formulaChars[i];
                if (curChar == '(')
                {
                    openBrackets.Push(i);
                }
                if (curChar == ')')
                {
                    var startBracket = openBrackets.Pop();
                    var exp = formula.Substring(startBracket, i - startBracket + 1);
                    var sub = $"#{substitutionDictionary.Count}#";
                    formula = formula.Replace(exp, sub);
                    substitutionDictionary.Add(sub, exp.Trim(new []{'(', ')'}));
                    formulaChars = formula.ToCharArray();
                    i = startBracket + 2;
                }
            }

            return formula;
        }

        private static IExpression ParseInternal(string formula, Random random, Dictionary<string, string> substitutionDictionary)
        {
            IExpression result = null;
            var reg = new Regex("#[0-9]+#");
            if (reg.Match(formula).Value.Equals(formula))
            {
                formula = substitutionDictionary[formula];
            }
            
            // Add + Subtract
            var subExpr = formula.Split(new[] { '+', '-' });
            if (subExpr.Length > 1)
            {
                var opsString = (string)formula.Clone();
                foreach (var sub in subExpr)
                {
                    opsString = opsString.Remove(opsString.IndexOf(sub, StringComparison.InvariantCulture), sub.Length);
                }
                var ops = opsString.ToCharArray();
                var expr = subExpr.Select(f => ParseInternal(f, random, substitutionDictionary)).ToList();
                result = new CalculationExpression(ops[0].ToString(), expr[0], expr[1]);
                for (var i = 2; i < expr.Count; i++)
                {
                    result = new CalculationExpression(ops[i - 1].ToString(), result, expr[i]);
                }
            }
            if (result != null)
                return result;


            // Multiply + Divide
            subExpr = formula.Split(new[] { '*', '/' });
            if (subExpr.Length > 1)
            {
                var opsString = (string)formula.Clone();
                foreach (var sub in subExpr)
                {
                    opsString = opsString.Remove(opsString.IndexOf(sub, StringComparison.InvariantCulture), sub.Length);
                }
                var ops = opsString.ToCharArray();
                var expr = subExpr.Select(f => ParseInternal(f, random, substitutionDictionary)).ToList();
                result = new CalculationExpression(ops[0].ToString(), expr[0], expr[1]);
                for (var i = 2; i < expr.Count; i++)
                {
                    result = new CalculationExpression(ops[i - 1].ToString(), result, expr[i]);
                }
            }
            if (result != null)
                return result;

            // Dice
            if (formula.Contains("d"))
                result = new DiceExpression(formula, random);
            if (result != null)
                return result;

            // Constant
            result = new ConstantExpression(formula);

            return result;
        }
    }
}
