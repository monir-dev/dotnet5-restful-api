namespace dotnet5_jwt_starter.Api.Contracts.V1.Resposes
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
