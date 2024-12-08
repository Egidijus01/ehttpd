using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public static class Dispatcher
    {
        public static async Task SendResponse(HttpListenerResponse response, HttpStatusCode statusCode, string message)
        {
            response.StatusCode = (int)statusCode;
            byte[] responseData = Encoding.UTF8.GetBytes(message);

            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = responseData.Length;

            await response.OutputStream.WriteAsync(responseData, 0, responseData.Length);
        }

        public static async Task SendErrorResponse(HttpListenerResponse response, HttpStatusCode statusCode, string message)
        {
            await SendResponse(response, statusCode, $"{{\"error\": \"{message}\"}}");
        }
    }
}
