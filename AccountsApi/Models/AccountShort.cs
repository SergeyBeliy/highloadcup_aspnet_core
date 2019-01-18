using System;
using System.Linq;
using AccountsApi.Infrastructure.Database;
using AccountsApi.Infrastructure.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountsApi.Models
{
    public class AccountShort
    {
        public AccountShort(Account account, QueryBase query)
        {
            Id = account.Id;
            EMail = account.EMail;
            if (query.Items.Any(q => q.FieldName.Equals("SName", StringComparison.OrdinalIgnoreCase)))
                SName = account.SName;
            if (query.Items.Any(q => q.FieldName.Equals("FName", StringComparison.OrdinalIgnoreCase)))
                FName = account.FName;
            if (query.Items.Any(q => q.FieldName.Equals("Country", StringComparison.OrdinalIgnoreCase)))
                Country = account.Country;
            if (query.Items.Any(q => q.FieldName.Equals("City", StringComparison.OrdinalIgnoreCase)))
                City = account.City;
            if (query.Items.Any(q => q.FieldName.Equals("Phone", StringComparison.OrdinalIgnoreCase)))
                Phone = account.Phone;
            if (query.Items.Any(q => q.FieldName.Equals("Sex", StringComparison.OrdinalIgnoreCase)))
                Sex = account.Sex;
            if (query.Items.Any(q => q.FieldName.Equals("Birth", StringComparison.OrdinalIgnoreCase)))
                Birth = account.Birth;
            if (query.Items.Any(q => q.FieldName.Equals("Joined", StringComparison.OrdinalIgnoreCase)))
                Joined = account.Joined;
            if (query.Items.Any(q => q.FieldName.Equals("Status", StringComparison.OrdinalIgnoreCase)))
                Status = account.Status;
            if (query.Items.Any(q => q.FieldName.Equals("Premium", StringComparison.OrdinalIgnoreCase)))
                Premium = account.Premium;
        }
        public long Id { get; set; }

        [JsonProperty("sname")]
        public string SName { get; set; }

        [JsonProperty("fname")]
        public string FName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        [JsonProperty("email")]
        public string EMail { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SexEnum? Sex { get; set; }

        [JsonConverter(typeof(SecondEpochConverter))]
        public DateTime? Birth { get; set; }

        [JsonConverter(typeof(SecondEpochConverter))]
        public DateTime? Joined { get; set; }

        public string Status { get; set; }
        public PremiumModel Premium { get; set; }
    }
}