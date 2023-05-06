namespace ElsaMina.Core.Services.Login;

public interface ILoginService
{
    Task<LoginResponseDto> Login(string challstr);
}