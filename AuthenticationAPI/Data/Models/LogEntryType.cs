using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data.Models;

[Index("Name", IsUnique = true)]
public class LogEntryType {
	[Key]
	[Required]
	public int LogEntryTypeID { get; set; }

	[Required]
	[Column(TypeName = "varchar(50)")]
	public string Name { get; set; }

	[Required]
	[Column(TypeName = "text")]
	public string Description { get; set; }

	public ICollection<LogEntry> LogEntries { get; set; }
}