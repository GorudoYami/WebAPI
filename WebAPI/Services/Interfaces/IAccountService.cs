using System.Threading.Tasks;
using WebAPI.Data.TransferObjects;

namespace WebAPI.Services.Interfaces;

public interface IAccountService : IService
{
	public Task<ServiceResult> LoginAsync(LoginDTO loginDTO);
	public Task<ServiceResult> RegisterAsync(RegisterDTO registerDTO);
	public ServiceResult Refresh(string token);
	public Task<ServiceResult> ChangePasswordAsync(PasswordDTO passwordDTO, string token);
	public Task<ServiceResult> GetPermissionsAsync(int accountId);
}
