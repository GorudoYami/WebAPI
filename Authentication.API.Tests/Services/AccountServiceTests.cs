using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI.Tests.Services;

[TestFixture]
public class AccountServiceTests {
	private AccountService Accounts { get; set; }

	[SetUp]
	public void Setup() {
		var logger = new Mock<ILogger<IAccountService>>();
		var jwt = new Mock<IJwtService>();

		var data = new List<Account>() {
			new Account() {
				AccountID = 0,
				Email = "examplemail@gmail.com",
				Username = "exampleusername",
				Password = ""
			},
		};

		var accountSet = new Mock<DbSet<Account>>();
		accountSet.Setup(s => s.FindAsync(0)).ReturnsAsync(new Account());
		var roleSet = new Mock<DbSet<Role>>();
		var permissionSet = new Mock<DbSet<Permission>>();

		var database = new Mock<DatabaseContext>();
		database.Setup(db => db.Accounts).Returns(accountSet.Object);
		database.Setup(db => db.Roles).Returns(roleSet.Object);
		database.Setup(db => db.Permissions).Returns(permissionSet.Object);

		Accounts = new AccountService(logger.Object, database.Object, jwt.Object);
	}


}
