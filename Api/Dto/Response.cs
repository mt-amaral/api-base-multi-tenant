namespace Api.Dto;

public class Response<TData>
{
    public Response(
        TData? data,
        string? message = null)
    {
        Data = data;
        Message = message;
    }

    public Response(TData? data, string? message = null, List<string>? errors = null)
    {
        Data = data;
        Message = message;
        Errors = errors;
    }


    public TData? Data { get; set; }
    public string? Message { get; set; }

    public List<string>? Errors { get; set; }
}