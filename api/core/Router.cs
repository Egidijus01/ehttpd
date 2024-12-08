using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API;

namespace Router
{
    public static class Route
    {
        public static async Task RouteRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.AbsolutePath == "/api/hello")
            {
                string responseMessage = "{\"info\": \"This is a test API endpoint.\"}";
                await APIDispatcher.SendResponse(response, HttpStatusCode.OK, responseMessage);
            }
            else if (request.Url.AbsolutePath == "/api/info")
            {
                string responseMessage = "{\"info\": \"This is a test API endpoint.\"}";
                await APIDispatcher.SendResponse(response, HttpStatusCode.OK, responseMessage);
            }
            else
            {
                // If no route matches, call the error dispatcher
                await APIDispatcher.SendErrorResponse(response, HttpStatusCode.NotFound, "API endpoint not found.");
            }
        }
    }
}
