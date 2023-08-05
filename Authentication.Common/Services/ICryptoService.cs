using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Common.Services;
public interface ICryptoService {
	/// <summary>
	/// PBKDF2 key derivation in base64
	/// </summary>
	/// <param name="secret">Secret to derive key from</param>
	/// <param name="salt">Salt in base64</param>
	/// <returns>Base64 string representation of derived hash</returns>
	string GetPasswordHashBase64(string password, string salt);

	/// <summary>
	/// PBKDF2 key derivation in base64
	/// </summary>
	/// <param name="secret">Secret to derive key from</param>
	/// <param name="salt">Randomized salt in base64</param>
	/// <returns>Base64 string representation of derived hash</returns>
	string GetPasswordHashBase64(string password, out string salt);
}
