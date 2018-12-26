using System;

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

        public SexEnum Sex { get; set; }

        public long Birth { get; set; }

        public long Joined { get; set; }

        public string Status { get; set; }
        public PremiumModel Premium { get; set; }
    }
}