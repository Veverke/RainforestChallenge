

using Newtonsoft.Json;
using RainforestChallenge;
using System.Text.Json.Serialization;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

_getRequest(httpClient, "https://www.letsrevolutionizetesting.com/challenge").Wait();


async Task _getRequest(HttpClient httpClient, string uri)
{
    bool makeNextRequest = false;
    var counter = 0;
    var uriHistory = new HashSet<string>();
    string responseContent;
    do 
    {
        if (uriHistory.Contains(uri))
        {
            Console.WriteLine($"uri [{uri}] already visited... aborting.");
            return;
        }
        uriHistory.Add(uri);
        var response = await httpClient.GetAsync(uri);
        await Console.Out.WriteLineAsync($"Performed get #[{++counter}] to {uri}... ");

        if (!response.IsSuccessStatusCode)
        {
            await Console.Out.WriteLineAsync($"Get request failed. Code: [{(int)response.StatusCode} - {response.StatusCode}]");
            return;
        }

        responseContent = await response.Content.ReadAsStringAsync();
        var responseContentObj = JsonConvert.DeserializeObject<RainforestGetResponse>(responseContent);
        if (!string.IsNullOrEmpty(responseContentObj?.message) && responseContentObj.message.Contains("This is not the end"))
        {
            uri = responseContentObj.follow;
            makeNextRequest = true;
        }
        else
        {
            makeNextRequest = false;
        }
    }
    while (makeNextRequest);

    await Console.Out.WriteLineAsync($"Congratulations, you made it! Response: [{responseContent}]");
}