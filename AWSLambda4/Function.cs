using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda4;

public class Function
{

    public async Task<APIGatewayHttpApiV2ProxyResponse> GetAllMoviesAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            DynamoDBContext dbContext = new DynamoDBContext(client);
            var data = await dbContext.ScanAsync<Movie>(default).GetRemainingAsync();

            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = JsonConvert.SerializeObject(data),
                StatusCode = 200
            };
        }
        catch(Exception ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = ex.Message,
                StatusCode = 200
            };
        }
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> CreateMovieAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try {
            var movieRequest = JsonConvert.DeserializeObject<Movie>(request.Body);
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            DynamoDBContext dbContext = new DynamoDBContext(client);
            await dbContext.SaveAsync(movieRequest);
            var message = $"Student with Id {movieRequest?.Id} Created";
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
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            DynamoDBContext dbContext = new DynamoDBContext(client);
            request.PathParameters.TryGetValue("id", out var id);
            int id_int = Int32.Parse(id);
            var movie = await dbContext.LoadAsync<Movie>(id_int);
            if (movie == null) throw new Exception("Not Found!");
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
                StatusCode = 400
            };
        }
    }
}
