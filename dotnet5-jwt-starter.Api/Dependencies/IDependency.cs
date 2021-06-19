using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet5_jwt_starter.Api.Dependencies
{
    public interface IDependency
    {
        void InitializeSevices(IServiceCollection services,IConfiguration configuration);
    }
}
