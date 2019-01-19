using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace AccountsApi.Tank.Test
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Start firing.. ");
            var keys = new List<string>();
            var lines = File.ReadAllLines(@"c:\_MyProjects\highloadcup_aspnet_core\TestData\ammo\phase_1_get.ammo");
            foreach(var line in lines){
                if(line.StartsWith("GET /accounts/filter/?")){
                    var queryString = line.Replace("GET /accounts/filter/?", "");
                    // queryString = queryString.Substring(queryString.IndexOf("?"));
                    var dict = QueryHelpers.ParseQuery (queryString);
                    var fields = String.Join(" ",dict.Keys.Where(s => s != "query_id" && s != "limit").Select(s => s.Split("_")[0]).OrderBy(s => s).ToArray());

                    if(!keys.Contains(fields))
                        keys.Add(fields);
                }
            }
            foreach(var k in keys.OrderBy(s => s)){
                Console.WriteLine(k);
            }
            Console.WriteLine("Firing complete");
            Console.ReadLine();
        }
    }
}
