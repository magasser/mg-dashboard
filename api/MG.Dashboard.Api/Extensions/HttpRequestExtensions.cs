namespace MG.Dashboard.Api.Extensions;

public static class HttpRequestExtensions
{
    private const string BearerPrefix = "Bearer ";

    public static bool TryGetToken(this HttpRequest request, out string? token)
    {
        token = null;
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!request.Headers.TryGetValue("Authorization", out var value))
        {
            return false;
        }

        string tokenValue = value!;
        if (string.IsNullOrWhiteSpace(tokenValue) || !tokenValue.StartsWith(BearerPrefix))
        {
            return false;
        }

        token = tokenValue.Substring(BearerPrefix.Length);
        return true;
    }
}