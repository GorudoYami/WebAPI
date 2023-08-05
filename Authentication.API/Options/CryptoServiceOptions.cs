using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Authentication.API.Options;

public class CryptoServiceOptions {
	public const string SectionName = "CryptoService";
	public KeyDerivationPrf Algorithm { get; set; }
	public int IterationCount { get; set; }
	public int KeyBitLength { get; set; }
	public int SaltBitLength { get; set; }
}
