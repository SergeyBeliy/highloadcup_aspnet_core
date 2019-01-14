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
        public SexEnum? Sex { get; set; }

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

        [Column ("like_ids")]
        public long[] LikeIds {
            get {
                if (Likes == null) {
                    return new long[0];
                }
                return Likes.Select (s => s.Id).ToArray ();
            }

            set {
                if (value == null) {
                    Likes = null;
                } else {
                    Likes = value.Select(v => new LikeModel{Id = v}).ToArray();
                } 
            }
        }

        [Column ("like_tss")]
        public long[] LikeTSs {
            get {
                if (Likes == null) {
                    return new long[0];
                }
                return Likes.Select (s => s.Ts).ToArray ();
            }
            set {
                if (value == null) {
                    Likes = null;
                } if( Likes != null && Likes.Length == value.Length) {
                    for(var i =0; i<value.Length; i++){
                        Likes[i].Ts = value[i];
                    }
                }
            }
        }

        [NotMapped]
        public LikeModel[] Likes { get; set; }

        [NotMapped]
        public string[] LikesJson {
            get {
                if (Likes == null) {
                    return new string[0];
                }
                return Likes.Select (l => JsonConvert.SerializeObject (l)).ToArray ();
            }

            set {
                Likes = value.Select (v => JsonConvert.DeserializeObject<LikeModel> (v)).ToArray ();
            }
        }

        [NotMapped]
        public PremiumModel Premium { get; set; }

        [Column ("premium_start")]
        public long? PremiumStart {
            get {
                return Premium == null?(long?) null : SecondEpochConverter.ConvertTo (Premium.Start);
            }

            set {
                if (value.HasValue) {
                    if (Premium == null)
                        Premium = new PremiumModel ();
                    Premium.Start = SecondEpochConverter.ConvertFrom (value.Value);
                } else {
                    Premium = null;
                }
            }
        }

        [Column ("premium_finish")]
        public long? PremiumFinish {
            get {
                return Premium == null?(long?) null : SecondEpochConverter.ConvertTo (Premium.Finish);
            }

            set {
                if (value.HasValue) {
                    if (Premium == null)
                        Premium = new PremiumModel ();
                    Premium.Finish = SecondEpochConverter.ConvertFrom (value.Value);
                } else {
                    Premium = null;
                }
            }
        }

    }
}