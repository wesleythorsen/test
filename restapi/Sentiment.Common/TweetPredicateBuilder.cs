using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sentiment.Data.MySql;
using Sentiment.Common;
using LinqKit;

//using Serialize.Linq;

namespace Sentiment.Common
{
    // http://www.albahari.com/nutshell/predicatebuilder.aspx
    public static class TweetPredicateBuilder
    {

        //public static Expression<Func<Tweet, bool>> BuildPredicate(string str)
        //{
        //    str = str.Replace(", ", ",");
        //    var orTerms = str.Split(',')
        //        .Where(s => s != "")
        //        .ToArray();

        //    var predicates = new List<Expression<Func<Tweet, bool>>>();

        //    foreach (var orTerm in orTerms)
        //    {
        //        var andTerms = orTerm.Split(' ');
        //        var predicate = PredicateBuilder.New<Tweet>();

        //        foreach (var andTerm in andTerms)
        //        {
        //            var temp = andTerm;
        //            predicate = predicate.And(p => p.text.Contains(temp));
        //        }

        //        predicates.Add(predicate);
        //    }

        //    var fullPredicate = PredicateBuilder.New<Tweet>();

        //    foreach (var pred in predicates)
        //    {
        //        fullPredicate = fullPredicate.Or(pred);
        //    }

        //    return fullPredicate;
        //}

        //public static string CleanInput(string str)
        //{
        //    str = str.Replace("AND", "&").Replace("OR", "|");

        //    str = str.Replace(" ", "");

        //    str = str.ToLower();

        //    return str;
        //}

        //public static Expression<Func<Tweet, bool>> BuildPredicate3(string str)
        //{
        //    var a = new Serialize.Linq.Serializers.XmlSerializer();
        //    var b = a.Serialize(str.Select(c => c == 'a').);

        //    if (!str.Contains('(') && !str.Contains(')')) return BuildPredicateNoBrackets(str);

        //    var b1Index = str.IndexOf('(');

        //    var p1 = BuildPredicateNoBrackets(str.Remove(b1Index - 1));
            
        //    var b2Index = str.Substring(b1Index).IndexOf(')') + b1Index;

        //    var p2 = BuildPredicate3(str.Substring(b1Index + 1, b2Index - b1Index));

        //    if (str[b1Index - 1] == '&') p1 = p1.And(p2);
        //    else p1 = p1.Or(p2);

        //    return p1;
        //}

        public static bool ValidatePredicateString(string str)
        {
            var terms = str.Split('&', '|');

            if (terms.Any(t => !t.IsValidSearchTerm())) return false;
            if (terms.Any(t => string.IsNullOrWhiteSpace(str))) return false;
            if (terms.Any(t => string.IsNullOrEmpty(str))) return false;

            return true;
        }

        //public static Expression<Func<tweet, bool>> BuildPredicate(string str)
        //{
        //    var terms = str.Split('|').ToArray();

        //    var predicate = PredicateBuilder.New<tweet>();

        //    foreach (var term in terms)
        //    {
        //        var temp = term;
        //        predicate = predicate.Or(t => t.text.Contains(temp));
        //    }

        //    return predicate;
        //}

        public static Expression<Func<tweet, bool>> BuildPredicate(string str)
        {
            var orTerms = str.Split('|')
                .Where(s => s != "")
                .ToArray();

            var predicates = new List<Expression<Func<tweet, bool>>>();

            foreach (var orTerm in orTerms)
            {
                var andTerms = orTerm.Split('&');
                var predicate = PredicateBuilder.New<tweet>();

                foreach (var andTerm in andTerms)
                {
                    var temp = andTerm;
                    predicate = predicate.And(p => p.text.Contains(temp));
                }

                predicates.Add(predicate);
            }

            var fullPredicate = PredicateBuilder.New<tweet>();

            foreach (var pred in predicates)
            {
                fullPredicate = fullPredicate.Or(pred);
            }

            return fullPredicate;
        }


        //public static Expression<Func<Tweet, bool>> BuildPredicate2(string str)
        //{
        //    //0.   a & b & ( c | d & e ) | f | ( g & ( h & i ) ) & j
        //    //1.   a & b & (           ) | f | (               ) & j
        //    //2a.            c | d & e
        //    //2b.                                g & (       ) 
        //    //3.                                       h & i

        //    // while strprt != end of str:
        //    //     create predicate
        //    //     if '(' encountered:
        //    //         get matching ')' index
        //    //         set strptr to index of ')'
        //    //         make recursive call with inner text
        //    //         append result of recursion to predicate

        //    var predicate = PredicateBuilder.New<Tweet>();

        //    int i = 0;
        //    while (i < str.Length)
        //    {
        //        if (str[i] == '(')
        //        {
        //            int bracketCounter = 1;
        //            int innerStartIndex = i;
        //            while (bracketCounter != 0)
        //            {
        //                i++;
        //                if (str[i] == '(') bracketCounter++;
        //                if (str[i] == ')') bracketCounter--;
        //            }

        //            predicate = predicate.And(BuildPredicate2(str.Substring(innerStartIndex, i - innerStartIndex)));
        //        }
        //        else
        //        {

        //        }
        //    }
        //    return predicate;

        //    ///////////////////////////////////////////////////////////

        //    int j = 0;

        //    while (str[j] != '(')
        //    {
        //        j = str.IndexOfAny(new[] { '&', '|' });
        //        var term = str.Remove(j);
        //        char op = str[j];
        //        str = str.Substring(j + 1);

        //        j = 0;
        //    }
            
        //}

        //public static Expression<Func<T, bool>> True<T>() { return f => true; }
        //public static Expression<Func<T, bool>> False<T>() { return f => false; }

        //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        //                                                    Expression<Func<T, bool>> expr2)
        //{
        //    var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        //    return Expression.Lambda<Func<T, bool>>
        //          (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        //}

        //public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        //                                                     Expression<Func<T, bool>> expr2)
        //{
        //    var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        //    return Expression.Lambda<Func<T, bool>>
        //          (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        //}
    }
}
