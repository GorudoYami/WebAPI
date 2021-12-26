using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Services {
	public enum ResultType {
		Ok,
		UsernameTaken,
		EmailTaken,
		NotFound,
		InvalidPassword,
		Exception
	}

	public class ServiceResult<T> {
		public T Value { get; set; }
		public ResultType Type { get; set; }

		public ServiceResult(ResultType result) {
			Type = result;
		}
	}
}
