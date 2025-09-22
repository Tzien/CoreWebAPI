namespace CoreWebApi.Models
{
    public record LoginRequest(string UserName, string Password);
    public record LoginResponse(string Token, object User);
    public class PagedResult<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> List { get; set; } = Enumerable.Empty<T>();
    }
}