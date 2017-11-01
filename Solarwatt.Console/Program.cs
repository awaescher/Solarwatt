using Solarwatt.Api;
using Solarwatt.Api.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solarwatt.Api.Dto;

namespace Solarwatt.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var connection = new DirtyHardcodedTestConnection();

			System.Console.WriteLine("Press \"P\" to add proxy credentials or any other key to proceed.");
			if (System.Console.ReadKey().Key == ConsoleKey.P)
			{
				System.Console.Write($"Enter your username	(Press [Return] for \"{Environment.UserName}\"): ");
				connection.ProxyUser = ReadConsoleLineWithFallback(Environment.UserName);
				System.Console.Write($"Enter your domain	(Press [Return] for \"{Environment.UserDomainName}\"): ");
				connection.ProxyUserDomain = ReadConsoleLineWithFallback(Environment.UserDomainName);
				System.Console.Write("Enter your password: ");
				connection.ProxyPassword = ReadPassword();
			}

			var connector = new SolarwattConnector(connection);
			connector.Login();

			var export = new List<ExportRow>();

			const int DAYS = 7;
			
			// Passed days
			System.Console.WriteLine($"Overview: Last {DAYS} days");

			var yesterday = DateTime.Today.AddDays(-1);
			var daysFromYesterday = DAYS - 1;
			export.AddRange(connector.GetExport(yesterday.AddDays(-1 * (daysFromYesterday)), yesterday, 60));
			
			// Today (finer interval because this is still happening)
			export.AddRange(connector.GetExport(DateTime.Today));

			foreach (var exportRow in export)
			{
				System.Console.WriteLine(exportRow.ToString());
			}

			System.Console.WriteLine();
			System.Console.WriteLine("Press any key to quit.");
			System.Console.ReadKey();
		}

		private static string ReadConsoleLineWithFallback(string fallback)
		{
			string input = System.Console.ReadLine();
			return string.IsNullOrEmpty(input) ? fallback : input;
		}

		public static string ReadPassword()
		{
			string password = "";
			ConsoleKeyInfo info = System.Console.ReadKey(true);
			while (info.Key != ConsoleKey.Enter)
			{
				if (info.Key != ConsoleKey.Backspace)
				{
					System.Console.Write("*");
					password += info.KeyChar;
				}
				else if (info.Key == ConsoleKey.Backspace)
				{
					if (!string.IsNullOrEmpty(password))
					{
						// remove one character from the list of password characters
						password = password.Substring(0, password.Length - 1);
						// get the location of the cursor
						int pos = System.Console.CursorLeft;
						// move the cursor to the left by one character
						System.Console.SetCursorPosition(pos - 1, System.Console.CursorTop);
						// replace it with space
						System.Console.Write(" ");
						// move the cursor to the left by one character again
						System.Console.SetCursorPosition(pos - 1, System.Console.CursorTop);
					}
				}
				info = System.Console.ReadKey(true);
			}
			// add a new line because user pressed enter at the end of their password
			System.Console.WriteLine();
			return password;
		}
	}
}
