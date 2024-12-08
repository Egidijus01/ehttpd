using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public static class APIDispatcher
    {
        public static async Task Dispatch(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                if (request.Url.AbsolutePath == "/api/hello")
                {
                    string responseMessage = "{\"message\": \"Hello, World!\"}";
                    byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);

                    response.ContentType = "application/json";
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = responseData.Length;

                    await response.OutputStream.WriteAsync(responseData, 0, responseData.Length);
                }
                else if (request.Url.AbsolutePath == "/api/info")
                {
                    string responseMessage = "{\"info\": \"This is a test API endpoint.\"}";
                    byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);

                    response.ContentType = "application/json";
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = responseData.Length;

                    await response.OutputStream.WriteAsync(responseData, 0, responseData.Length);
                }
                else
                {
                    string errorMessage = "{\"error\": \"API endpoint not found.\"}";
                    byte[] errorData = Encoding.UTF8.GetBytes(errorMessage);

                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.ContentType = "application/json";
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = errorData.Length;

                    await response.OutputStream.WriteAsync(errorData, 0, errorData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in API Dispatcher: {ex.Message}");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                byte[] errorMessage = Encoding.UTF8.GetBytes("{\"error\": \"Internal server error.\"}");
                response.ContentType = "application/json";
                response.ContentLength64 = errorMessage.Length;
                await response.OutputStream.WriteAsync(errorMessage, 0, errorMessage.Length);
            }
            finally
            {
                response.Close();
            }
        }
    }
}
