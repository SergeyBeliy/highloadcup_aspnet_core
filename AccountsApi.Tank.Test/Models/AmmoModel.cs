namespace AccountsApi.Tank.Test.Models
{
    public class AmmoModel
    {
        public long LoadTime { get; set; }
        public string Url { get; set; }

        public string PostData { get; set; }

        public string Protocol { get; set; }

        public int ExpectedCode { get; set; }

        public int ActualCode { get; set; }

        public string ExpectedResponse { get; set; }

        public string ActualResponse { get; set; }

        public string ContentType { get; set; }

        public bool Success
        {
            get
            {
                if (ActualCode != ExpectedCode)
                {
                    return false;
                }
                if (ActualResponse != ExpectedResponse)
                {
                    return false;
                }
                return true;
            }
        }
    }
}