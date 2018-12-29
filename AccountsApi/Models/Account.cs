using System;

namespace AccountsApi.Models {
    public class Account {
        public long Id { get; set; }
        
        public string SName { get; set; }
        
        public string FName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string EMail { get; set; }

        public SexEnum Sex { get; set; }

        public long Birth { get; set; }

        public long Joined { get; set; }

        public string Status { get; set; }

        public string[] Interests { get; set;}

        public LikeModel[] Likes { get; set;}

        public PremiumModel Premium { get; set;}
    }
}