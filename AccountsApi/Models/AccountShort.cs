using System;
using AccountsApi.Infrastructure.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountsApi.Models {
    public class AccountShort {
        public AccountShort (Account account) {
            Id = account.Id;
            SName = account.SName;
            FName = account.FName;
            Country = account.Country;
            City = account.City;
            Phone = account.Phone;
            EMail = account.EMail;
            Sex = account.Sex;
            Birth = account.Birth;
            Joined = account.Joined;
            Status = account.Status;
            Premium = account.Premium;
        }
        public long Id { get; set; }
        public string SName { get; set; }

        public string FName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string EMail { get; set; }

        [JsonConverter (typeof (StringEnumConverter))]
        public SexEnum Sex { get; set; }

        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Birth { get; set; }

        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Joined { get; set; }

        public string Status { get; set; }
        public PremiumModel Premium { get; set; }
    }
}