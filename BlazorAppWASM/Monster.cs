using System;
using System.IO;
using System.Text.Json;
using System.Reflection;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;




namespace BlazorApp
{
	public enum ElementalType
	{
		Ice,
		Lightning,
		Fire,
		Poison,
		Holy,
	}
	public enum ElementalAffinity
	{
		neutral,
		strong,
		immune,
		weak
	};
	public class MonsterStats
	{
		public int id { get; set; }
		public int level { get; set; }
		public int hp { get; set; }
		public int avoid { get; set; }
		public int exp { get; set; }
		public int[] ele { get; set; } = Array.Empty<int>();

		public string GetEleString(int i)
		{
			string output;
			switch ((ElementalType)i)
			{
				case ElementalType.Ice:
					output = "Ice: ";
					break;
				case ElementalType.Lightning:
					output = "Lightning: ";
						break;
				case ElementalType.Fire:
					output = "Fire: ";
					break;
				case ElementalType.Poison:
					output = "Lightning: ";
					break;
				case ElementalType.Holy:
					output = "Holy: ";
					break;
				default:
					output = "Error Type: ";
					return output;
			}
			if(ele == null)
			{
				return "elemental list is null";
			}
			if (ele[0] == -1)
			{
				return "elemental list wasn't set correctly";
			}
				
			if(i >= 5)
			{
				return "what element requested";
			}
			if (ele[i] == (int)ElementalAffinity.weak)
			{
				return output + "weak";
			}
			else if (ele[i] == (int)ElementalAffinity.neutral)
			{
				return output + "neutral";
			}
			else if (ele[i] == (int)ElementalAffinity.strong)
			{
				return output + "strong";
			}
			else if (ele[i] == (int)ElementalAffinity.immune)
			{
				return output + "immune";
			}
			return output + "elemental error";
		}
	};
	public class Monster
	{
		public static string[] regions = 
		{
			"Victoria",
			"Bosses",
			"Ludi",
			"Orbis",
			"Zipangu",
		};

		public static List<string> searchResults = new List<string>();

		//each MonsterList represents a region named by the string key
		public static Dictionary<string, MonsterStats> monsterData = new Dictionary<string, MonsterStats>();


		public static Dictionary<int, List<string>> dropTable = new Dictionary<int, List<string>>();
		public static string[] items = Array.Empty<string>();

		public static void InitMonsterData(string jsonString)
		{

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			Dictionary<string, MonsterStats>? ret = JsonSerializer.Deserialize<Dictionary<string, MonsterStats>>(jsonString, options);
			if (ret != null)
			{
				monsterData = ret;
			}

		}
		public static void InitMonsterDrops(string dropContent)
		{

			//var lines = dropContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			var lines = dropContent.Split('\n');
			int currentKey = 0;
			int previousKey = -1;
			List<string> itemData = new List<string>();

			foreach (var line in lines)
			{
				
				if (line.Contains(":"))
				{
					//new item
					if (!int.TryParse(line.Split(':')[1], out currentKey))
					{
						currentKey = -1;
					}
					if (itemData.Count > 0)
					{
						dropTable.Add(previousKey, itemData);
					}
					itemData = new List<string>();
					previousKey = currentKey;
				}
				else
				{
					// Add value to current key
					itemData.Add(line.Trim());
				}
			}
		}

		public static void InitItems(string itemContent)
		{
			items = itemContent.Split('\n');
		}

		public static void Search(string searchItem, bool searchDrops)
		{
			foreach (var monster in monsterData)
			{
				if (searchDrops)
				{
					if (dropTable.TryGetValue(monster.Value.id, out List<string> drop_table))
					{
						if (drop_table != null)
						{

							bool foundSearchInItems = false;
							foreach (var drop in drop_table)
							{
								if (drop.Contains(searchItem, StringComparison.OrdinalIgnoreCase))
								{
									foundSearchInItems = true;
									if (!searchResults.Contains(monster.Key))
									{
										searchResults.Add(monster.Key);
									}
									break;
								}
							}
							if (foundSearchInItems)
							{
								continue;
							}
						}
					}
				}
				if (monster.Key.Contains(searchItem, StringComparison.OrdinalIgnoreCase))
				{
					searchResults.Add(monster.Key);
				}
			}

			searchResults = searchResults.Distinct().ToList();
		}


		public static string GetMonsterIDString(string idString)
		{
			//@Monster.currentMonster.Value.id.ToString()
			return "Pictures/monsters/" + idString + ".png";
		}
		public static Dictionary<string, List<double>> expectedHitsToKill = new Dictionary<string, List<double>>();
		private static double Erf(double x)
		{
			// Constants for approximation
			const double a1 = 0.254829592;
			const double a2 = -0.284496736;
			const double a3 = 1.421413741;
			const double a4 = -1.453152027;
			const double a5 = 1.061405429;
			const double p = 0.3275911;

			// Save the sign of x
			int sign = x < 0 ? -1 : 1;
			x = Math.Abs(x);

			// Compute the approximation
			double t = 1.0 / (1.0 + p * x);
			double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

			return sign * y;
		}
		private static double Standard_Normal_CDF(double z)
		{
			return 0.5 * (1.0 + Erf(z / Math.Sqrt(2.0)));
		}

		public static void GetExpectedHitsToKill(string monsterName, int levelDiff, int minDmg, int maxDmg, int targetHP, int monsterDefense, float skillPercentage = 100.0f, int skillLines = 1)
		{
			if (levelDiff < 0)
			{
				levelDiff = 0;
			}

			float fMinDmg = (minDmg * skillPercentage / 100.0f) * (1.0f - 0.01f * levelDiff) - monsterDefense * 0.6f;
			float fMaxDmg = (maxDmg * skillPercentage / 100.0f) * (1.0f - 0.01f * levelDiff) - monsterDefense * 0.5f;
			if (fMaxDmg < 0.0 || fMinDmg < 0.0)
			{
				return;
			}
			minDmg = (int)Math.Floor(fMinDmg);
			maxDmg = (int)Math.Floor(fMaxDmg);

			float avgDmg = (maxDmg + minDmg) / 2.0f;

			int maxHitsToKill = (int)Math.Ceiling((double)targetHP / (double)minDmg);
			int minHitsToKill = (int)Math.Ceiling((double)targetHP / (double)maxDmg);
			double expectedHits = (double)targetHP / avgDmg;

			List<double> probabilities = new List<double>();
			for (int i = 0; i < minHitsToKill; i++)
			{
				probabilities.Add(0.0);
			}
			for (int i = minHitsToKill; i <= maxHitsToKill; i++)
			{
				double meanDmg = (double)(avgDmg * i);
				double hitVariance = (double)i * Math.Pow(maxDmg - minDmg, 2.0) / 12;
				double hitStdDev = Math.Sqrt(hitVariance);
				double backVal = probabilities.Last();
				probabilities.Add(1.0 - Standard_Normal_CDF(((double)targetHP - meanDmg) / hitStdDev));
			}
			for (int i = maxHitsToKill; i >= minHitsToKill; i--)
			{
				probabilities[i] = probabilities[i] - probabilities[i - 1];
			}

			//ref List<double> mainTable = hpTables[hpTables.Count - 1];

			//filtering the hit count into skill hits
			if (skillLines > 1)
			{
				for (int i = 0; i < (probabilities.Count / skillLines); i++)
				{
					probabilities[i] = probabilities[i * skillLines];
					for (int j = 1; j < skillLines; j++)
					{
						probabilities[i] += probabilities[i * skillLines + j];
					}
				}
				probabilities.RemoveRange(probabilities.Count / skillLines + 1, probabilities.Count - (probabilities.Count / skillLines + 1));
			}
			expectedHitsToKill.Add(monsterName, probabilities);
		}
	}
}
