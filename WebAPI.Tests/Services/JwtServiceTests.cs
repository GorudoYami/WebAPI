﻿using System;
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
	private JwtService Jwt { get; set; }

	[SetUp]
	public void Setup()
	{
		var logger = new Mock<ILogger<IJwtService>>();
		Jwt = new JwtService(logger.Object, null);
	}

	[Test]
	public void 
}