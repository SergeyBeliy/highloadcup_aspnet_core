using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AccountsApi.Models {

    public class LikeModel {

        public long Id { get; set; }

        public long Ts { get; set; }
    }
}