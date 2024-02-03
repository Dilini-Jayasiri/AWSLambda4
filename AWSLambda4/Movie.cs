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

        [DynamoDBProperty("minutes")]
        public string Minutes { get; set; }

        [DynamoDBProperty("synopsis")]
        public string? Synopsis {  get; set; }

        [DynamoDBProperty("posterUrl")]
        public string? PosterURL { get; set; }

        [DynamoDBProperty("tagline")]
        public string? Tagline { get; set; }

        [DynamoDBProperty("votes")]
        public string? Votes { get; set; }

        [DynamoDBProperty("releaseDate")]
        public string? ReleaseDate { get; set; }

        [DynamoDBProperty("genre")]
        public string? Genre { get; set; }

        [DynamoDBProperty("backdropURL")] 
        public string? BackdropURL { get; set; }

    }
}
