using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace BlazorApp
{

	public enum Elements
	{
		ice,
		lightning,
		fire,
		poison,
		holy,
	}
	public enum Weapons : int
	{
		OneHandedSword = 0,
		TwoHandedSword,
		OneHandedAxe,
		TwoHandedAxe,
		Spear,
		Polearm,
		OneHandedBW,
		TwoHandedBW,
		Dagger,
		Bow,
		Crossbow,
		Knuckle,
		Gun,
		Claw,
		Staff,
		Wand,
		MAX_COUNT
	};
	public struct WeaponData
	{
		public string name;
		public float lowStatMulti;
		public float highStatMulti;
		public string imageSource;

		public WeaponData(string inName, float inLowStatMulti, float inHighStatMulti, string inImageSource)
		{
			name = inName;
			lowStatMulti = inLowStatMulti;
			highStatMulti = inHighStatMulti;
			imageSource = inImageSource;
		}
	};

	public struct CharStats
	{
		public int str;
		public int dex;
		public int intel;
		public int luk;
		public int watt;
		public float mastery;
	}
	public struct MonsterAccuracyData
	{
		public float acc1;
		public float acc100;
		public float accRatio;


	}
	public struct DamageRange
	{
		public double min;
		public double max;
	};
	public class Calculation
	{
		public static WeaponData[] weaponData = new WeaponData[] {
			new WeaponData("One Handed Sword", 4.0f, 4.4f, "Pictures/weapons/one_sword.png"),
			new WeaponData("Two Handed Sword", 4.6f, 5.0f, "Pictures/weapons/two_sword.png"),
			new WeaponData("One Handed Axe", 3.6f, 5.0f, "Pictures/weapons/one_axe.png"),
			new WeaponData("Two Handed Axe", 4.0f, 5.2f, "Pictures/weapons/two_axe.png"),
			new WeaponData("Spear", 3.8f, 5.1f, "Pictures/weapons/spear.png"),
			new WeaponData("Polearm", 3.8f, 5.2f, "Pictures/weapons/polearm.png"),
			new WeaponData("One Handed Blunt Weapon", 3.6f, 5.0f, "Pictures/weapons/one_blunt.png"),
			new WeaponData("Two Handed Blunt Weapon", 4.0f, 5.2f, "Pictures/weapons/two_blunt.png"),
			new WeaponData("Dagger", 3.6f, 4.2f, "Pictures/weapons/dagger.png"),
			new WeaponData("Bow", 3.4f, 3.4f, "Pictures/weapons/bow.png"),
			new WeaponData("Crossbow", 3.6f, 3.6f, "Pictures/weapons/xbow.png"),
			new WeaponData("Knuckle", 4.8f, 4.8f, "Pictures/weapons/knuckle.png"),
			new WeaponData("Gun", 3.6f, 3.6f, "Pictures/weapons/gun.png"),
			new WeaponData("Claw", 3.6f, 4.2f, "Pictures/weapons/claw.png"),
			new WeaponData("Staff", 4.4f, 4.4f, "Pictures/weapons/staff.png"),
			new WeaponData("Wand", 4.4f, 4.4f, "Pictures/weapons/wand.png")
		};


		public static Dictionary<string, MonsterAccuracyData> accuracyData = new Dictionary<string, MonsterAccuracyData>();
		public static DamageRange GetDamageRange(CharStats stats, Weapons weapon)
		{
			DamageRange ret = new DamageRange();
			ret.min = 1;
			ret.max = 1;
			switch (weapon)
			{
				case Weapons.Gun:
				case Weapons.Bow:
				case Weapons.Crossbow:
					ret.max = (stats.dex * weaponData[(int)weapon].highStatMulti + stats.str) * stats.watt / 100;
					ret.min = (stats.dex * weaponData[(int)weapon].lowStatMulti * 0.9 * stats.mastery + stats.str) * stats.watt / 100;
					break;
				case Weapons.Staff:
				case Weapons.Wand:
				case Weapons.Spear:
				case Weapons.Polearm:
				case Weapons.Knuckle:
				case Weapons.OneHandedSword:
				case Weapons.OneHandedBW:
				case Weapons.OneHandedAxe:
				case Weapons.TwoHandedSword:
				case Weapons.TwoHandedBW:
				case Weapons.TwoHandedAxe:
					ret.max = (stats.str * weaponData[(int)weapon].highStatMulti + stats.dex) * stats.watt / 100;
					ret.min = (stats.str * weaponData[(int)weapon].lowStatMulti * 0.9 * stats.mastery + stats.dex) * stats.watt / 100;
					break;
				case Weapons.Dagger:
				case Weapons.Claw:
					ret.max = (stats.luk * weaponData[(int)weapon].highStatMulti + stats.str + stats.dex) * stats.watt / 100;
					ret.min = (stats.luk * weaponData[(int)weapon].lowStatMulti * 0.9 * stats.mastery + stats.str + stats.dex) * stats.watt / 100;
					break;
			}

			ret.max = Math.Floor(ret.max);
			ret.min = Math.Floor(ret.min);

			return ret;
		}
		public static DamageRange GetMagicDamageRange(double magic, double intel, double spellAttack, float mastery) {
			DamageRange ret;

			//double magic = intel + addMagic;
			double magicSquared = magic * magic / 1000;
			//avg = ((magicSquared + (magic + magic * mastery * 0.9) / 2.0) / 30.0 + intel / 200.0) * spellAttack;
			ret.max = ((magicSquared + magic) / 30.0 + intel / 200.0) * spellAttack;
			ret.min = ((magicSquared + (magic * mastery * 0.9)) / 30.0 + intel / 200.0) * spellAttack;
			ret.max = Math.Floor(ret.max);
			ret.min = Math.Floor(ret.min);

			return ret;
		}

								//monsterName,    hits, probability
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
