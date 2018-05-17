using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubPullerDemo
{
    public class GitHubRepo
    {
        public string Name;
        public string Url;
        public string User;
    }
    
    static class Program
    {
        static void Main(string[] args)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GithubPullerDemo","0.1"));
                
                var response = client
                    .GetAsync("https://api.github.com/users/vector623/repos")
                    .Result;
                var responseText = response
                    .Content
                    .ReadAsStringAsync()
                    .Result;
                var responseJson = JsonConvert
                    .DeserializeObject<dynamic>(responseText);
                var responseData = new RouteValueDictionary(responseJson);

                var githubRepos = (responseData["Root"] as JArray)?
                    .Select(jsonObject => new GitHubRepo
                    {
                        Name = jsonObject["name"]?.ToString(),
                        Url = jsonObject["url"]?.ToString(),
                        User = jsonObject["owner"]?["login"]?.ToString(),
                    })
                    .ToList();
            }
        }
    }

}