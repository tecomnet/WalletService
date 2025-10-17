using System.Net;
using System.Text;
using Newtonsoft.Json;
using Template.RestAPI.Models;
using Template.UnitTest.FixtureBase;

namespace Template.UnitTest.IntegrationTest;

public class UserApiTest : DatabaseTestFixture
{
    // Api URI
    private const string API_URI = "users";
    // Api version
    private const string API_VERSION = "0.1";


    public UserApiTest()
    {
        SetupDataAsync(async context =>
        {
            // Create data
            var commonSettings = new CommonSettings();
            // Add data
            //await context.AddRangeAsync(commonSettings.Users);
            // Save changes
            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

 
    
    #region Get test
        [Fact]
        public async Task Get_Unauthorized()
        {
            // Arrange
            var client = Factory.CreateClient();
            // Act
            var response = await client.GetAsync(
                requestUri: $"{API_VERSION}/{API_URI}");
            // Assert
            Assert.Equal(expected: HttpStatusCode.Unauthorized, actual: response.StatusCode);
        }
    
        [Fact]
        public async Task Get_ReturnsConfigNotFound()
        {
            // Arrange
            await ClearTransactionConfig();
            string[] expectedErrors = ["Error esperado"]; //{ ServiceErrorsBuilder.EmKeyValueConfigNotFound };
            var client = Factory.CreateAuthenticatedClient();
            // Act
            var response = await client.GetAsync( requestUri: $"{API_VERSION}/{API_URI}");
            // Read the content
            var responseContentString = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<InlineResponse400>(responseContentString);
            // Asserts
            Assert.Equal(expected: HttpStatusCode.NotFound, actual: response.StatusCode);
            Assert.NotNull(errorResult);
            var errors = errorResult.Errors;
            foreach (var error in errors)
                Assert.Contains(error.ErrorCode, expectedErrors);
        }
    
        [Fact]
        public async Task Get_Ok()
        {
            // Arrange
            var client = Factory.CreateAuthenticatedClient();
            // Act
            var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}");
            // Read the content
            var responseContentString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserResult>(responseContentString);
            // Assert 
            Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
            Assert.NotNull(result);
            //Assert.Equal(expected: nameof(TransactionsKeys.UnderscoreReplacementEnabled), actual: result.Key);
            // Check all key config result properties 
            ValidateProperties(result: result);
        }
    
        #endregion
        
        #region Post test
    [Fact]
    public async Task Post_Unauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Act
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/{API_URI}", content: null);
        // Assert
        Assert.Equal(expected: HttpStatusCode.Unauthorized, actual: response.StatusCode);
    }

    [Fact]
    public async Task Post_Ok()
    {
        // Arrange
        await ClearTransactionConfig();
        var client = Factory.CreateAuthenticatedClient();
        // Request
        var body = new UserUpdateRequest()
        {
            User = "uno"
        };
        // Create content
        var content = CreateContent(body: body);
        // Act
        var response = await client.PostAsync( requestUri: $"{API_VERSION}/{API_URI}", content:content);
        // Read the content
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UserResult>(responseContentString);
        // Asserts
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        Assert.NotNull(result);
        // Valida tipos
        ValidateProperties(result: result);
        // Valida datos enviados
        AssertProperties(body: body, result: result);
    }
    #endregion
    
    
    #region Put test
    [Fact]
    public async Task Put_Unauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Act
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/{API_URI}", content: null);
        // Assert
        Assert.Equal(expected: HttpStatusCode.Unauthorized, actual: response.StatusCode);
    }

    [Fact]
    public async Task Put_Ok()
    {
       
    }
    #endregion
    #region Private methods

    /// <summary>
    /// Validate the properties  
    /// </summary>
    /// <param name="result">The result</param>
    private void ValidateProperties(UserResult result)
    {
        // Validate type of properties 
        Assert.IsType<string>(result.User);
        // Verify the type of common properties
        Assert.IsType<Guid>(result.Guid);
        Assert.IsType<DateTime>(result.ModificationTimestamp);
        Assert.IsType<Guid>(result.ModificationUser);
        Assert.IsType<DateTime>(result.CreationTimestamp);
        Assert.IsType<Guid>(result.CreationUser);
    }

    /// <summary>
    /// Create string content
    /// </summary>
    /// <param name="body">any object</param>
    /// <returns>String content</returns>
    private StringContent CreateContent(object body)
    {
        // Serialize the object request
        var json = JsonConvert.SerializeObject(body);
        // Create content
        var content = new StringContent(
            content: json,
            encoding: Encoding.UTF8,
            mediaType: "application/json");
        // Return content
        return content;
    }


    /// <summary>
    /// Get the config result from database
    /// </summary>
    /// <returns>TransaccionesConfigResult</returns>
    private async Task<UserResult?> GetTransactionConfigFromDataBase()
    {
       
        // Create client
        var client = Factory.CreateAuthenticatedClient();
        // Call api method
        var response = await client.GetAsync(
            requestUri: $"{API_VERSION}/{API_URI}");
        // Read the content
        var responseContentString = await response.Content.ReadAsStringAsync();
        // Is ok status
        if (response.StatusCode != HttpStatusCode.OK)
            return null;
        else
        {
            // Deserialize the object
            var result =
                JsonConvert.DeserializeObject<UserResult>(responseContentString);
            // Return the result
            return result;
        }
    }

    /// <summary>
    /// Clear all the ApiKeyChannel
    /// </summary>
    private async Task ClearTransactionConfig()
    {
        // Context
        var context = CreateContext();
        // Remueve la config de transaction
        //await context.KeyValueConfig.OfType<TransactionsKeyValueConfig>().ExecuteDeleteAsync();
        // Save changes
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Assert the properties sent
    /// </summary>
    /// <param name="body">body request</param>
    /// <param name="result">ApiKeyChannel result</param>
    private void AssertProperties(UserRequest body, UserResult result)
    {
        // Assert key value config properties 
        Assert.Equal(expected: body.User, actual: result.User);
    }

    /// <summary>
    /// Assert the properties sent
    /// </summary>
    /// <param name="body">body request</param>
    /// <param name="result">api key channel result</param>
    private void AssertProperties(UserUpdateRequest body, UserResult result)
    {
        // Assert key value config properties 
        Assert.Equal(expected: body.User, actual: result.User);
    }

    #endregion
   
}