using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace Job_Portal_System.Client
{
    public class NamedEntity
    {
        public long Id { get; set; }
        public string Label { get; set; }

        public static IEnumerable<NamedEntity> GetNamedEntities(string queryFileName)
        {
            var endpoint = new SparqlRemoteEndpoint(
                endpointUri: new Uri("http://dbpedia.org/sparql"),
                defaultGraphUri: "http://dbpedia.org");
            var queryString = new SparqlParameterizedString
            {
                CommandText = File.ReadAllText(queryFileName),
            };
            var results = endpoint.QueryWithResultSet(queryString.ToString());
            return results
                .Select(result => new NamedEntity
                {
                    Id = long.Parse(result["id"].ToString()),
                    Label = result["title"].ToString(),
                });
        }

        public static IEnumerable<NamedEntity> GetNamedEntities(string query, string queryFileName)
        {
            var endpoint = new SparqlRemoteEndpoint(
                endpointUri: new Uri("http://dbpedia.org/sparql"),
                defaultGraphUri: "http://dbpedia.org");
            var queryString = new SparqlParameterizedString
            {
                CommandText = File.ReadAllText(queryFileName),
            };
            query = query.First().ToString().ToUpper() + query.Substring(1).ToLower();
            queryString.SetLiteral("queryString", query, "en");
            var results = endpoint.QueryWithResultSet(queryString.ToString());
            return results
                .Select(result => new NamedEntity
                {
                    Id = long.Parse(result["id"].ToString()),
                    Label = result["title"].ToString(),
                });
        }
    }
}
