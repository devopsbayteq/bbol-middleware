using MediatR;
using System.Text.Json.Serialization;

namespace Common.WebApi.Models;
public interface IApiBaseRequest
{
    /// <inheritdoc />
    [JsonIgnore]
    ContextRequest ContextRequest { get; set; }
}

/// <summary>
/// RequestBase
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IApiBaseRequest<out TResponse> : IApiBaseRequest, IRequest<TResponse>
{

}