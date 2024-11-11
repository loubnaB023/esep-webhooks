using Amazon.Lambda.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(object input, ILambdaContext context){
        // Log the received input for debugging purposes
        context.Logger.LogInformation($"FunctionHandler received: {input}");

        // Parse the JSON input to extract the issue URL
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        // Create an HttpClient instance to send the Slack notification
        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        // Send the request and get the response
        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}
