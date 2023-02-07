using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data.Models;

public class LogEntry {
	[Key]
	[Required]
	public int LogEntryID { get; set; }

	[Required]
	public int LogEntryTypeID { get; set; }
	[Required]
	public LogEntryType LogEntryType { get; set; }

	[Required]
	public DateTime Timestamp { get; set; }

	[Required]
	[Column(TypeName = "varchar(100)")]
	public string Location { get; set; }

	[Required]
	[Column(TypeName = "text")]
	public string Message { get; set; }
}
