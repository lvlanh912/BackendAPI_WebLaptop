namespace Backend_WebLaptop.IRespository
{
    public interface IVnPayRepository
    {
        public string CreatePaymentLink(string orderId);
    }
}
