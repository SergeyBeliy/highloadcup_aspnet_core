using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AccountsApi.Infrastructure.Helpers;
using Newtonsoft.Json;

namespace AccountsApi.Models {
    public class PremiumModel {

        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Start { get; set; }

        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Finish { get; set; }
    }
}