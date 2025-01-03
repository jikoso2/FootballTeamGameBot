﻿using static FootballteamBOT.ApiHelper.FTPContracts.BetResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.MatchesResponse;
using System.Text.Json.Serialization;
using static FootballteamBOT.ApiHelper.FTPContracts.MatchesResponse.Match;

namespace FootballteamBOT.ApiHelper
{
	public class FTPContracts
	{
		public class ErrorResponse
		{
			public string? Error { get; set; }
		}

		public class LoginResponse
		{
			public string? Info { get; set; }
			public string? Token { get; set; }
			public int Id { get; set; }
		}

		public class TrainingCostResponse
		{
			public Cost Costs { get; set; } = new();
			public int EnergyCost { get; set; }
			public class Cost
			{
				public DetailCost Condition { get; set; } = new();
				public DetailCost Defensive { get; set; } = new();
				public DetailCost Efficacy { get; set; } = new();
				public DetailCost Freekicks { get; set; } = new();
				public DetailCost Offensive { get; set; } = new();
				public DetailCost Playmaking { get; set; } = new();
				public DetailCost Pressing { get; set; } = new();
				public DetailCost Reading { get; set; } = new();

				public class DetailCost
				{
					public int Time { get; set; }
					public double Actual { get; set; }
				}
			}
		}

		public class TrainingSpecjalizationRequest
		{
			[JsonPropertyName("specialization")]
			public string Specialization { get; set; } = string.Empty;
			/// <summary>
			/// ToBotTraining
			/// </summary>
			[JsonPropertyName("minutes")]
			public int Minutes { get; set; }
			/// <summary>
			/// ToBotTraining
			/// </summary>
			[JsonPropertyName("approved")]
			public int Approved { get; set; }
		}

		public class TrainingSpecjalizationResponse
		{
			public Specialization[] Specializations { get; set; } = Array.Empty<Specialization>();

			public class Specialization
			{
				public int Time { get; set; }
				public string Sql_key { get; set; } = string.Empty;
			}
		}

		public class TrainingBotSpecjalizationResponse
		{
			public string Energy { get; set; } = string.Empty;
		}

		public enum TrainingType
		{
			playmaking,
			condition,
			defensive,
			pressing,
			efficacy,
			freekicks,
			offensive,
			reading
		}

		public class UserResponse
		{
			public UserEntity? User { get; set; }

			public class UserEntity
			{
				public string Name { get; set; } = string.Empty;
				public long Euro { get; set; }
				public long Credits { get; set; }
				public long Energy { get; set; }
				public long Fight_id { get; set; }
				public double Defensive { get; set; }
				public double Pressing { get; set; }
				public double Condition { get; set; }
				public double Freekicks { get; set; }
				public int Team_Id { get; set; }
				public int Trained_Today { get; set; }
				public int Ovr { get; set; }
				public string Timezone { get; set; } = string.Empty;
			}
		}

		public class CanteenResponse
		{
			public CanteenQueue Queue { get; set; } = new();
			public int Used { get; set; }
			public int Current_limit { get; set; }
			public int Max_limit { get; set; }
			public CanteenLimit[] Limits { get; set; } = Array.Empty<CanteenLimit>();

			public class CanteenQueue
			{
				public int Canteen_id { get; set; }
				public long End { get; set; }
				public int Energy { get; set; }
				public long Id { get; set; }
				public long Start { get; set; }
				public long User_id { get; set; }

			}

			public class CanteenLimit
			{
				public string Key { get; set; } = string.Empty;
				public int Canteens { get; set; }
				public bool Finished { get; set; }
				public string Title { get; set; } = string.Empty;
			}
		}

		public class CantineeRequest
		{
			[JsonPropertyName("canteen_id")]
			public int Canteen_id { get; set; }
		}

		public class PacksResponse
		{
			public PacksEntity Packs { get; set; } = new();
			public int Keys { get; set; }
			public int Free_keys { get; set; }
			public int Key_multiplier { get; set; }
			public class PacksEntity
			{
				public PackEntity Bronze { get; set; } = new();
				public PackEntity Energetic_locked { get; set; } = new();
				public PackEntity Gold { get; set; } = new();
				public PackEntity Silver { get; set; } = new();
				public PackEntity Cards_locked { get; set; } = new();
				public PackEntity Cards_new_locked { get; set; } = new();
			}

			public class PackEntity
			{
				public int Amount { get; set; }
			}
		}

		public class JobsResponse
		{
			public Jobs_Queue Queue { get; set; } = new();
			public class Jobs_Queue
			{
				public int Job_id { get; set; }
				public long End { get; set; }
				public long Id { get; set; }
				public long Start { get; set; }
				public long User_id { get; set; }
				public long Income { get; set; }
			}
		}

		public class ItemsResponse
		{
			public Items_Stats Items_stats { get; set; } = new();
			public Item[] Items { get; set; } = Array.Empty<Item>();
			public class Items_Stats
			{
				public int Poor { get; set; }
				public int Rare { get; set; }
				public int Upgraded { get; set; }
			}
			public class Item
			{
				public double Bonus { get; set; }
				public long Id { get; set; }
				public string Name { get; set; } = string.Empty;
				public string Rarity { get; set; } = string.Empty;
				public string Type { get; set; } = string.Empty;
			}
		}

		public class MatchesResponse
		{
			public Dictionary<string, Match> Matches { get; set; } = new Dictionary<string, Match>();
			public long Max_bet { get; set; }
			public int Left_bets { get; set; }

			public class Match
			{
				public long Id { get; set; }
				public Team Guest { get; set; } = new();
				public Team Host { get; set; } = new();
				public Courses Courses { get; set; } = new();

				public class Team
				{
					public long Id { get; set; }
					public int Ovr { get; set; }
					public string Name { get; set; } = string.Empty;
				}
			}
			public class Courses
			{
				public double Draw { get; set; }
				public double Guest { get; set; }
				public double Host { get; set; }
			}
		}

		public class MatchesResponse2
		{
			public Match[] Matches { get; set; } = Array.Empty<Match>();
			public long Max_bet { get; set; }
			public int Left_bets { get; set; }

		}

		[JsonDerivedType(typeof(PossibleMatchBetsResponseList), 2)]
		[JsonDerivedType(typeof(PossibleMatchBetsResponseDict), 1)]
		[JsonDerivedType(typeof(PossibleMatchBetsResponseBase), 0)]
		public class PossibleMatchBetsResponseBase
		{
			public PossibleMatchBetsResponseBase()
			{
			}

			public long Max_bet { get; set; }
			public int Left_bets { get; set; }
		}

		public class PossibleMatchBetsResponseDict : PossibleMatchBetsResponseBase, IPossibleMatchBetsResponse
		{
			public IDictionary<int, Match>? Matches { get; set; }

			public IEnumerable<Match>? Items => Matches?.Values;
		}

		public class PossibleMatchBetsResponseList : PossibleMatchBetsResponseBase, IPossibleMatchBetsResponse
		{
			public IEnumerable<Match>? Matches { get; set; }

			public IEnumerable<Match>? Items => Matches;
		}

		public interface IPossibleMatchBetsResponse
		{
			public long Max_bet { get; set; }
			public int Left_bets { get; set; }
			public IEnumerable<Match> Items { get; }
		}

		public class BetResponse
		{
			public Dictionary<string, Bet> Bets { get; set; } = new();
			public class Bet
			{
				public DateTime ParsedDateTime { get { return DateTime.Parse(Date); } private set { } }
				public string Date { get; set; } = string.Empty;
				public long Euro { get; set; }
				public string Status { get; set; } = string.Empty;
				public double Course { get; set; }
			}
		}

		public class BetResponse2
		{
			public Bet[] Bets { get; set; } = Array.Empty<Bet>();
		}

		public class MailboxResponse
		{
			public IEnumerable<Mail> Mailbox { get; set; } = Array.Empty<Mail>();

			public class Mail
			{
				public int Id { get; set; }

				public string Title { get; set; } = string.Empty;

				public DateTime Last_reply_date { get; set; }

			}
		}

		public class MailResponse
		{
			public MessageEntity Message { get; set; } = new();

			public class MessageEntity
			{
				public string Title { get; set; } = string.Empty;

				public DetailedMail[] Replies { get; set; } = Array.Empty<DetailedMail>();

				public class DetailedMail
				{
					public string Content { get; set; } = string.Empty;
				}
			}
		}

		public class TeamResponse
		{
			public TeamEntity Team { get; set; } = new();

			public Dictionary<string, TeamMatch[]?>? Timetable { get; set; }

			public CompositionModel Composition { get; set; } = new();

			public class TeamEntity
			{
				public long Euro { get; set; }
				public long Euro_building { get; set; }
				public string Name { get; set; } = string.Empty;
				public int Ovr { get; set; }
				public int Training_hour { get; set; }

			}

			public class TeamMatch
			{
				public long Id { get; set; }
				public string Type { get; set; } = string.Empty;
				public string Start_date { get; set; } = string.Empty;
				public Team Guest { get; set; } = new();
				public Team Host { get; set; } = new();
			}

			public class CompositionModel
			{
				public PlayerCompositionModel[] Players { get; set; } = Array.Empty<PlayerCompositionModel>();
			}

			public class PlayerCompositionModel
			{
				public long Id { get; set; }
			}
		}

		public class TeamResponse2
		{
			public TeamEntity Team { get; set; } = new();

			public string[]? Timetable { get; set; }

			public CompositionModel Composition { get; set; } = new();

			public class TeamEntity
			{
				public long Euro { get; set; }
				public long Euro_building { get; set; }
				public string Name { get; set; } = string.Empty;
				public int Ovr { get; set; }
				public int Training_hour { get; set; }

			}

			public class CompositionModel
			{
				public PlayerCompositionModel[] Players { get; set; } = Array.Empty<PlayerCompositionModel>();
			}

			public class PlayerCompositionModel
			{
				public long Id { get; set; }
			}
		}
		public class StarterFreeResponse
		{
			public PrizeEntity Prize { get; set; } = new();

			public class PrizeEntity
			{
				public ItemInner Item { get; set; } = new();

				public class ItemInner
				{
					public string Display { get; set; } = string.Empty;
				}
			}
		}

		public class EnchantCostResponse
		{
			public double Bonus { get; set; }
			public int Cost { get; set; }
			public int Level { get; set; }
		}

		public class CalendarResponse
		{
			public bool Calendar_disabled { get; set; }
			public bool Show_info { get; set; }
			public Challenge Today_challenge { get; set; } = new();

			public class Challenge
			{
				public int Day_of_season { get; set; }
				public DetailsEntity Details { get; set; } = new();

				public class DetailsEntity
				{
					public bool Finished { get; set; }
				}
			}
		}

		public class TricksFightsResponse
		{
			public int Global_limit { get; set; }
			public int Limit { get; set; }
		}

		public class CenterResponse
		{
			public int Used_Today { get; set; } = -1;
		}

		public class AugmentResponse
		{
			public Crystal[] Crystals { get; set; } = Array.Empty<Crystal>();

			public class Crystal
			{
				public long Id { get; set; }
				public long Amount { get; set; }
				public string Type { get; set; } = string.Empty;

			}
		}

		public class OpenCardResponse
		{
			public Prize[] Prizes { get; set; } = Array.Empty<Prize>();

			public class Prize
			{
				public string Name { get; set; } = string.Empty;
				public string Rarity { get; set; } = string.Empty;
				public int Ovr { get; set; }
			}
		}

		public class ProfileResponse
		{
			public Profile? User { get; set; }

			public class Profile
			{
				public long Id { get; set; }
				public string Name { get; set; }
				public double Ranking_Position { get; set; }
				public SkillsModel Skills { get; set; }
			}

			public class SkillsModel
			{
				public TrainingModel Training { get; set; }
				public TrainingModel All { get; set; }
				public TrainingModel Cards { get; set; }
				public double AverageTraining
				{
					get
					{
						var sum = Training.Playmaking + Training.Condition + Training.Defensive + Training.Efficacy + Training.Freekicks + Training.Offensive + Training.Pressing + Training.Reading;
						return Double.Round(sum / 8.0, 3);
					}
				}
				public double AverageCards
				{
					get
					{
						var sum = Cards.Playmaking + Cards.Condition + Cards.Defensive + Cards.Efficacy + Cards.Freekicks + Cards.Offensive + Cards.Pressing + Cards.Reading;
						return Double.Round(sum / 8.0, 3);
					}
				}
			}

			public class TrainingModel
			{
				public double Condition { get; set; }
				public double Defensive { get; set; }
				public double Efficacy { get; set; }
				public double Freekicks { get; set; }
				public double Offensive { get; set; }
				public double Playmaking { get; set; }
				public double Pressing { get; set; }
				public double Reading { get; set; }
			}
		}

		public class SellCardsResponse
		{
			public CardsModel[] Cards { get; set; } = Array.Empty<CardsModel>();
			public DuplicateModel[] Duplicates { get; set; } = Array.Empty<DuplicateModel>();

			public class CardsModel
			{
				public long Card_Id { get; set; }
				public long Ovr { get; set; }
				public long Id { get; set; }
				public string Rarity { get; set; }
				public string Name { get; set; }
			}

			public class DuplicateModel
			{
				public int Amount { get; set; }
				public long card_id { get; set; }
				public string Rarity { get; set; }
			}
		}

		public class PackPriceResponse
		{
			public AuctionModel[] Auctions { get; set; } = Array.Empty<AuctionModel>();

			public class AuctionModel
			{
				public int price { get; set; }
				public int quantity { get; set; }
				public string type { get; set; } = string.Empty;
			}
		}

		public class DuelsResponse
		{
			public int Ranked_Played_Today { get; set; }
			public int Quick_Played_Today { get; set; }
			public DeckModel[] My_Decks { get; set; } = Array.Empty<DeckModel>();

			public class DeckModel
			{
				public int Id { get; set; }
				public string Name { get; set; } = string.Empty;
				public CardModel[] Cards { get; set; } = Array.Empty<CardModel>();

				public class CardModel
				{
					public long Id { get; set; }
					public string Name { get; set; } = string.Empty;
				}
			}
		}

		public class DrinksResponse
		{
			public DrinkItem[] Items { get; set; }

			public class DrinkItem
			{
				public int Id { get; set; }
				public int Tier { get; set; }
				public int Amount { get; set; }
				public string Drink { get; set; } = string.Empty;

			}
		}

		public class TeamTrainingResponse
		{
			public int Signed_Members { get; set; }

			public Dictionary<string, ActiveBonusesItem> Active_Bonuses { get; set; } = new Dictionary<string, ActiveBonusesItem>();

			public class ActiveBonusesItem
			{
				public int value_display { get; set; }
			}
		}

		public class UnfinishedTasksResponse
		{
			public Categorie Categories { get; set; }

			public class Categorie
			{
				public Daily daily { get; set; }
				public Weekly weekly { get; set; }
				public Season season { get; set; }

				[JsonPropertyName("one-time")]
				public OneTime one_time { get; set; }

				public class Daily
				{
					public int cards { get; set; }
					public int city { get; set; }
					public int duels { get; set; }
					public int others { get; set; }
					public int training { get; set; }
				}

				public class Weekly
				{
					public int locker_room { get; set; }
					public int team { get; set; }
					public int trader { get; set; }
				}

				public class Season
				{
					public int matches { get; set; }
					public int others { get; set; }
					public int training { get; set; }
				}

				public class OneTime
				{
					public int games { get; set; }
					public int skills { get; set; }
					public int team { get; set; }
					public int others { get; set; }
					public int training { get; set; }
				}
			}
		}

		public class TasksResponse
		{
			public List<Taskk> Tasks { get; set; }

			public class Taskk
			{
				public bool can_finish { get; set; }
				public string description { get; set; }
				public string key { get; set; }
				public int level { get; set; }
				public int max_level { get; set; }
			}
		}
	}
}