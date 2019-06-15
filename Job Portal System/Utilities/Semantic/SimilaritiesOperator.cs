using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace Job_Portal_System.Utilities.Semantic
{
    public class SimilaritiesOperator
    {
        private const int MaxTokensNum = 3;

        private static readonly IComparer<IEnumerable<int>> TokensSetsComparer =
            Comparer<IEnumerable<int>>.Create((set1, set2) =>
            {
                var enumerable1 = set1 as int[] ?? set1.ToArray();
                var enumerable2 = set2 as int[] ?? set2.ToArray();
                return (enumerable1.Count() - enumerable1.Max()).CompareTo(enumerable2.Count() - enumerable2.Max());
            });

        public static IEnumerable<string> GetSimilarities(string sentence, IHostingEnvironment env)
        {
            var queryFilePath = Path.Combine(env.ContentRootPath, "Queries", "GetSimilarities.txt");
            var tokens = sentence.Split(" ");
            var setsLengths = GetSetsLengths(tokens.Length)
                .OrderBy(setLengths => setLengths, TokensSetsComparer);
            var tokenSimilaritiesDictionary = new Dictionary<string, List<string>>();
            var res = new List<string> { sentence };

            foreach (var setLengths in setsLengths)
            {
                var similarities = new List<List<string>>();
                var i = 0;
                var finished = true;

                foreach (var setLength in setLengths)
                {
                    var token = string.Join(" ", tokens, i, setLength);

                    if (tokenSimilaritiesDictionary.ContainsKey(token))
                    {
                        var tokenSimilarities = tokenSimilaritiesDictionary[token];
                        if (tokenSimilarities == null)
                        {
                            finished = false;
                            break;
                        }
                        similarities.Add(tokenSimilaritiesDictionary[token]);
                    }
                    else
                    {
                        var tokenSimilarities = GetTokenSimilarities(queryFilePath, token);
                        if (!tokenSimilarities.Any())
                        {
                            if (setLength == 1)
                            {
                                tokenSimilarities.Add(token);
                            }
                            else
                            {
                                tokenSimilaritiesDictionary.Add(token, null);
                                finished = false;
                                break;
                            }
                        }
                        similarities.Add(tokenSimilarities);
                        tokenSimilaritiesDictionary.Add(token, tokenSimilarities);
                    }
                    i += setLength;
                }

                if (!finished) continue;
                res.AddRange(GetCombinations(similarities));
                break;
            }
            return res.Distinct();
        }

        private static IEnumerable<List<int>> GetSetsLengths(int n)
        {
            if (n == 1)
            {
                return new List<List<int>>
                {
                    new List<int> { 1 },
                };
            }

            var res = new List<List<int>>();
            for (var i = n; i > 0; i--)
            {
                if (i > MaxTokensNum) continue;
                if (n > i)
                {
                    var smallerSets = GetSetsLengths(n - i);
                    foreach (var smallerSet in smallerSets)
                    {
                        var lengths = new List<int> { i };
                        lengths.AddRange(smallerSet);
                        res.Add(lengths);
                    }
                }
                else
                {
                    res.Add(new List<int> { i });
                }
            }
            return res;
        }

        private static List<string> GetTokenSimilarities(string queryFilePath, string token)
        {
            var endpoint = new FusekiConnector("http://localhost:3030/wordnet/data");
            var query = new SparqlParameterizedString
            {
                CommandText = File.ReadAllText(queryFilePath),
            };
            query.SetLiteral("query", token);

            return ((SparqlResultSet)endpoint.Query(query.ToString()))
                .Select(result => ((ILiteralNode)result["word"]).Value)
                .ToList();
        }

        private static IEnumerable<string> GetCombinations(IReadOnlyList<List<string>> similarities)
        {
            var res = new List<string>();
            var iterators = similarities.Select(t => 0).ToList();
            var shouldContinue = similarities.Any();
            while (shouldContinue)
            {
                var combination = new List<string>();
                var shouldIncrementIterator = true;
                for (var i = 0; i < similarities.Count(); i++)
                {
                    if (similarities[i].Any())
                    {
                        combination.Add(similarities[i][iterators[i]]);
                        if (!shouldIncrementIterator || iterators[i] >= similarities[i].Count()) continue;
                        if (iterators[i] == similarities[i].Count() - 1)
                        {
                            if (i == similarities.Count() - 1) shouldContinue = false;
                            iterators[i] = 0;
                        }
                        else
                        {
                            iterators[i]++;
                            shouldIncrementIterator = false;
                        }
                    }
                    else
                    {
                        shouldContinue = false;
                    }
                }
                res.Add(string.Join(" ", combination));
            }
            return res;
        }
    }
}
