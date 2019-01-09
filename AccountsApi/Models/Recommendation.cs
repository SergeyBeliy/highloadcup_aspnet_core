using System;
using AccountsApi.Infrastructure.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountsApi.Models {
    public class Recommendation {
        public Recommendation (Account account) {
            Id = account.Id;
            EMail = account.EMail;
            Status = account.Status;
            SName = account.SName;
            FName = account.FName;
            Birth = account.Birth;
            Premium = account.Premium;
        }
        public long Id { get; set; }

        [JsonProperty ("sname")]
        public string SName { get; set; }

        [JsonProperty ("fname")]
        public string FName { get; set; }

        [JsonProperty ("email")]
        public string EMail { get; set; }

        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Birth { get; set; }
        public string Status { get; set; }
        public PremiumModel Premium { get; set; }

    }
}