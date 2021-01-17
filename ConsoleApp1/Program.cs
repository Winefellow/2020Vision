


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConsoleApp29
{
    public class Something
    {
        public string A { get; set; }
    }

    class Program
    {
        private static readonly string[] NotChars = { "!", "~" };

        private static Expression<Func<T, bool>> CreateContaintsExpression<T>(string str)
            where T : class
        {
            string[] split = str.Split('=');
            string value = NotChars.Select(@char => @char[0])
                .Aggregate(split[1], (current, @char) => current.TrimStart(@char)).Trim();

            ParameterExpression parameterExp = Expression.Parameter(typeof(T), "type");
            MemberExpression propertyExp = Expression.Property(parameterExp, split[0]);
            BinaryExpression nullCheckExp = Expression.NotEqual(propertyExp, Expression.Constant(null, typeof(object)));

            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(split[1], typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            // if not null and contains value…        
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(nullCheckExp, containsMethodExp), parameterExp);
        }

        private static string Search;

        private static IQueryable<Something> FilterSearch(IQueryable<Something> query)
        {
            if (string.IsNullOrEmpty(Search) == false)
            {
                MatchCollection fieldMatch = Regex.Matches(Search, @"^((\b[^\s=]+)=(([^=]|\\=)+))*$");

                if (fieldMatch.Count == 1)
                {
                    // For the case that the searchstring contains a single key/value pair indicating a specific field to search.
                    query = query.Where(ContaintsFieldMatchCondition<Something>(fieldMatch[0].Value));
                }
                else
                {
                    string[] keyWords;
                    if (Search[0] == '"')
                    {
                        // Ter overweging: We doen hier wel ongelooflijk moeilijk om een string die door quotes omgeven is te matchen...
                        keyWords = Search.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries).Select(x => "%" + x + "%").ToArray();
                    }
                    else
                    {
                        // We should write tests for this!
                        //    Deze voorbeelden werken niet:
                        //              : "test mij" => "%_test mij_%"
                        //              : "test.mij" => "Test" + "Mij"
                        //              

                        // Split a string that has white spaces, unless they are enclosed within “quotes”.
                        // Replace all non-alfanumeric characters by the wildcard character
                        keyWords = Regex.Matches(Search, @"[\""].+?[\""]|[^ ]+").Cast<Match>()
                           .Select(m => "%" + Regex.Replace(m.Value, "[^A-Za-z0-9]", "_").Trim().ToLower() + "%").ToArray();
                    }

                    // Split a string that has white spaces, unless they are enclosed within “quotes”.
                    // Replace all non-alfanumeric characters by the wildcard character

                    if (keyWords.Any())
                    {
                        query = query.Where(x =>
                            keyWords.Any(y => Like((x.A ?? string.Empty).ToLower(), y)));
                    }
                }

            }
            return query;
        }

        // returns true if (data LIKE keyword)
        private static bool Like(string data, string keyword)
        {
            return Regex.IsMatch(data, WildCardToRegular(keyword));
        }

        // Converts % en ? naar regex * en .
 
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("%_", ".*").Replace("_%", ".*").Replace("_", ".").Replace("%", ".*") + "$";
        }

        private static Expression<Func<T, bool>> ContaintsFieldMatchCondition<T>(string fieldValue)
            where T : class
        {
            string[] split = fieldValue.Split('=');
            string value = NotChars.Select(@char => @char[0]).Aggregate(split[1], (current, @char) => current.TrimStart(@char)).Trim();

            ParameterExpression parameterExp = Expression.Parameter(typeof(T), "type");
            MemberExpression propertyExp = Expression.Property(parameterExp, split[0]);
            BinaryExpression nullCheckExp = Expression.NotEqual(propertyExp, Expression.Constant(null, typeof(object)));

            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression someValue = Expression.Constant(value, typeof(string));
            MethodCallExpression containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(nullCheckExp, containsMethodExp), parameterExp);
        }



        static void Main(string[] args)
        {
            List<Something> list = new List<Something>
            {
                new Something { A = "yyy" },
                new Something { A = "zzz" },
                new Something { A = "aaa zzz xxx yyy" },
                new Something { A = "zzz ddd sss aaaa" },
                new Something { A = "zzz aav saff" }
            };

            List<Something> result = list.AsQueryable().Where(CreateContaintsExpression<Something>("A=yyy")).ToList();

            list.Add(new Something { A = null });

            Search= '"'+@"xxx yyy" + '"'; 
            var filtered = FilterSearch(list.AsQueryable()).ToList();

            Search = @"a=zzz xxx";
            filtered = FilterSearch(list.AsQueryable()).ToList();
            int nREcords = filtered.Count;
            result = list.AsQueryable().Where(CreateContaintsExpression<Something>("A=zzz xxx")).ToList();

            result = list.AsQueryable().Where(CreateContaintsExpression<Something>("A=yyy")).ToList();
        }
    }
}
