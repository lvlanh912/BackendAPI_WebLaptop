namespace Backend_WebLaptop.Configs
{
    public class DatabaseConfig
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? AccountsCollections { get; set; }
        public string? ProvincesCollections { get; set; }
        public string? DistrictsCollections { get; set; }
        public string? WardsCollections { get; set; }
        public string? ShippingAddressCollections { get; set; }
        public string? ProductsCollections { get; set; }
        public string? CategoryCollections { get; set; }
        public string? CommentsCollections { get; set; }
        public string? CartsCollections { get; set; }
        public string? OrdersCollections { get; set; }
        public string? PaymentsCollections { get; set; }
        public string? StatusOrderCollections { get; set; }
        public string? VouchersCollections { get; set; }
        public string? StatusOrderingsCollections { get; set; }
        public string? NewsCollections { get; set; }
        public string? SessionsCollections { get; set; }
        public string? ChatsCollections { get; set; }

    }
    public class AuthenticationConfig
    {
        public string? SecretKey { get; set; }
      

    }
}
