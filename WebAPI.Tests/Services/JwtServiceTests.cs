using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebAPI.Services.Interfaces;
using WebAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebAPI.Tests.Services;

[TestFixture]
public class JwtServiceTests
{
	private ILogger<IJwtService> Logger;

	[SetUp]
	public void Setup()
	{
		Logger = new Mock<ILogger<IJwtService>>().Object;
	}


}
