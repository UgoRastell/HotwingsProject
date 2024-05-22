using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.MongoDB
{
    public class MongoIngredient
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Quantity")]
        public string Quantity { get; set; }
    }
}
