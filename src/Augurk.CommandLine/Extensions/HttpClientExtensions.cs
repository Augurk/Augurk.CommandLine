using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Augurk.CommandLine
{
    /// <summary>
    /// Provides Augurk specific extensionmethods for the HttpClient
    /// </summary>
    public static class HttpClientExtensions
    {
        public static async Task<string> GetAugurkVersionAsync(this HttpClient httpClient)
        {
            var result = await httpClient.GetAsync("api/version");

            // Augurk versions up until 2.5.1 do not expose an api version
            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "2.5.1 or older";
            }
            
            string version = await result.Content.ReadAsStringAsync();

            Console.WriteLine($"Connect with Augurk version {version} at {httpClient.BaseAddress}");

            return version;
        }
    }
}
