namespace Backend_WebLaptop.Model.OnlinePayment
{

      public  class PaymentLinkBag
        {
            public DateTime TimeOut { get; set; } = DateTime.Now.AddMinutes(15);
            public string PaymentLink { get; set; } = string.Empty;
        }
}
