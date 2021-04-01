using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.API.Entities
{
    //This will be a db table inside our MongoDb, so we should install the Mongo driver.

    //To identify this entity as Mongo entity, we should add MongoDb driver and use annotations. 
    //Install package MongoDb.Bson
    public class Product
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] //to use this Id column as Identity column inside our MongoDB
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonRequired]
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; } = 0;

    }
}
