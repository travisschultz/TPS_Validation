using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPS_Validation
{
	public sealed class Globals
	{
		private string log = "";
		private double photonTolerance = 2;
		private double electronTolerance = 2;
		private static readonly Lazy<Globals> lazy = new Lazy<Globals>(() => new Globals());

		public static Globals Instance { get { return lazy.Value; } }

		private Globals()
		{
		}

		public void CreateLogEntry(String entry)
		{
			log += entry + '\n';
		}

		public string GetLog()
		{
			return log;
		}

		public void ClearLog()
		{
			log = "";
		}

		public double GetPhotonTolerance()
		{
			return photonTolerance;
		}

		public void SetPhotonTolerance(double tol)
		{
			photonTolerance = tol;
		}

		public double GetElectronTolerance()
		{
			return electronTolerance;
		}

		public void SetElectronTolerance(double tol)
		{
			electronTolerance = tol;
		}
	}
}
