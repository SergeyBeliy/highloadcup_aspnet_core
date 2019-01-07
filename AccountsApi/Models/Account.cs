using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AccountsApi.Infrastructure.Helpers;
using Newtonsoft.Json;

namespace AccountsApi.Models {
    [Table ("accounts")]
    public class Account {
        [Key]
        [Column ("id")]
        public long Id { get; set; }

        [MaxLength (50)]
        [Column ("sname")]
        public string SName { get; set; }

        [MaxLength (50)]
        [Column ("fname")]
        public string FName { get; set; }

        [MaxLength (50)]
        [Column ("country")]
        public string Country { get; set; }

        [MaxLength (50)]
        [Column ("city")]
        public string City { get; set; }

        [MaxLength (16)]
        [Column ("phone")]
        public string Phone { get; set; }

        [MaxLength (100)]
        [Column ("email")]
        public string EMail { get; set; }

        [Column ("sex")]
        public SexEnum Sex { get; set; }

        [Column ("birth")]
        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Birth { get; set; }

        [Column ("joined")]
        [JsonConverter (typeof (SecondEpochConverter))]
        public DateTime Joined { get; set; }

        [MaxLength (10)]
        [Column ("status")]
        public string Status { get; set; }

        [Column ("interests")]
        public string[] Interests { get; set; }

        [NotMapped]
        public LikeModel[] Likes { get; set; }

        [Column (name: "likes", TypeName = "json[]")]
        public string[] LikesJson {
            get {
                if (Likes == null) {
                    return new string[0];
                }
                return Likes.Select (l => JsonConvert.SerializeObject (l)).ToArray ();
            }

            set {
                Likes = value.Select(v => JsonConvert.DeserializeObject<LikeModel>(v)).ToArray();
            }
        }

        [NotMapped]
        public PremiumModel Premium { get; set; }

        [Column (name: "premium", TypeName = "json")]
        public string PremiumJson {
            get {
                return JsonConvert.SerializeObject (Premium);
            }
            set {
                Premium =  JsonConvert.DeserializeObject<PremiumModel>(value);
            }
        }
    }
}