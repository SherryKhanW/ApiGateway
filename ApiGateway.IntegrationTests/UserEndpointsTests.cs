using System.Net;
using System.Net.Http.Json;
using UserManagement.Grpc.Contracts;

namespace ApiGateway.IntegrationTests;

public class UserEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_WithExistingEmail_ReturnsOk()
    {
        var response = await _client.GetAsync("/users?email=test@example.com");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<GetUserResponse>();

        Assert.NotNull(body);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);
        Assert.Equal("test@example.com", body.Data.Email);
        Assert.Equal("Pakistan", body.Data.Country);
    }
	[Fact]
	public async Task GetUser_WithUnknownEmail_ReturnsNotFound()
	{
    // Arrange
		var email = "missing@example.com";

    // Act
    	var response = await _client.GetAsync($"/users?email={email}");

    // Assert
    	Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    	var body = await response.Content.ReadFromJsonAsync<GetUserResponse>();

    	Assert.NotNull(body);
    	Assert.False(body.Success);
    	Assert.Equal("User not found", body.ErrorMessage);
	}

	[Fact]
	public async Task GetUser_WithoutEmailOrUserId_ReturnsBadRequest()
	{
    // Act
    	var response = await _client.GetAsync("/users");

    // Assert
    	Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}