using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda4
{
    public class Function
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly DynamoDBContext _dbContext;


        public Function()
        {
            _dynamoDbClient = new AmazonDynamoDBClient();
            _dbContext = new DynamoDBContext(_dynamoDbClient);
        }


        public async Task<APIGatewayHttpApiV2ProxyResponse> GetAllMoviesAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var data = await _dbContext.ScanAsync<Movie>(default).GetRemainingAsync();

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = JsonConvert.SerializeObject(data),
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateMovieAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {

                var movieRequest = JsonConvert.DeserializeObject<Movie>(request.Body);
                movieRequest.Id = GenerateUniqueId();
                await _dbContext.SaveAsync(movieRequest);
                var message = $"Movie with Id {movieRequest?.Id} Created";
                LambdaLogger.Log(message);
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = message,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = ex.Message,
                    StatusCode = 400
                };
            }
        }

        public static int GenerateUniqueId()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();

            // Take the first 4 bytes and convert them to an integer
            int uniqueId = BitConverter.ToInt32(bytes, 0);

            return uniqueId;
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateMovieAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {

                var movieUpdateRequest = JsonConvert.DeserializeObject<Movie>(request.Body);

                // Validate if required fields are present in the update request, handle validation as needed

                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                DynamoDBContext dbContext = new DynamoDBContext(client);

                // Load the existing movie record from DynamoDB
                Movie existingMovie = await dbContext.LoadAsync<Movie>(movieUpdateRequest.Id);

                if (existingMovie == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "Movie not found",
                        StatusCode = 404
                    };
                }

                // Update the fields that are provided in the update request
                if (!string.IsNullOrEmpty(movieUpdateRequest.MovieName))
                {
                    existingMovie.MovieName = movieUpdateRequest.MovieName;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.ReleaseDate))
                {
                    existingMovie.ReleaseDate = movieUpdateRequest.ReleaseDate;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Rate))
                {
                    existingMovie.Rate = movieUpdateRequest.Rate;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Minutes))
                {
                    existingMovie.Minutes = movieUpdateRequest.Minutes;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Synopsis))
                {
                    existingMovie.Synopsis = movieUpdateRequest.Synopsis;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.PosterURL))
                {
                    existingMovie.PosterURL = movieUpdateRequest.PosterURL;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Tagline))
                {
                    existingMovie.Tagline = movieUpdateRequest.Tagline;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Votes))
                {
                    existingMovie.Votes = movieUpdateRequest.Votes;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.Genre))
                {
                    existingMovie.Genre = movieUpdateRequest.Genre;
                }

                if (!string.IsNullOrEmpty(movieUpdateRequest.BackdropURL))
                {
                    existingMovie.BackdropURL = movieUpdateRequest.BackdropURL;
                }


                // Add more fields to update as needed

                // Save the updated movie record back to DynamoDB
                await dbContext.SaveAsync(existingMovie);

                var message = $"Movie with Id {existingMovie.Id} Updated";
                LambdaLogger.Log(message);

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = message,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = ex.Message,
                    StatusCode = 400
                };
            }
        }


        public async Task<APIGatewayHttpApiV2ProxyResponse> GetMovieByIdAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                if (!request.PathParameters.TryGetValue("id", out var id) || !int.TryParse(id, out var id_int))
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "Invalid movie ID",
                        StatusCode = 400
                    };
                }

                var movie = await _dbContext.LoadAsync<Movie>(id_int);
                if (movie == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "Movie not found",
                        StatusCode = 404
                    };
                }

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = JsonConvert.SerializeObject(movie),
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}