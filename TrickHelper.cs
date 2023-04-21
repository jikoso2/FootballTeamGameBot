using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballteamBOT
{
	public static class TrickHelper
	{
		public static string TrickFilePath { get; set; } = string.Empty;

		public static Dictionary<int, List<int>> GetInfoFromFile(string trickName)
		{
			var result = new Dictionary<int, List<int>>();
			if (!File.Exists(TrickFilePath))
			{
				var fileStream = File.Create(TrickFilePath);
				fileStream.Close();
			}

			var file = File.ReadAllLines(TrickFilePath);

			for (int i = 0; i < file.Length; i++)
			{
				if (i == 0 && file[i] != trickName)
					break;
				else if (i == 0)
					continue;

				if (!string.IsNullOrEmpty(file[i]))
					result.Add(i, file[i].Split(',').ToList().Select(a => int.Parse(a)).ToList());
				else
					result.Add(i, new List<int>());
			}
			return result;
		}

		public static void SaveFactors(string trickName, Dictionary<int, List<int>> factors)
		{
			StringBuilder sb = new();
			sb.AppendLine(trickName);

			for (int i = 1; i <= factors.Count; i++)
			{
				sb.AppendLine(string.Join(',', factors[i]));
			}
			File.WriteAllText(TrickFilePath, sb.ToString());
		}

		public static void SaveNewFactors(string trickName, int numbersOfFactor)
		{
			StringBuilder sb = new();
			sb.AppendLine(trickName);

			for (int i = 0; i < numbersOfFactor; i++)
			{
				sb.AppendLine(String.Join(',', Enumerable.Range(1, 10).Select(a => a.ToString())));
			}
			File.WriteAllText(TrickFilePath, sb.ToString());
		}
	}
}
