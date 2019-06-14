using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPS_Validation
{
	public sealed class ValidationLog
	{
		private String data = "";
		private static readonly Lazy<ValidationLog> lazy = new Lazy<ValidationLog>(() => new ValidationLog());

		public static ValidationLog Instance { get { return lazy.Value; } }

		private ValidationLog()
		{
		}

		public void CreateEntry(String entry)
		{
			data += entry + '\n';
		}

		public String GetLog()
		{
			return data;
		}

		public void ClearLog()
		{
			data = "";
		}
	}
}
