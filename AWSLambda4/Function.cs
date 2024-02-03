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
