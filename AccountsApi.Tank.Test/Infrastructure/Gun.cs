using System;
using AccountsApi.Tank.Test.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AccountsApi.Tank.Test.Infrastructure
{
    public class Gun
    {
        public string _baseAddress;
        HttpClient _httpClient = new HttpClient();
        public Gun(string baseAddress)
        {
            _baseAddress = baseAddress;
            _httpClient.BaseAddress = new Uri(baseAddress);
        }
        // private static HttpClient httpClient = new HttpClient();
        public async Task Fire (AmmoModel ammo){
            if (ammo.Protocol.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _httpClient.GetAsync(new Uri(ammo.Url));
                ammo.ActualCode = (int) response.StatusCode;
                ammo.ActualResponse = await response.Content.ReadAsStringAsync();
            }


        }
    }
}