using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda4
{
    [DynamoDBTable("Movies")]
    public class Movie
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("movie_name")]
        public string? MovieName { get; set; }

        [DynamoDBProperty("rate")]
        public string? Rate { get; set; }

        [DynamoDBProperty("description")]
        public string Description { get; set; }

        [DynamoDBProperty("directorName")]
        public string? DirectorName {  get; set; }

        [DynamoDBProperty("duration")]
        public string? Duration { get; set; }

    }
}
