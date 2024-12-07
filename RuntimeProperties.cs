using System.Runtime.CompilerServices;

namespace FootballteamBOT
{
	public class RuntimeProperties
	{
		public RuntimeProperties()
		{
			Training = new TrainingProperties() { NotifyChange = true };
			Cantinee = new CantineeProperties() { NotifyChange = true };
			Team = new TeamProperties() { NotifyChange = true };
			Startup = new StartupProperties() { NotifyChange = true };
		}

		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			if (NotifyChange)
				Logger.LogRuntimesProps($"{propertyName} : {field} => {value}", "RUNTIME-PROPERTIES");
			field = value;
			return true;
		}

		#region Global
		public bool NotifyChange { get; set; }

		private string email = string.Empty;
		public string Email { get => email; set => SetField(ref email, value); }

		public string Password { get; set; } = string.Empty;

		private string fingerPrint = string.Empty;
		public string FingerPrint { get => fingerPrint; set => SetField(ref fingerPrint, value); }

		private string server = string.Empty;
		public string Server { get => server; set => SetField(ref server, value); }

		private int intervalRefresh;
		public int IntervalRefresh { get => intervalRefresh; set => SetField(ref intervalRefresh, value); }

		#endregion


		#region Training
		private int trainingLimit;
		public int TrainingLimit { get => trainingLimit; set => SetField(ref trainingLimit, value); }

		private bool trainingCenterAfterLimit;
		public bool TrainingCenterAfterLimit { get => trainingCenterAfterLimit; set => SetField(ref trainingCenterAfterLimit, value); }

		private int trainingCenterAmount;
		public int TrainingCenterAmount { get => trainingCenterAmount; set => SetField(ref trainingCenterAmount, value); }

		private string trainingCenterSkill = string.Empty;
		public string TrainingCenterSkill { get => trainingCenterSkill; set => SetField(ref trainingCenterSkill, value); }

		public TrainingProperties Training { get; set; }
		#endregion


		#region CardDuels

		private int rankedDuels;
		public int RankedDuels { get => rankedDuels; set => SetField(ref rankedDuels, value); }

		private int quickDuels;
		public int QuickDuels { get => quickDuels; set => SetField(ref quickDuels, value); }

		#endregion


		#region Bets
		private bool betManager;
		public bool BetManager { get => betManager; set => SetField(ref betManager, value); }

		private double betMinCourse;
		public double BetMinCourse { get => betMinCourse; set => SetField(ref betMinCourse, value); }

		private int betValue;
		public int BetValue { get => betValue; set => SetField(ref betValue, value); }
		#endregion


		#region Team

		public TeamProperties Team { get; set; }

		#endregion

		#region Startup

		public StartupProperties Startup { get; set; }

		#endregion


		#region OtherProps
		private bool cleanMailBox;
		public bool CleanMailBox { get => cleanMailBox; set => SetField(ref cleanMailBox, value); }

		private bool eatFood;
		public bool EatFood { get => eatFood; set => SetField(ref eatFood, value); }

		private bool autoGetCardPack;
		public bool AutoGetCardPack { get => autoGetCardPack; set => SetField(ref autoGetCardPack, value); }

		private bool autoOpenCardPacks;
		public bool AutoOpenCardPacks { get => autoOpenCardPacks; set => SetField(ref autoOpenCardPacks, value); }

		private long targetEuro;
		public long TargetEuro { get => targetEuro; set => SetField(ref targetEuro, value); }

		private int jobType;
		public int JobType { get => jobType; set => SetField(ref jobType, value); }

		private bool autoAchivementRewards;
		public bool AutoAchivementRewards { get => autoAchivementRewards; set => SetField(ref autoAchivementRewards, value); }
		#endregion


		#region Cantinee
		public CantineeProperties Cantinee { get; set; }
		#endregion

		public class TeamProperties
		{
			private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				if (NotifyChange)
					Logger.LogRuntimesProps($"TeamProperties.{propertyName} : {field} => {value}", "RUNTIME-PROPERTIES");
				field = value;
				return true;
			}

			public bool NotifyChange { get; set; }

			private bool training;
			public bool Training { get => training; set => SetField(ref training, value); }

			private string trainingSkill = string.Empty;
			public string TrainingSkill { get => trainingSkill; set => SetField(ref trainingSkill, value); }

			private bool trainingDrinkBooster;
			public bool TrainingDrinkBooster { get => trainingDrinkBooster; set => SetField(ref trainingDrinkBooster, value); }

			private int trainingDrinkBoosterMinFreq;
			public int TrainingDrinkBoosterMinFreq { get => trainingDrinkBoosterMinFreq; set => SetField(ref trainingDrinkBoosterMinFreq, value); }

			private bool euroAutoTransfer;
			public bool EuroAutoTransfer { get => euroAutoTransfer; set => SetField(ref euroAutoTransfer, value); }

			private bool matchBooster;
			public bool MatchBooster { get => matchBooster; set => SetField(ref matchBooster, value); }

			private string boosterSkill = string.Empty;
			public string BoosterSkill { get => boosterSkill; set => SetField(ref boosterSkill, value); }

			private bool salary;
			public bool Salary { get => salary; set => SetField(ref salary, value); }

			private bool autoSparingSignUp;
			public bool AutoSparingSignUp { get => autoSparingSignUp; set => SetField(ref autoSparingSignUp, value); }

			private bool messageNotification;
			public bool MessageNotification { get => messageNotification; set => SetField(ref messageNotification, value); }

			private int countryBoosterLevel;
			public int CountryBoosterLevel { get => countryBoosterLevel; set => SetField(ref countryBoosterLevel, value); }

			private int countryBoosterEngagementLevel;
			public int CountryBoosterEngagementLevel { get => countryBoosterEngagementLevel; set => SetField(ref countryBoosterEngagementLevel, value); }

			private int leagueBoosterLevel;
			public int LeagueBoosterLevel { get => leagueBoosterLevel; set => SetField(ref leagueBoosterLevel, value); }

			private int leagueBoosterEngagementLevel;
			public int LeagueBoosterEngagementLevel { get => leagueBoosterEngagementLevel; set => SetField(ref leagueBoosterEngagementLevel, value); }

			private int tournamentBoosterLevel;
			public int TournamentBoosterLevel { get => tournamentBoosterLevel; set => SetField(ref tournamentBoosterLevel, value); }

			private int tournamentBoosterEngagementLevel;
			public int TournamentBoosterEngagementLevel { get => tournamentBoosterEngagementLevel; set => SetField(ref tournamentBoosterEngagementLevel, value); }

			private int sparingBoosterLevel;
			public int SparingBoosterLevel { get => sparingBoosterLevel; set => SetField(ref sparingBoosterLevel, value); }

			private int sparingBoosterEngagementLevel;
			public int SparingBoosterEngagementLevel { get => sparingBoosterEngagementLevel; set => SetField(ref sparingBoosterEngagementLevel, value); }

		}

		public class TrainingProperties
		{
			private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				if (NotifyChange)
					Logger.LogRuntimesProps($"TrainingProperties.{propertyName} : {field} => {value}", "STARTUP-PROPERTIES");
				field = value;
				return true;
			}

			public bool NotifyChange { get; set; }

			private string skill = String.Empty;
			public string Skill { get => skill; set => SetField(ref skill, value); }

			private bool specialize;
			public bool Specialize { get => specialize; set => SetField(ref specialize, value); }

			private bool learning;
			public bool Learning { get => learning; set => SetField(ref learning, value); }

			private string training1 = String.Empty;
			public string Training1 { get => training1; set => SetField(ref training1, value); }

			private string training2 = String.Empty;
			public string Training2 { get => training2; set => SetField(ref training2, value); }

			private bool useBot;
			public bool UseBot { get => useBot; set => SetField(ref useBot, value); }

		}

		public class CantineeProperties
		{
			private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				if (NotifyChange)
					Logger.LogRuntimesProps($"CanteenProperties.{propertyName} : {field} => {value}", "CANTEEN-PROPERTIES");
				field = value;
				return true;
			}

			public bool NotifyChange { get; set; }

			private bool resolver;
			public bool Resolver { get => resolver; set => SetField(ref resolver, value); }

			private bool calendarChecker;
			public bool CalendarChecker { get => calendarChecker; set => SetField(ref calendarChecker, value); }

			private bool jobs;
			public bool Jobs { get => jobs; set => SetField(ref jobs, value); }

			private bool goldenBallsWarehouse;
			public bool GoldenBallsWarehouse { get => goldenBallsWarehouse; set => SetField(ref goldenBallsWarehouse, value); }

			private int amountGoldenBallsWarehouse;
			public int AmountGoldenBallsWarehouse { get => amountGoldenBallsWarehouse; set => SetField(ref amountGoldenBallsWarehouse, value); }

			private bool sellingItems;
			public bool SellingItems { get => sellingItems; set => SetField(ref sellingItems, value); }

			private bool donateItemWarehouse;
			public bool DonateItemWarehouse { get => donateItemWarehouse; set => SetField(ref donateItemWarehouse, value); }

			private bool augment;
			public bool Augment { get => augment; set => SetField(ref augment, value); }

			private long augmentItemId;
			public long AugmentItemId { get => augmentItemId; set => SetField(ref augmentItemId, value); }

			private string augmentItemType = string.Empty;
			public string AugmentItemType { get => augmentItemType; set => SetField(ref augmentItemType, value); }

			private bool exchangeBoosters;
			public bool ExchangeBoosters { get => exchangeBoosters; set => SetField(ref exchangeBoosters, value); }

			private long boosterId;
			public long BoosterId { get => boosterId; set => SetField(ref boosterId, value); }
		}

		public class StartupProperties
		{
			private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				if (NotifyChange)
					Logger.LogRuntimesProps($"StartupProperties.{propertyName} : {field} => {value}", "STARTUP-PROPERTIES");
				field = value;
				return true;
			}

			public bool NotifyChange { get; set; }

			private bool useStartupProcedure;
			public bool UseStartupProcedure { get => useStartupProcedure; set => SetField(ref useStartupProcedure, value); }


			private bool enchantItem;
			public bool EnchantItem { get => enchantItem; set => SetField(ref enchantItem, value); }

			private int enchantItemId;
			public int EnchantItemId { get => enchantItemId; set => SetField(ref enchantItemId, value); }

			private int enchantLevel;
			public int EnchantLevel { get => enchantLevel; set => SetField(ref enchantLevel, value); }

			private int enchantAttempts;
			public int EnchantAttempts { get => enchantAttempts; set => SetField(ref enchantAttempts, value); }


			private bool augmentItem;
			public bool AugmentItem { get => augmentItem; set => SetField(ref augmentItem, value); }

			private int augmentItemId;
			public int AugmentItemId { get => augmentItemId; set => SetField(ref augmentItemId, value); }

			private int augmentAttempts;
			public int AugmentAttempts { get => augmentAttempts; set => SetField(ref augmentAttempts, value); }

			private string augmentItemType = String.Empty;
			public string AugmentItemType { get => augmentItemType; set => SetField(ref augmentItemType, value); }


			private bool openEnergyPacks;
			public bool OpenEnergyPacks { get => openEnergyPacks; set => SetField(ref openEnergyPacks, value); }

			private int openEnergyPackAmount;
			public int OpenEnergyPackAmount { get => openEnergyPackAmount; set => SetField(ref openEnergyPackAmount, value); }
		}
	}
}