using Moq;
using Moq.Protected;
using Movies.Client.Exceptions;
using Movies.Client.Handlers;
using Movies.Client.Testing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace Movies.Test
{
    public class TestableClassWithApiAccessUnitTests
    {
        [Fact]
        public void FindOne_On401Response_ThrowUnauthorizedApiAccess()
        {
            HttpClient httpClient = new(new Return401UnauthorizedResponseHandler());
            TestableClassWithApiAccess testableClass = new(httpClient);
            CancellationTokenSource cancellationTokenSource = new();

            Assert.ThrowsAsync<UnauthorizedApiAccessException>(
                () => testableClass.FindOne(cancellationTokenSource.Token));
        }

        [Fact]
        public void FindOne_On401Response_ThrowUnauthorizedApiAccess_WithMoq()
        {
            Mock<HttpMessageHandler> mock = new();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });

            HttpClient httpClient = new(mock.Object);

            TestableClassWithApiAccess testableClass = new(httpClient);

            CancellationTokenSource cancellationTokenSource = new();

            Assert.ThrowsAsync<UnauthorizedApiAccessException>(
                () => testableClass.FindOne(cancellationTokenSource.Token));
        }
    }
}
