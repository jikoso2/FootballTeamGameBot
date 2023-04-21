using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballteamBOT
{
	public static class Logger
	{
		public static void LogI(string message, string context)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));

		}

		public static void LogE(string message, string context)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));
		}

		public static void LogW(string message, string context)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));
		}

		public static void LogD(string message, string context)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));
		}

		public static void LogConfiguration(string message, string context)
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));
		}

		public static void LogRuntimesProps(string message,string context)
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine(String.Format("{0,-10} \t {1,-20} {2,-10}", DateTime.Now, context, message));
		}
	}
}
