using ECC_Practice.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC;

namespace ECC_Practice;
using System;


public class APITesting
{
    public const string HOST = "https://localhost:7277";
    
    public async Task<string[]> getPublicKey_from_API()
    {
        Console.WriteLine("[+] Get PublicKey Phase: ");
        using (var client = new HttpClient())
        {
            var response = client.GetAsync(HOST+"/publicKey").Result;
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();
            var point = JsonConvert.DeserializeObject<PointObject>(body);
            
            Console.WriteLine($"\t[-] server Public: {point?.x},{point?.y}");
            return new string[] { point.x, point.y };
        }
    }
    public async void sessionRequest(string x, string y)
    {
        Console.WriteLine("[+] SessionKey Request Phase: ");
        using (var client = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PointObject {x = x, y = y});
                var content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var url = HOST+"/sessionKey";
                var response = client.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\t[-] Result: {body}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }
    }

    public async Task<MessageObject> dataRequest(string msg, string r, PointObject PublicKey)
    {
        Console.WriteLine("[+] Data Reqeust Phase: ");
        using (var client = new HttpClient())
        {
            var json = JsonConvert.SerializeObject(new MessageObject() { message = msg, signature = r, publicKey = PublicKey});
            var content = new StringContent(json);
            content.Headers.ContentType =  new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var url = HOST + "/dataReqeust";
            
            var response = client.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();
            MessageObject responseJson = JsonConvert.DeserializeObject<MessageObject>(body);
            Console.WriteLine($"\t[-] Received Message: {responseJson.message}");
            Console.WriteLine($"\t[-] Received Signature: {responseJson.signature}");
            return responseJson;
        }
    }
}