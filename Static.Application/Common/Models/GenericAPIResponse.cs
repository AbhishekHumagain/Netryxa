namespace Static.Application.Common.Models
{
    public class GenericApiResponse<T>
    {
        public bool Success { get; set; } = false;
        public T? Data { get; set; }
        public List<string>? Error { get; set; }

    }
}
