using System.Collections.Concurrent;
using System.Net;

class CachingProxyServer
{
    private static readonly ConcurrentDictionary<string, string> Cache = new();
    private static readonly HttpClient httpClient = new();

    static async Task Main(string[] args)
    {
        string? originUrl = null;
        int port = 0;
        bool clearCache = false;

        // Parse command-line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--origin" when i + 1 < args.Length:
                    originUrl = args[i + 1];
                    break;
                case "--port" when i + 1 < args.Length:
                    port = int.Parse(args[i + 1]);
                    break;
                case "--clear-cache":
                    clearCache = true;
                    break;
            }
        }

        if (clearCache)
        {
            Cache.Clear();
            Console.WriteLine("Cache cleared.");
            return;
        }

        if (port == 0 || originUrl == null)
        {
            Console.WriteLine("Usage: caching-proxy --port <number> --origin <url>");
            return;
        }

        string prefix = $"http://localhost:{port}/";
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(prefix);
        listener.Start();

        Console.WriteLine($"Caching proxy server started on {prefix}");
        Console.WriteLine($"Forwarding requests to {originUrl}");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            _ = Task.Run(() => HandleRequest(context, originUrl));
        }
    }

    private static async Task HandleRequest(HttpListenerContext context, string originUrl)
    {
        string requestPath = context.Request.Url.PathAndQuery;
        string requestUrl = $"{originUrl}{requestPath}";

        // Check if the request is in the cache
        if (Cache.TryGetValue(requestUrl, out string cachedResponse))
        {
            Console.WriteLine($"Cache HIT: {requestUrl}");
            context.Response.Headers["X-Cache"] = "HIT";
            await WriteResponse(context, cachedResponse);
        }
        else
        {
            Console.WriteLine($"Cache MISS: {requestUrl}");
            context.Response.Headers["X-Cache"] = "MISS";

            // Forward request to origin server
            try
            {
                HttpRequestMessage forwardRequest = new HttpRequestMessage(new HttpMethod(context.Request.HttpMethod), requestUrl);
                HttpResponseMessage forwardResponse = await httpClient.SendAsync(forwardRequest);

                string responseBody = await forwardResponse.Content.ReadAsStringAsync();

                // Cache the response
                Cache[requestUrl] = responseBody;

                await WriteResponse(context, responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error forwarding request: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await WriteResponse(context, "Internal server error");
            }
        }
    }

    private static async Task WriteResponse(HttpListenerContext context, string responseBody)
    {
        context.Response.ContentType = "application/json"; // Assuming JSON responses
        using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
        {
            await writer.WriteAsync(responseBody);
        }
        context.Response.Close();
    }
}
