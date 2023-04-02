namespace romeo_assistant.Middlewares
{
    public class LogRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LogRequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LogRequestMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log connection attempt
            _logger.LogInformation($"Connection attempt from IP: {context.Connection.RemoteIpAddress}");

            // Log the request content and media type
            context.Request.EnableBuffering(); // Enable request stream buffering
            var requestBody = await ReadStreamAsync(context.Request.Body);
            string requestMediaType = context.Request.ContentType ?? "Not specified";
            _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} Media Type: {requestMediaType} Body: {requestBody}");
            context.Request.Body.Position = 0; // Reset the stream position

            // Log the response content, status code, and media type
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            string responseMediaType = context.Response.ContentType ?? "Not specified";
            _logger.LogInformation($"Response: {context.Response.StatusCode} Media Type: {responseMediaType} Body: {responseBody}");

            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            stream.Position = 0;
            return content;
        }
    }
}
