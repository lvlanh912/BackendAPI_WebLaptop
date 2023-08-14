using MongoDB.Driver;

namespace Backend_WebLaptop.Database

{
    public class Database_Service
    {
        public IMongoDatabase mongoDatabase;
        public Database_Service()
        {
           /* var mongoClient = new MongoClient("mongodb+srv://lvlanh:Lanh1234@atlascluster.ta2xu4y.mongodb.net/?retryWrites=true&w=majority");
            this.mongoDatabase = mongoClient.GetDatabase("Laptop_Store");*/
            var mongoClient = new MongoClient("mongodb://localhost:27017/?retryWrites=true&w=majority");
            this.mongoDatabase = mongoClient.GetDatabase("Laptop_Store");
        }
    }
}
