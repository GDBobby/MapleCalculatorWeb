

namespace BlazorApp
{
	public enum ClassID : int
	{
		Warrior,

		Fighter,
		Crusader,
		Hero,

		Page,
		WhiteKnight,
		Paladin,

		Spearman,
		DragonKnight,
		DarkKnight,

		Archer,
		Hunter,
		Ranger,
		Bowmaster,

		Crossbowman,
		Sniper,
		Marksman,

		Thief,
		Bandit,
		ChiefBandit,
		Shadower,

		Assassin,
		Hermit,
		NightLord,

		Pirate,
		Brawler,
		Marauder,
		Bucaneer,

		Gunslinger,
		Outlaw,
		Corsair,

		Magician,
		FirePoisonWizard,
		FirePoisonMage,
		FirePoisonArchMage,

		IceLightningWizard,
		IceLightningMage,
		IceLightningArchMage,

		Cleric,
		Priest,
		Bishop
	};
	public class JobData
	{


		public struct Classes
		{
			public string Name;
			public Weapons[] WeaponIDs;

			public Classes(string name, Weapons[] weaponIDs)
			{
				Name = name;
				WeaponIDs = weaponIDs;
			}
		};
		public static int[] firstJobs = new int[6]
		{
			(int)ClassID.Warrior, (int)ClassID.Archer, (int)ClassID.Thief, (int)ClassID.Pirate, (int)ClassID.Magician, (int)ClassID.Bishop + 1
		};

		public static Classes[] classData = new Classes[] {
		new Classes("Warrior",
			new Weapons[] {Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedAxe,
			Weapons.TwoHandedAxe,
			Weapons.Spear,
			Weapons.Polearm,
			Weapons.OneHandedBW,
			Weapons.TwoHandedBW}
		),

		new Classes("Fighter", new Weapons[] {Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedAxe,
			Weapons.TwoHandedAxe }
		),
		new Classes("Crusader", new Weapons[] {Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedAxe,
			Weapons.TwoHandedAxe }
		),
		new Classes("Hero", new Weapons[] {Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedAxe,
			Weapons.TwoHandedAxe }
		),

		new Classes("Page", new Weapons[]{Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedBW,
			Weapons.TwoHandedBW }
		),
		new Classes("White Knight", new Weapons[] {Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedBW,
			Weapons.TwoHandedBW }
		),
		new Classes("Paladin", [Weapons.OneHandedSword,
			Weapons.TwoHandedSword,
			Weapons.OneHandedBW,
			Weapons.TwoHandedBW ]
		),

		new Classes("Spearman", new Weapons[] {Weapons.Spear, Weapons.Polearm} ),
		new Classes("Dragon Knight", new Weapons[] {Weapons.Spear, Weapons.Polearm} ),
		new Classes("Dark Knight", new Weapons[] {Weapons.Spear, Weapons.Polearm} ),

		new Classes("Archer", new Weapons[]{Weapons.Bow, Weapons.Crossbow} ),
		new Classes("Hunter", new Weapons[]{Weapons.Bow}),
		new Classes("Ranger", new Weapons[]{Weapons.Bow}),
		new Classes("Bowmaster", new Weapons[]{Weapons.Bow}),

		new Classes("Crossbowman", new Weapons[]{Weapons.Crossbow}),
		new Classes("Sniper", new Weapons[]{Weapons.Crossbow}),
		new Classes("Marksman", new Weapons[]{Weapons.Crossbow}),

		new Classes("Thief", new Weapons[]{Weapons.Dagger, Weapons.Claw} ),
		new Classes("Bandit", new Weapons[]{Weapons.Dagger}),
		new Classes("Chief Bandit", new Weapons[]{Weapons.Dagger}),
		new Classes("Shadower", new Weapons[]{Weapons.Dagger}),

		new Classes("Assassin", new Weapons[]{Weapons.Claw}),
		new Classes("Hermit", new Weapons[]{Weapons.Claw}),
		new Classes("Night Lord", new Weapons[]{Weapons.Claw}),

		new Classes("Pirate", new Weapons[]{Weapons.Knuckle, Weapons.Gun}),
		new Classes("Brawler", new Weapons[]{Weapons.Knuckle}),
		new Classes("Marauder", new Weapons[]{Weapons.Knuckle}),
		new Classes("Bucaner", new Weapons[]{Weapons.Knuckle}),

		new Classes("Gunslinger", new Weapons[]{Weapons.Gun } ),
		new Classes("Outlaw", new Weapons[]{Weapons.Gun}),
		new Classes("Corsair", new Weapons[]{Weapons.Gun}),

		new Classes("Magician", new Weapons[]{Weapons.Staff, Weapons.Wand })
	};
	}
}
