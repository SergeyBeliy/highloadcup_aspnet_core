namespace AccountsApi.Models
{
    public class LikeUpdateModel
    {
        public long Likee { get; set; }

        public long Ts { get; set; }

        public long Liker { get; set; }
    }
}