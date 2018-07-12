using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TvMaze.Models
{
    public class TvShow
    {
        [BsonId]
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Person> Cast { get; set; }

        public TvShow()
        {
            Cast = new List<Person>();
        }
    }
}
