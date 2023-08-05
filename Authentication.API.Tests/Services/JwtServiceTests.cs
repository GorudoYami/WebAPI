using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Data.Models;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebAPI.Data;
using System.Text;

namespace WebAPI.Tests.Services;

[TestFixture]
public class JwtServiceTests {
	private const int TOKEN_LIFETIME = 10;
	private const string TOKEN_KEY = "tsDoYsoqaKnbKvnkwCEgmXnsxe4s3BJqAMdYPtyowQqIWd1RQCnKnIYT9MWGfCI2";
	private const string TOKEN_ISSUER = "gorudoyami.net";
	private const string TOKEN_AUDIENCE = "gorudoyami.net";

	private JwtService Jwt { get; set; }

	[SetUp]
	public void Setup() {
		var logger = new Mock<ILogger<IJwtService>>();

		var gvData = new List<GlobalVariable>() {
			new GlobalVariable() {
				Name = "TokenLifetime",
				Value = TOKEN_LIFETIME.ToString()
			},
			new GlobalVariable() {
				Name = "TokenKey",
				Value = TOKEN_KEY
			},
			new GlobalVariable() {
				Name = "TokenIssuer",
				Value = TOKEN_ISSUER
			},
			new GlobalVariable() {
				Name = "TokenAudience",
				Value = TOKEN_AUDIENCE
			}
		}.AsQueryable();

		var gvSet = new Mock<DbSet<GlobalVariable>>();
		gvSet.As<IQueryable<GlobalVariable>>().Setup(gv => gv.Provider).Returns(gvData.Provider);
		gvSet.As<IQueryable<GlobalVariable>>().Setup(gv => gv.Expression).Returns(gvData.Expression);
		gvSet.As<IQueryable<GlobalVariable>>().Setup(gv => gv.ElementType).Returns(gvData.ElementType);
		gvSet.As<IQueryable<GlobalVariable>>().Setup(gv => gv.GetEnumerator()).Returns(gvData.GetEnumerator());

		var database = new Mock<DatabaseContext>();
		database.Setup(db => db.GlobalVariables).Returns(gvSet.Object);

		Jwt = new JwtService(logger.Object, database.Object);
	}

	[Test]
	public void Constructor_TokenParametersAreSetAndCorrect() {
		string tokenKey = Encoding.UTF8.GetString(Jwt.TokenKey.Key);

		Assert.AreEqual(tokenKey, TOKEN_KEY);
		Assert.AreEqual(Jwt.TokenLifetime, TOKEN_LIFETIME);
		Assert.AreEqual(Jwt.TokenIssuer, TOKEN_ISSUER);
		Assert.AreEqual(Jwt.TokenAudience, TOKEN_AUDIENCE);
	}

	[Test]
	public void CreateEncodedJwt_ResultIsNotNull() {
		string result = Jwt.CreateEncodedJwt(0);

		Assert.IsNotNull(result);
	}

	[Test]
	public void GetPasswordHash_HashIsCorrect() {
		const string password = "testpassword";
		const string salt = "dGVzdHNhbHQ=";
		const string expectedResult = "7Ncpwk/wPnoKHXdqpX2ADzXAhQiFK9CqHx5Ref2+3YyBJIVffZ9YM+n2A5PvtQm5vALvanL/blxecZ/3rNrhKQ==";

		string result = Jwt.GetPasswordHash(password, salt);

		Assert.AreEqual(expectedResult, result);
	}
}