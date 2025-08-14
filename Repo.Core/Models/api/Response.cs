namespace Repo.Core.Models.api;

public class Response<T>
{
    public bool Success { get; }
    public T Data { get; }
    public string Error { get; }

    private Response(bool success, T data, string error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public static Response<T> Ok(T data) => new(true, data, null);
    public static Response<T> Fail(string error) => new(false, default, error);
    
}