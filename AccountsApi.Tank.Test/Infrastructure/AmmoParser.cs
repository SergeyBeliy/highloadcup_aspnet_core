using System.Collections.Generic;
using System.IO;
using AccountsApi.Tank.Test.Models;

namespace AccountsApi.Tank.Test.Infrastructure
{
    public static class AmmoParser
    {
        public static IEnumerable<AmmoModel> ParseGet(string ammo, string answer)
        {
            // string[] answerLines = File.ReadAllLines(answer);
            // string[] ammoLines = File.ReadAllLines(ammo);
            // var items = fileContent.Split('	');
            // AmmoModel item = null;
            // var result = new List<AmmoModel>();
            // for (var i = 0; i < items.Length; i++)
            // {
            //     if ((i + 1) % 4 == 1)
            //     {
            //         if (item != null)
            //             result.Add(item);
            //         item = new AmmoModel();
            //         item.Protocol = items[i];
            //     }
            //     if ((i + 1) % 4 == 2)
            //     {
            //         if(item != null)
            //             item.Url = items[i];
            //     }
            //     if ((i + 1) % 4 == 3)
            //     {
            //         if(item != null)
            //             item.ExpectedCode = parseInt(items[i]);
            //     }
            //     if ((i + 1) % 4 == 0)
            //     {
            //         if(item != null)
            //             item.ExpectedResponse = parseInt(items[i]);
            //     }
            // }
            return null;
        }

    }
}