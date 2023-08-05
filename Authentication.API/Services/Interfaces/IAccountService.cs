using System.Threading.Tasks;

namespace WebAPI.Services.Interfaces;

public interface IAccountService {
	public Task<ServiceResult> LoginAsync(LoginDTO loginDTO);
	public Task<ServiceResult> RegisterAsync(RegisterDTO registerDTO);
	public ServiceResult Refresh(string token);
	public Task<ServiceResult> ChangePasswordAsync(PasswordDTO passwordDTO, string token);
	public Task<ServiceResult> GetPermissionsAsync(int accountId);
}
