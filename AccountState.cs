﻿using static FootballteamBOT.ApiHelper.FTPContracts.CanteenResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.DuelsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.ItemsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.JobsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.MatchesResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.TeamResponse;

namespace FootballteamBOT
{
	public class AccountState
	{
		public string Name { get; set; } = string.Empty;
		public long Euro { get; set; }
		public long Credits { get; set; }
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
		public int QuickDuels { get; set; }
		public int RankedDuels { get; set; }
		public bool CalendarFinished { get; set; }
		public DeckModel DuelsDeck { get; set; } = new DeckModel();
		public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
		public CanteenState Canteen { get; set; } = new CanteenState();
		public PacksState Packs { get; set; } = new PacksState();
		public JobState Job { get; set; } = new JobState();
		public ItemState Item { get; set; } = new ItemState();
		public BetState Bet { get; set; } = new BetState();
		public TeamState Team { get; set; } = new TeamState();
		public int ServerTimeDay() => Program.GetNowServerDataTime(TimeZone).Day;
		public int ServerTimeHour() => Program.GetNowServerDataTime(TimeZone).Hour;
		public int ServerTimeMinute() => Program.GetNowServerDataTime(TimeZone).Minute;

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
			public int Card { get; set; }
			public int CardGolden { get; set; }
		}

		public class JobState
		{
			public Jobs_Queue Queue { get; set; } = new();
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

