using Ganss.Xss;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Static.API.Middleware;

/// <summary>
/// 
/// </summary>
/// <param name="next"></param>
public class AntiXssMiddleware(RequestDelegate next)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        var sanitizer = new HtmlSanitizer();

        //Sanitize query string parameters
        var sanitizedQuery = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in context.Request.Query)
        {
            var sanitizedValue = sanitizer.Sanitize(kvp.Value!);
            sanitizedQuery[kvp.Key] = new StringValues(sanitizedValue);
        }

        context.Request.Query = new QueryCollection(sanitizedQuery);

        //Sanitize headers
        foreach (var header in context.Request.Headers)
        {
            var sanitizedValue = sanitizer.Sanitize(header.Value!);
            context.Request.Headers[header.Key] = new StringValues(sanitizedValue);
        }

        //Sanitize form data
        if (context.Request.HasFormContentType)
        {
            if (context.Request.ContentType?.ToLower().StartsWith("multipart/form-data") != true)
            {
                var sanitizedForm = new Dictionary<string, StringValues>();
                foreach (var formValue in context.Request.Form)
                {
                    // Check if the form value is a string
                    if (formValue.Value.Count == 1 && !string.IsNullOrEmpty(formValue.Value[0]))
                    {
                        var sanitizedValue = sanitizer.Sanitize(formValue.Value[0]!);
                        sanitizedForm[formValue.Key] = new StringValues(sanitizedValue);
                    }
                    else
                    {
                        // If it's not a string, leave it untouched
                        sanitizedForm[formValue.Key] = formValue.Value;
                    }
                }

                context.Request.Form = new FormCollection(sanitizedForm);
            }
        }

        //Sanitize request body
        if (context.Request.ContentType?.ToLower().StartsWith("application/json") == true)
        {
            var originalBody = context.Request.Body;
            using var reader = new StreamReader(originalBody, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            var sanitizedBody = SanitizeJsonBody(body, sanitizer);
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(sanitizedBody));
        }

        await next(context);
    }

    private string SanitizeJsonBody(string body, HtmlSanitizer sanitizer)
    {
        var json = JToken.Parse(body);

        SanitizeToken(json, sanitizer);
        return json.ToString();
    }

    private void SanitizeToken(JToken token, HtmlSanitizer sanitizer)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
            {
                var obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    SanitizeToken(property.Value, sanitizer);
                }

                break;
            }
            case JTokenType.Array:
            {
                var array = (JArray)token;
                foreach (var child in array.Children())
                {
                    SanitizeToken(child, sanitizer);
                }

                break;
            }
            case JTokenType.String:
            {
                var value = ((JValue)token).Value<string>();
                var sanitizedValue = sanitizer.Sanitize(value!);
                sanitizedValue = Regex.Replace(sanitizedValue, "<.*?>", string.Empty);
                token.Replace(JToken.FromObject(sanitizedValue));
                break;
            }
        }
    }
}