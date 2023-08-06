using static FootballteamBOT.ApiHelper.FTPContracts.CanteenResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.ItemsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.JobsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.MatchesResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.TeamResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.TricksResponse;

namespace FootballteamBOT
{
	public class AccountState
	{
		public string Name { get; set; } = string.Empty;
		public long Euro { get; set; }
		public long Energy { get; set; }
		public double Defensive { get; set; }
		public double Pressing { get; set; }
		public double Condition { get; set; }
		public double Freekicks { get; set; }
		public int Overall { get; set; }
		public int TrainedToday { get; set; }
		public int TeamId { get; set; }
		public int TrainingCenterUsedToday { get; set; }
		public long FightId { get; set; }
		public bool CalendarFinished { get; set; }
		public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
		public CanteenState Canteen { get; set; } = new CanteenState();
		public PacksState Packs { get; set; } = new PacksState();
		public JobState Job { get; set; } = new JobState();
		public TrickState Trick { get; set; } = new TrickState();
		public ItemState Item { get; set; } = new ItemState();
		public BetState Bet { get; set; } = new BetState();
		public TeamState Team { get; set; } = new TeamState();

		public class CanteenState
		{
			public int Limit { get; set; }
			public int MaxLimit { get; set; }
			public int Used { get; set; }
			public CanteenQueue Queue { get; set; } = new();
			public CanteenLimit[] CanteenTasks { get; set; } = Array.Empty<CanteenLimit>();
		}

		public class PacksState
		{
			public int PremiumKeys { get; set; }
			public int FreeKeys { get; set; }
			public int Bronze { get; set; }
			public int Silver { get; set; }
			public int Gold { get; set; }
			public int Energy { get; set; }
			public int KeyMultiplier { get; set; }
		}

		public class JobState
		{
			public Jobs_Queue Queue { get; set; } = new();
		}

		public class TrickState
		{
			public Trick_Queue Queue { get; set; } = new();
			public Trick[] Tricks { get; set; } = Array.Empty<Trick>();
		}
	}

	public class ItemState
	{
		public Items_Stats ItemStats { get; set; } = new();
		public Item[] Items { get; set; } = Array.Empty<Item>();
	}

	public class BetState
	{
		public int BetsLeft { get; set; }
		public long BetsMax { get; set; }
		public double DayPoints { get; set; }
		public long DayProfit { get; set; }
		public Match[] Matches { get; set; } = Array.Empty<Match>();
	}

	public class TeamState
	{
		public string Name { get; set; } = string.Empty;
		public int Ovr { get; set; }
		public long Euro { get; set; }
		public int TrainingHour { get; set; }
		public long EuroBuilding { get; set; }
		public TeamMatch? NextMatch { get; set; }
		public bool ContractSalary { get; set; }
	}
}

