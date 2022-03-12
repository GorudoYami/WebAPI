namespace WebAPI.Services.Interfaces;

public interface IJwtService : IService
{
	public string CreateEncodedJwt(int accountId);
	public string GetPasswordHash(string password, string saltString);
	public (string hash, string salt) GetPasswordHash(string password);
	public string GetSubject(string token);
}
