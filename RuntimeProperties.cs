using System.Runtime.CompilerServices;

namespace FootballteamBOT
{
	public class RuntimeProperties
	{
		public RuntimeProperties() { Training = new TrainingProperties() { NotifyChange = true }; Cantinee = new CantineeProperties() { NotifyChange = true }; }

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

		#endregion


		#region Training
		private int trainingLimit;
		public int TrainingLimit { get => trainingLimit; set => SetField(ref trainingLimit, value); }

		public TrainingProperties Training { get; set; }
		#endregion


		#region Trick
		private bool trickLearn;
		public bool TrickLearn { get => trickLearn; set => SetField(ref trickLearn, value); }

		private string trick = string.Empty;
		public string Trick { get => trick; set => SetField(ref trick, value); }
		#endregion


		#region Bets
		private bool betManager;
		public bool BetManager { get => betManager; set => SetField(ref betManager, value); }

		private double betMinCourse;
		public double BetMinCourse { get => betMinCourse; set => SetField(ref betMinCourse, value); }

		private int betValue;
		public int BetValue { get => betValue; set => SetField(ref betValue, value); }
		#endregion


		#region Club
		private bool clubTraining;
		public bool ClubTraining { get => clubTraining; set => SetField(ref clubTraining, value); }

		private string clubTrainingSkill = string.Empty;
		public string ClubTrainingSkill { get => clubTrainingSkill; set => SetField(ref clubTrainingSkill, value); }

		private bool clubEuroAutoTransfer;
		public bool ClubEuroAutoTransfer { get => clubEuroAutoTransfer; set => SetField(ref clubEuroAutoTransfer, value); }

		private bool clubMatchBooster;
		public bool ClubMatchBooster { get => clubMatchBooster; set => SetField(ref clubMatchBooster, value); }

		private string clubBoosterSkill = string.Empty;
		public string ClubBoosterSkill { get => clubBoosterSkill; set => SetField(ref clubBoosterSkill, value); }

		private int clubBoosterLevel;
		public int ClubBoosterLevel { get => clubBoosterLevel; set => SetField(ref clubBoosterLevel, value); }

		private int clubBoosterEngagementLevel;
		public int ClubBoosterEngagementLevel { get => clubBoosterEngagementLevel; set => SetField(ref clubBoosterEngagementLevel, value); }
		#endregion


		#region Starters
		private bool getFreeStarter;
		public bool GetFreeStarter { get => getFreeStarter; set => SetField(ref getFreeStarter, value); }

		private bool getFreeStarterEvent;
		public bool GetFreeStarterEvent { get => getFreeStarterEvent; set => SetField(ref getFreeStarterEvent, value); }
		#endregion


		#region OtherProps
		private bool cleanMailBox;
		public bool CleanMailBox { get => cleanMailBox; set => SetField(ref cleanMailBox, value); }

		private bool eatFood;
		public bool EatFood { get => eatFood; set => SetField(ref eatFood, value); }
		#endregion


		#region Cantinee
		public CantineeProperties Cantinee { get; set; }
		#endregion

		public class TrainingProperties
		{
			private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
			{
				if (EqualityComparer<T>.Default.Equals(field, value)) return false;
				if (NotifyChange)
					Logger.LogRuntimesProps($"TrainingProperties.{propertyName} : {field} => {value}", "RUNTIME-PROPERTIES");
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

			private bool sellingItems;
			public bool SellingItems { get => sellingItems; set => SetField(ref sellingItems, value); }
		}
	}
}