namespace WebAPI.Services.Interfaces;

public interface IJwtService {
	string CreateEncodedToken(int accountId);
	string GetSubject(string token);
	string RefreshToken(int accountId);
}
