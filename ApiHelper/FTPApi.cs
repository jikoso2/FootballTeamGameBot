using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;
using RegularExp = System.Text.RegularExpressions;

using TimeZoneConverter;

using static FootballteamBOT.RuntimeProperties;
using static FootballteamBOT.ApiHelper.FTPContracts;
using static FootballteamBOT.ApiHelper.FTPContracts.ItemsResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.MatchesResponse;
using static FootballteamBOT.ApiHelper.FTPContracts.TeamResponse;

namespace FootballteamBOT.ApiHelper
{
	public partial class FTPApi
	{
		public FTPApi(string server, int configurationNumber)
		{
			HttpClient = new HttpClient();
			FTPServer = server;
			TrickHelper.TrickFilePath = Directory.GetCurrentDirectory() + $"\\Configurations\\TrickFiles\\trickFile{configurationNumber}.txt";
		}

		public HttpClient HttpClient { get; set; }
		public string FTPEndpoint { get { return $"https://api.{FTPServer}.footballteamgame.com"; } }
		public string FTPServer { get; set; }

		public object httpClientSemaphore = new();

		public DateTime endBotTraining = DateTime.Now;
		public int dayTricks = 0;
		public int dayFreeStarter = 0;
		public int dayFreeStarterEvent = 0;
		public int dayTeamTraining = 0;
		public int dayCardPack = 0;
		public int hourCalendar = 0;
		public int salaryTeamDay = 0;
		public int sparingTeamDay = 0;
		public int dayTeamRaport = 0;
		public int dayDonateWarehouseGB = 0;
		public long assignedCardFightId = 0;
		public bool inTrickQueue = false;
		public List<long> doneMatches = new();


		public string SendPostReq(string url, object body, bool ignoreFault = false)
		{
			lock (httpClientSemaphore)
			{
				var response = HttpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json")).Result;
				if (response.StatusCode != HttpStatusCode.OK && !ignoreFault)
					throw new WebException(DeserializeJson<ErrorResponse>(response.Content.ReadAsStringAsync().Result).Error);
				return response.Content.ReadAsStringAsync().Result;
			}
		}

		public Tuple<string, HttpStatusCode> SendSpecPostReq(string url, object body)
		{
			lock (httpClientSemaphore)
			{
				var response = HttpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json")).Result;
				return Tuple.Create(response.Content.ReadAsStringAsync().Result, response.StatusCode);
			}
		}

		public string SendGetReq(string url)
		{
			lock (httpClientSemaphore)
			{
				var response = HttpClient.GetAsync(url).Result;
				if (!response.IsSuccessStatusCode)
					throw new WebException(DeserializeJson<ErrorResponse>(response.Content.ReadAsStringAsync().Result).Error);
				return response.Content.ReadAsStringAsync().Result;
			}
		}

		public Tuple<string, HttpStatusCode> SendSpecDeleteReq(string url)
		{
			lock (httpClientSemaphore)
			{
				var response = HttpClient.DeleteAsync(url).Result;
				return Tuple.Create(response.Content.ReadAsStringAsync().Result, response.StatusCode);
			}
		}

		public static T DeserializeJson<T>(string input)
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var result = JsonSerializer.Deserialize<T>(input, options);
			if (result != null)
				return result;
			else
				throw new Exception($"Error while deserializing: {result}");
		}

		public static string NodeParse(Tuple<string, HttpStatusCode> input)
		{
			var result = string.Empty;
			var parsedJson = JsonNode.Parse(input.Item1);

			if (parsedJson == null)
				throw new JsonException($"Parsed from Json Error. Json: {input.Item1}");
			else
			{
				var resultError = parsedJson["error"];
				var resultMessage = parsedJson["message"];
				var resultInfo = parsedJson["info"];

				if (resultError != null)
					result = resultError.ToString();
				if (resultMessage != null)
					result = resultMessage.ToString();
				if (resultInfo != null)
					result = resultInfo.ToString();
			}

			return result;
		}

		public void Login(string email, string password, string fingerPrint)
		{
			HttpClient = new HttpClient();
			AddUnnecessaryHeaders();
			var loginRequest = new { email, password, fingerprint = fingerPrint, locale = "PL" };
			try
			{
				var result = SendPostReq($"{FTPEndpoint}/auth/login", loginRequest);
				var loginResponse = DeserializeJson<LoginResponse>(result);

				HttpClient.DefaultRequestHeaders.Add("x-auth-token", loginResponse.Token);
				HttpClient.DefaultRequestHeaders.Add("x-auth-id", loginResponse.Id.ToString());

				Logger.LogI($"{loginResponse.Info}", "LOGIN");
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), "LOGIN");
			}
		}

		public void AddUnnecessaryHeaders()
		{
			HttpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36");
			HttpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
			HttpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
			HttpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
			HttpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
		}

		public AccountState GetAccountState()
		{
			try
			{
				lock (httpClientSemaphore)
				{
					var response = SendGetReq($"{FTPEndpoint}/user");
					var userGetResponse = DeserializeJson<UserResponse>(response);

					var accountState = new AccountState()
					{
						Name = userGetResponse.User.Name,
						Euro = userGetResponse.User.Euro,
						Credits = userGetResponse.User.Credits,
						FightId = userGetResponse.User.Fight_id,
						Energy = userGetResponse.User.Energy,
						Condition = userGetResponse.User.Condition,
						Defensive = userGetResponse.User.Defensive,
						Pressing = userGetResponse.User.Pressing,
						Freekicks = userGetResponse.User.Freekicks,
						TrainedToday = userGetResponse.User.Trained_Today,
						TeamId = userGetResponse.User.Team_Id,
						Overall = userGetResponse.User.Ovr,
						TimeZone = TZConvert.GetTimeZoneInfo(userGetResponse.User.Timezone)
					};

					var canteenResponse = SendGetReq($"{FTPEndpoint}/canteen");
					var canteenGetResponse = DeserializeJson<CanteenResponse>(canteenResponse);

					accountState.Canteen.Limit = canteenGetResponse.Current_limit;
					accountState.Canteen.MaxLimit = canteenGetResponse.Max_limit;
					accountState.Canteen.Used = canteenGetResponse.Used;
					accountState.Canteen.Queue = canteenGetResponse.Queue;
					accountState.Canteen.CanteenTasks = canteenGetResponse.Limits;

					var packsResponse = SendGetReq($"{FTPEndpoint}/character/packs");
					var packsGetResponse = DeserializeJson<PacksResponse>(packsResponse);

					accountState.Packs.Bronze = packsGetResponse.Packs.Bronze;
					accountState.Packs.Energy = packsGetResponse.Packs.Energetic_locked;
					accountState.Packs.Gold = packsGetResponse.Packs.Gold;
					accountState.Packs.Silver = packsGetResponse.Packs.Silver;
					accountState.Packs.Card = packsGetResponse.Packs.Cards_locked;
					accountState.Packs.FreeKeys = packsGetResponse.Free_keys;
					accountState.Packs.PremiumKeys = packsGetResponse.Keys;
					accountState.Packs.KeyMultiplier = packsGetResponse.Key_multiplier;

					var jobResponse = SendGetReq($"{FTPEndpoint}/jobs");
					var jobGetResponse = DeserializeJson<JobsResponse>(jobResponse);

					accountState.Job.Queue = jobGetResponse.Queue;

					var itemsResponse = SendGetReq($"{FTPEndpoint}/character/cloakroom/items");
					var itemsGetResponse = DeserializeJson<ItemsResponse>(itemsResponse);

					accountState.Item.Items = itemsGetResponse.Items;
					var balls = itemsGetResponse.Items.Where(a => a.Name.Equals("Piłka Sukcesu") || a.Name.Equals("Ball of Success")).ToList();
					accountState.Item.ItemStats = itemsGetResponse.Items_stats;
					accountState.Item.Items = accountState.Item.Items.ToList().Except(balls).ToArray();
					accountState.Item.ItemStats.Poor -= balls.Count;

					var matchesResponse = SendGetReq($"{FTPEndpoint}/games/bets");

					try
					{
						var matchesGetResponse1 = DeserializeJson<MatchesResponse>(matchesResponse);
						accountState.Bet.BetsLeft = matchesGetResponse1.Left_bets;
						accountState.Bet.BetsMax = matchesGetResponse1.Max_bet;
						accountState.Bet.Matches = matchesGetResponse1.Matches.Select(a => a.Value).ToArray();
					}
					catch (JsonException)
					{
						var matchesGetResponse2 = DeserializeJson<MatchesResponse2>(matchesResponse);
						accountState.Bet.BetsLeft = matchesGetResponse2.Left_bets;
						accountState.Bet.BetsMax = matchesGetResponse2.Max_bet;
						accountState.Bet.Matches = matchesGetResponse2.Matches;
					}
					catch (Exception) { }

					var betsStatsResponse = SendGetReq($"{FTPEndpoint}/games/bets/my?page=1");
					try
					{
						var betsStatsGetResponse = DeserializeJson<BetResponse>(betsStatsResponse);
						var totalProfit = betsStatsGetResponse.Bets.Where(a => a.Value.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Value.Status != null && a.Value.Status == "won").Select(a => a.Value.Euro * a.Value.Course - a.Value.Euro).Sum();
						var totalLose = betsStatsGetResponse.Bets.Where(a => a.Value.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Value.Status != null && a.Value.Status == "lost").Select(a => a.Value.Euro).Sum();
						totalProfit -= totalLose;
						var todayPoints = betsStatsGetResponse.Bets.Where(a => a.Value.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Value.Status != null && a.Value.Status == "won").Select(a => a.Value.Course).Sum();

						accountState.Bet.DayPoints = todayPoints;
						accountState.Bet.DayProfit = (long)totalProfit;
					}
					catch (JsonException)
					{
						var betsStatsGetResponse = DeserializeJson<BetResponse2>(betsStatsResponse);
						var totalProfit = betsStatsGetResponse.Bets.Where(a => a.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Status != null && a.Status == "won").Select(a => a.Euro * a.Course - a.Euro).Sum();
						var totalLose = betsStatsGetResponse.Bets.Where(a => a.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Status != null && a.Status == "lost").Select(a => a.Euro).Sum();
						totalProfit -= totalLose;
						var todayPoints = betsStatsGetResponse.Bets.Where(a => a.ParsedDateTime.DayOfYear == DateTime.Today.DayOfYear && a.Status != null && a.Status == "won").Select(a => a.Course).Sum();

						accountState.Bet.DayPoints = todayPoints;
						accountState.Bet.DayProfit = (long)totalProfit;
					}
					catch (Exception) { }

					if (accountState.TeamId != 0)
					{
						var teamStatsResponse = SendGetReq($"{FTPEndpoint}/teams/{accountState.TeamId}");
						var teamStatsGetResponse = DeserializeJson<TeamResponse>(teamStatsResponse);

						accountState.Team.Euro = teamStatsGetResponse.Team.Euro;
						accountState.Team.EuroBuilding = teamStatsGetResponse.Team.Euro_building;
						accountState.Team.Name = teamStatsGetResponse.Team.Name;
						accountState.Team.Ovr = teamStatsGetResponse.Team.Ovr;
						accountState.Team.TrainingHour = teamStatsGetResponse.Team.Training_hour;

						try
						{
							accountState.Team.NextMatch = teamStatsGetResponse.Timetable.Values.SelectMany(a => a).ToList().FirstOrDefault(a => DateTime.Parse(a.Start_date).AddMinutes(35) > DateTime.Now && DateTime.Parse(a.Start_date).AddMinutes(1) < DateTime.Now);
						}
						catch (Exception) { }
					}

					var duelsResponse = SendGetReq($"{FTPEndpoint}/duels");
					var duelsGetResponse = DeserializeJson<DuelsResponse>(duelsResponse);

					accountState.QuickDuels = duelsGetResponse.Quick_Played_Today;
					accountState.RankedDuels = duelsGetResponse.Ranked_Played_Today;
					accountState.DuelsDeck = duelsGetResponse.My_Decks.FirstOrDefault()!;

					var calendarResponse = SendGetReq($"{FTPEndpoint}/calendar");
					var calendarGetResponse = DeserializeJson<CalendarResponse>(calendarResponse);

					accountState.CalendarFinished = calendarGetResponse.Today_challenge.Details == null || calendarGetResponse.Today_challenge.Details.Finished;

					var centerResponse = SendGetReq($"{FTPEndpoint}/training/center");
					var centerGetResponse = DeserializeJson<CenterResponse>(centerResponse);

					accountState.TrainingCenterUsedToday = centerGetResponse.Used_Today;

					return accountState;
				}
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), "ACCOUNT-STATE");
				return new AccountState();
			}
		}

		#region Training
		public bool NormalTraining(long energy)
		{
			var trainings = new List<int[]>();
			for (int i = 0; i < 9; i++)
			{
				var trainingCost = GetTrainingCosts((TrainingType)i);

				if (trainingCost.Item3 < 100)
					trainings.Add(new int[] { 0, i });
			}

			while (energy > 0)
			{
				try
				{
					var finalDuration = 5;
					for (int j = 0; j < 2; j++)
					{
						var skillToTraining = trainings.MinBy(a => a[0]);

						if (skillToTraining == null)
							break;

						var durationAndCost = GetTrainingCosts((TrainingType)skillToTraining[1]);

						finalDuration = durationAndCost.Item1 > finalDuration ? durationAndCost.Item1 : finalDuration;

						var trainingRequest = new { duration = durationAndCost.Item1, skill = Enum.GetName(typeof(TrainingType), skillToTraining[1]) };
						SendPostReq($"{FTPEndpoint}/training/normal", trainingRequest);

						skillToTraining[0] += 1;
						energy -= durationAndCost.Item2;
					}
					Thread.Sleep(finalDuration * 1000 + 500);
				}
				catch (Exception ex)
				{
					Logger.LogE(ex.ToString(), "TRAINING");
					return false;
				}
			}
			return true;
		}

		public bool NormalBotTraining(long energy)
		{
			var opName = "BOT-TRAINING";

			try
			{
				var trainings = new List<int[]>();
				for (int i = 0; i < 9; i++)
				{
					var trainingCost = GetTrainingCosts((TrainingType)i);

					if (trainingCost.Item3 < 99)
						trainings.Add(new int[] { 0, i });
				}

				if (!trainings.Any() && trainings.Count >= 2)
					throw new ArgumentException("Your all skills are above 99+");

				if (energy >= 200 && endBotTraining < DateTime.Now)
				{
					Logger.LogD("Starting BOT normal training", opName);

					var botTrainingReq1 = new { skill = Enum.GetName(typeof(TrainingType), trainings[0][1]), minutes = 5 };
					var trainingResult1 = SendSpecPostReq($"{FTPEndpoint}/training/bot/normal", botTrainingReq1);

					var botTrainingReq2 = new { skill = Enum.GetName(typeof(TrainingType), trainings[1][1]), minutes = 5 };
					var trainingResult2 = SendSpecPostReq($"{FTPEndpoint}/training/bot/normal", botTrainingReq2);

					if (trainingResult1.Item2 != HttpStatusCode.OK && trainingResult2.Item2 != HttpStatusCode.OK)
						return false;

					Logger.LogD($"Start bot:  Details: {NodeParse(trainingResult1)}", opName);
					Logger.LogD($"Start bot:  Details: {NodeParse(trainingResult2)}", opName);

					endBotTraining = DateTime.Now.AddMinutes(5);
				}
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), "BOT-TRAINING");
				return false;
			}

			return true;
		}

		public Tuple<int, int, double> GetTrainingCosts(TrainingType trainingType)
		{
			var infoResponse = SendGetReq($"{FTPEndpoint}/training/normal");
			var infoGetResponse = DeserializeJson<TrainingCostResponse>(infoResponse);
			int duration = 0;
			double level = 100;

			switch (trainingType)
			{
				case TrainingType.condition:
					duration = infoGetResponse.Costs.Condition.Time;
					level = infoGetResponse.Costs.Condition.Actual;
					break;
				case TrainingType.defensive:
					duration = infoGetResponse.Costs.Defensive.Time;
					level = infoGetResponse.Costs.Defensive.Actual;
					break;
				case TrainingType.pressing:
					duration = infoGetResponse.Costs.Pressing.Time;
					level = infoGetResponse.Costs.Pressing.Actual;
					break;
				case TrainingType.efficacy:
					duration = infoGetResponse.Costs.Efficacy.Time;
					level = infoGetResponse.Costs.Efficacy.Actual;
					break;
				case TrainingType.freekicks:
					duration = infoGetResponse.Costs.Freekicks.Time;
					level = infoGetResponse.Costs.Freekicks.Actual;
					break;
				case TrainingType.offensive:
					duration = infoGetResponse.Costs.Offensive.Time;
					level = infoGetResponse.Costs.Offensive.Actual;
					break;
				case TrainingType.playmaking:
					duration = infoGetResponse.Costs.Playmaking.Time;
					level = infoGetResponse.Costs.Playmaking.Actual;
					break;
				case TrainingType.reading:
					duration = infoGetResponse.Costs.Reading.Time;
					level = infoGetResponse.Costs.Reading.Actual;
					break;
				default:
					break;
			}
			return new Tuple<int, int, double>(duration, infoGetResponse.EnergyCost, level);
		}

		public bool TrainingSpecialization(int duration, bool learning, string skill = "defensive", string firstSpec = "first", string secondSpec = "second")
		{
			Logger.LogD($"Starting training specialization: {skill} ({firstSpec},{secondSpec}) ", "TRAINING-SPEC");

			var specResponse = SendGetReq($"{FTPEndpoint}/training/specializations/{skill}");
			var specGetResponse = DeserializeJson<TrainingSpecjalizationResponse>(specResponse);

			for (int i = 0; i < duration; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					var trainingSpecReq = new TrainingSpecjalizationRequest() { Specialization = $"{skill}_{firstSpec}" };
					var trainingResult = SendSpecPostReq($"{FTPEndpoint}/training/specializations/{skill}/train", trainingSpecReq);
					if (trainingResult.Item2 == HttpStatusCode.UnprocessableEntity && learning)
						LearnSpecialize(skill, firstSpec);

					var trainingSpecReq2 = new TrainingSpecjalizationRequest() { Specialization = $"{skill}_{secondSpec}" };
					var trainingResult2 = SendSpecPostReq($"{FTPEndpoint}/training/specializations/{skill}/train", trainingSpecReq2);
					if (trainingResult2.Item2 == HttpStatusCode.UnprocessableEntity && learning)
						LearnSpecialize(skill, secondSpec);
					Thread.Sleep(specGetResponse.Specializations.First(a => a.Sql_key == $"{skill}_{firstSpec}").Time * 1000 + 500);
				}
			}

			return true;
		}

		public bool BotTrainingSpecialization(long userEnergy, bool learning, string skill, string firstSpec, string secondSpec)
		{
			var opName = "BOT-TRAINING-SPEC";
			var specResponse = SendGetReq($"{FTPEndpoint}/training/bot/specialization?minutes={5}&specialization={skill}_{firstSpec}");
			var specGetResponse = DeserializeJson<TrainingBotSpecjalizationResponse>(specResponse);

			if (userEnergy >= long.Parse(specGetResponse.Energy) * 2 && endBotTraining < DateTime.Now)
			{
				Logger.LogD($"Starting BOT training specialization: {skill} ({firstSpec},{secondSpec}) ", "BOT-TRAINING-SPEC");

				var botTrainingSpecReq1 = new TrainingSpecjalizationRequest() { Specialization = $"{skill}_{firstSpec}", Minutes = 5, Approved = 0 };
				var trainingResult1 = SendSpecPostReq($"{FTPEndpoint}/training/bot/specialization", botTrainingSpecReq1);

				var botTrainingSpecReq2 = new TrainingSpecjalizationRequest() { Specialization = $"{skill}_{secondSpec}", Minutes = 5, Approved = 0 };
				var trainingResult2 = SendSpecPostReq($"{FTPEndpoint}/training/bot/specialization", botTrainingSpecReq2);

				if (trainingResult1.Item1.Contains("unlock_price"))
				{
					if (learning)
					{
						botTrainingSpecReq1.Approved = 1;
						trainingResult1 = SendSpecPostReq($"{FTPEndpoint}/training/bot/specialization", botTrainingSpecReq1);
					}
					else
					{
						Logger.LogD($"You have to unlock skill: {skill}_{firstSpec}", opName);
					}
				}

				if (trainingResult2.Item1.Contains("unlock_price"))
				{
					if (learning)
					{
						botTrainingSpecReq2.Approved = 1;
						trainingResult2 = SendSpecPostReq($"{FTPEndpoint}/training/bot/specialization", botTrainingSpecReq2);
					}
					else
					{
						Logger.LogD($"You have to unlock skill: {skill}_{secondSpec}", opName);
					}
				}

				if (trainingResult1.Item2 != HttpStatusCode.OK && trainingResult2.Item2 != HttpStatusCode.OK)
					return false;

				Logger.LogD($"Start bot: {skill}_{firstSpec}, energy cost: {specGetResponse.Energy}. Details: {NodeParse(trainingResult1)}", "TRAINING-BOT-SPEC");
				Logger.LogD($"Start bot: {skill}_{secondSpec}, energy cost: {specGetResponse.Energy}. Details: {NodeParse(trainingResult2)}", "TRAINING-BOT-SPEC");

				endBotTraining = DateTime.Now.AddMinutes(5);
				return true;
			}
			else
				return false;
		}

		public bool LearnSpecialize(string skill = "defensive", string specialize = "first")
		{
			var learnSpecReq = new TrainingSpecjalizationRequest() { Specialization = $"{skill}_{specialize}" };
			var response = SendSpecPostReq($"{FTPEndpoint}/training/specializations/{skill}/unlock", learnSpecReq);
			if (response.Item2 == HttpStatusCode.OK)
				return true;
			else
			{
				Logger.LogE(response.Item1, "LEARN-SPECIALIZE");
				return false;
			}
		}

		#endregion

		public bool EatMeal(int mealToEat)
		{
			var opName = "CANTEEN";
			try
			{
				var cantineeRequest = new CantineeRequest() { Canteen_id = mealToEat };
				var result = SendSpecPostReq($"{FTPEndpoint}/canteen", cantineeRequest);
				Logger.LogD($"Meal: {mealToEat}, {NodeParse(result)}", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool StartJob(int jobId)
		{
			try
			{
				lock (httpClientSemaphore)
				{
					var jobRequest = new { job_id = jobId };
					SendPostReq($"{FTPEndpoint}/jobs", jobRequest);
					Logger.LogD($"Starting job: {jobId}", "JOB");
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), "JOB");
				return false;
			}
		}

		public bool TeamTraining(AccountState accState, string skill, bool notification)
		{
			var opName = "TEAM-TRAINING";
			try
			{
				var teamTrainingRequest = new { skill, double_with_credits = false };
				if (dayTeamTraining != accState.ServerTimeDay())
				{
					if (skill.Contains('_'))
					{
						SendPostReq($"{FTPEndpoint}/teams/{accState.TeamId}/training-specialization", teamTrainingRequest);
					}
					else
					{
						SendPostReq($"{FTPEndpoint}/teams/{accState.TeamId}/training", teamTrainingRequest);
					}

					Logger.LogD($"You signed up for team training, skill: {skill}", opName);
					dayTeamTraining = accState.ServerTimeDay();

					if (notification)
						SendTeamNotification("Zapraszam na trening klubowy, który właśnie się rozpoczyna.", accState.TeamId);

					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				dayTeamTraining = accState.ServerTimeDay();
				return false;
			}
		}

		public bool OpenEnergeticPack(int amount)
		{
			var opName = "PACKS";
			try
			{
				var packRequest = new { type = "energetic_locked", amount };
				var result = SendSpecPostReq($"{FTPEndpoint}/character/packs/event-free", packRequest);

				var parsedJson = JsonNode.Parse(result.Item1);
				var prize = parsedJson?["prize"]?["item"]?["display"];

				Logger.LogD($"Open energetic pack, amount: {amount}, prize: {prize}", opName);

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool OpenPack(string type)
		{
			var opName = "PACKS";
			try
			{
				var packRequest = new { type, amount = 1 };
				var result = SendSpecPostReq($"{FTPEndpoint}/character/packs/free", packRequest);

				var parsedJson = JsonNode.Parse(result.Item1);
				var prize = parsedJson?["prize"]?["item"]?["display"];

				Logger.LogD($"Open {type} pack, prize: {prize}", opName);

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool OpenPremiumPack(string type, int amount = 1)
		{
			var opName = "PREMUM-PACKS";
			try
			{
				var packRequest = new { type, amount };
				var result = SendSpecPostReq($"{FTPEndpoint}/character/packs", packRequest);

				var parsedJson = JsonNode.Parse(result.Item1);
				var prize = parsedJson?["prize"]?["item"]?["display"];

				Logger.LogD($"Open {type} pack, prize: {prize}", opName);

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public Item? SellWeakestItem(Item[] items, bool forBalls = false)
		{
			var itemToSell = items.MinBy(a => a.Bonus);

			if (itemToSell != null && SellItem(new Item[] { itemToSell }, forBalls))
				return itemToSell;
			else
				return null;
		}

		public bool SellItem(Item[] items, bool forBalls)
		{
			var opName = "SELL-ITEM";
			try
			{
				var sellItemRequest = new { items = items.Select(a => a.Id).ToArray() };
				var forBallsAddon = forBalls ? "-golden-balls" : "";
				SendPostReq($"{FTPEndpoint}/character/items/sell{forBallsAddon}", sellItemRequest);
				Logger.LogD($"Selling item: {string.Join(',', items.Select(a => a.Name))}", opName);
				return true;
			}
			catch (NullReferenceException)
			{
				Logger.LogE("You try sell item doesn't exist", opName);
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool ChangePackToKey(string type, int amount = 1)
		{
			var opName = "PACKS";
			try
			{
				var exchangeRequest = new { type, amount };
				SendPostReq($"{FTPEndpoint}/character/packs/exchange", exchangeRequest);
				Logger.LogD($"Exchange {type} packs to keys", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool BetManager(Match[] matches, long maxBetAmount, double minBetCourse, AccountState accState)
		{
			var opName = "BETS";
			var result = false;

			try
			{
				foreach (var match in matches)
				{
					if (accState.Bet.BetsLeft == 0)
						break;

					if (accState.Euro < maxBetAmount * 2)
					{
						Logger.LogW($"You don't have euros to betting (condition: maxBet * 2 < totalEuros)", opName);
						break;
					}

					if (accState.TeamId == match.Guest.Id || accState.TeamId == match.Host.Id)
						continue;

					var totalOVR = match.Guest.Ovr + match.Host.Ovr * 1.05;
					var maxOVR = Math.Max(match.Guest.Ovr, match.Host.Ovr * 1.05);
					var minOVR = Math.Min(match.Guest.Ovr, match.Host.Ovr * 1.05);

					if ((double)maxOVR / totalOVR > 0.6)
					{
						var betCourse = match.Guest.Ovr > match.Host.Ovr ? match.Courses.Guest : match.Courses.Host;

						if (betCourse >= minBetCourse && betCourse < 2 && maxOVR - minOVR > 60)
						{
							var betRequest = new
							{
								match_id = match.Id,
								type = match.Guest.Ovr > match.Host.Ovr ? 2 : 1,
								euro = maxBetAmount
							};

							SendPostReq($"{FTPEndpoint}/games/bets", betRequest);

							Logger.LogD($"Betting {betRequest.type} for {betRequest.euro}, course: {betCourse}, match: {match.Id} ({match.Host.Name} - {match.Guest.Name})", opName);
							accState.Bet.BetsLeft -= 1;
							accState.Euro -= maxBetAmount;
							result |= true;
							Thread.Sleep(4000);
						}
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public EnchantCostResponse GetItemInfo(int itemid)
		{
			var resultItemInfo = DeserializeJson<EnchantCostResponse>(SendGetReq($"{FTPEndpoint}/character/enchanting/{itemid}"));
			return resultItemInfo;
		}

		public bool Enchanting(int itemid, EnchantCostResponse enchantCostResponse)
		{
			var opName = "ENCHANTING";
			try
			{
				Logger.LogI($"Enchanting item level: {enchantCostResponse.Level},bonus: {enchantCostResponse.Bonus}", opName);

				var enchantingRequest = new { special_item_id = 0, use_golden_balls = 0 };
				var resultEnchanting = SendSpecPostReq($"{FTPEndpoint}/character/enchanting/{itemid}", enchantingRequest);

				Logger.LogI($"Cost: {enchantCostResponse.Cost}, result: {resultEnchanting.Item1}", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool TeamTransferEuro(int teamId, long amount)
		{
			var opName = "TRANSFER-EURO";
			try
			{
				var transferRequest = new { amount };
				SendPostReq($"{FTPEndpoint}/teams/{teamId}/accounting/transfer", transferRequest);
				Logger.LogD($"Transfer euros in team {amount.ToString("C", CultureInfo.CurrentCulture)}", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool GetFreeStarter(AccountState accState)
		{
			var opName = "FREE-PACK";
			try
			{
				if (dayFreeStarter != accState.ServerTimeDay())
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/shop/starters-free", new object());
					dayFreeStarter = accState.ServerTimeDay();

					if (result.Item2 == HttpStatusCode.OK)
					{
						var prize = DeserializeJson<StarterFreeResponse>(result.Item1);
						Logger.LogD($"You get free starter daily: {prize.Prize.Item.Display}", opName);
						return true;
					}
					else
					{
						Logger.LogD($"{NodeParse(result)}", opName);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool GetFreeStarterEvent(AccountState accState)
		{
			var opName = "FREE-EVENT-PACK";
			try
			{
				if (dayFreeStarterEvent != accState.ServerTimeDay())
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/shop/event-packs", new object());
					dayFreeStarterEvent = accState.ServerTimeDay();

					if (result.Item2 == HttpStatusCode.OK)
					{
						var prize = DeserializeJson<StarterFreeResponse>(result.Item1);
						Logger.LogD($"You get free starter daily: {prize.Prize.Item.Display}", opName);
						return true;
					}
					else
					{
						Logger.LogD($"{NodeParse(result)}", opName);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool CleanMailBox()
		{
			var result = false;
			var opName = "MAILBOX-CLEANER";
			try
			{
				var mailsResponse = SendGetReq($"{FTPEndpoint}/mailbox?mailbox_type=all&page=1");
				var mailsGetResponse = DeserializeJson<MailboxResponse>(mailsResponse);
				var deletingMails = new List<MailboxResponse.Mail>();

				if (FTPServer == "pl")
					deletingMails = mailsGetResponse.Mailbox.Where(a => a.Title.Contains("Trening asystenta zakończony")).ToList();
				else if (FTPServer == "us" || FTPServer == "en")
					deletingMails = mailsGetResponse.Mailbox.Where(a => a.Title.Contains("Assistant training has finished")).ToList();

				if (deletingMails.Any())
					Logger.LogD($"Start Mailbox Cleaner", opName);

				foreach (var mail in deletingMails)
				{
					var messageResponse = SendSpecDeleteReq($"{FTPEndpoint}/mailbox/{mail.Id}");
					Logger.LogD($"{mail.Id}, Details: {NodeParse(messageResponse)}", opName);
					result |= true;
					Thread.Sleep(1000);
				}

				return result;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool DonateWarehouse(AccountState accState, string type, int amount = 0)
		{
			var opName = "DONATE-TEAM";
			try
			{
				if (type == "golden_balls")
				{
					var donateRequest = new { amount = amount != 0 ? amount : 200, type };
					var result = SendSpecPostReq($"{FTPEndpoint}/teams/{accState.TeamId}/warehouse", donateRequest);
					Logger.LogD($"{NodeParse(result)}", opName);
					return true;
				}
				else if (type == "items")
				{
					var itemToSell = accState.Item.Items.Where(a => a.Rarity == "poor").MinBy(a => a.Bonus);
					if (itemToSell != null)
					{
						var donateRequest = new { ids = new long[] { itemToSell.Id }, type };
						var result = SendSpecPostReq($"{FTPEndpoint}/teams/{accState.TeamId}/warehouse", donateRequest);

						accState.Item.ItemStats.Poor -= 1;
						var list = accState.Item.Items.ToList();
						list.Remove(itemToSell);
						accState.Item.Items = list.ToArray();

						Logger.LogD($"{NodeParse(result)}", opName);
						return true;
					}
				}

				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool DonateTeamWarehouseGB(AccountState accState, int amount)
		{
			var opName = "DONATE-WAREHOUSE-GB";
			try
			{
				if (dayDonateWarehouseGB != accState.ServerTimeDay())
				{
					dayDonateWarehouseGB = accState.ServerTimeDay();

					DonateWarehouse(accState, "golden_balls", amount);

					Logger.LogD($"Create Raport File", opName);

					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public bool MatchBooster(TeamMatch match, TeamProperties teamProperties, int teamId)
		{
			var opName = "MATCH-TEAM";
			try
			{
				if (!doneMatches.Contains(match.Id))
				{
					var matchType = string.Empty;
					var levelBooster = 1;
					var levelEngagement = 1;

					switch (match.Type)
					{
						case "league":
							matchType = "Ligowy";
							levelBooster = teamProperties.LeagueBoosterLevel;
							levelEngagement = teamProperties.LeagueBoosterEngagementLevel;
							break;
						case "sparing":
							matchType = "Sparingowy";
							levelBooster = teamProperties.SparingBoosterLevel;
							levelEngagement = teamProperties.SparingBoosterEngagementLevel;
							break;
						case "country":
							matchType = "Puchar Krajowy";
							levelBooster = teamProperties.CountryBoosterLevel;
							levelEngagement = teamProperties.CountryBoosterEngagementLevel;
							break;
						case "tournament":
							matchType = "Turniejowy";
							levelBooster = teamProperties.TournamentBoosterLevel;
							levelEngagement = teamProperties.TournamentBoosterEngagementLevel;
							break;
						default:
							break;
					}

					var changeGameStyleRequest = new { level = levelBooster, style = teamProperties.BoosterSkill };
					var result1 = SendSpecPostReq($"{FTPEndpoint}/match/{match.Id}/change-game-style", changeGameStyleRequest);

					var engagementRequest = new { level = levelEngagement };
					var result2 = SendSpecPostReq($"{FTPEndpoint}/match/{match.Id}/engagement", engagementRequest);

					Logger.LogD($"{NodeParse(result1)}", opName);
					Logger.LogD($"{NodeParse(result2)}", opName);

					if (teamProperties.MessageNotification)
						SendTeamNotification($"Zapraszam na mecz: {match.Host.Name}[{match.Host.Ovr}] - {match.Guest.Name}[{match.Guest.Ovr}] który właśnie startuje. Rodzaj meczu: {matchType}.", teamId);

					doneMatches.Add(match.Id);

					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool CalendarChecker()
		{
			var opName = "CALENDAR-CHECKER";
			try
			{
				if (hourCalendar != DateTime.Now.Hour)
				{
					var calendarRequest = new { free_finish_with_credits = false };
					var result = SendSpecPostReq($"{FTPEndpoint}/calendar/daily", calendarRequest);
					Logger.LogD($"{NodeParse(result)}", opName);

					hourCalendar = DateTime.Now.Hour;
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}

		}

		public bool TrainingCenter(string skill, int amount)
		{
			var opName = "TRAINING-CENTER";
			try
			{
				var trainingCenterRequest = new { skill, amount };
				var result = SendSpecPostReq($"{FTPEndpoint}/training/center-specialization", trainingCenterRequest);
				Logger.LogD($"{NodeParse(result)}", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}

		}

		public bool GetSalaryFromTeam(AccountState accountState)
		{
			var opName = "TEAM-SALARY";
			try
			{
				if (salaryTeamDay != accountState.ServerTimeDay())
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/teams/{accountState.TeamId}/accounting/salary", new object());
					Logger.LogD($"{NodeParse(result)}", opName);
					salaryTeamDay = accountState.ServerTimeDay();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool SendTeamNotification(string content, int teamId)
		{
			var opName = "TEAM-NOTIFICATION";
			try
			{
				var messageTeamRequest = new { content };
				var result = SendSpecPostReq($"{FTPEndpoint}/teams/{teamId}/control/message", messageTeamRequest);
				Logger.LogD($"{NodeParse(result)}. Details: {content}", opName);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool GetCardPack(AccountState accState)
		{
			var opName = "CARD-PACK";
			try
			{
				if (dayCardPack != accState.ServerTimeDay())
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/canteen/prize", new object());
					Logger.LogD($"{NodeParse(result)}", opName);
					dayCardPack = accState.ServerTimeDay();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool OpenCardPacks(int amount)
		{
			var opName = "OPEN-CARD-PACKS";
			try
			{
				for (int i = 0; i < amount; i++)
				{
					var packRequest = new { type = "cards_locked", amount = 1 };
					var result = SendSpecPostReq($"{FTPEndpoint}/character/cards/open", packRequest);

					var openCardResponse = DeserializeJson<OpenCardResponse>(result.Item1);
					var message = "\n" + string.Join('\n', openCardResponse.Prizes.Select(a => $"Typ: {a.Rarity}, Nazwa: [{a.Ovr}]{a.Name}"));
					Logger.LogD($"Open card pack, prize: {message}", opName);
					Thread.Sleep(2000);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool Augment(long itemId, string itemType)
		{
			var opName = "AUGMENT";
			try
			{
				long? crystalId = 0;

				var augmentResponse = SendGetReq($"{FTPEndpoint}/character/items/special-bonus");
				var augmentGetResponse = DeserializeJson<AugmentResponse>(augmentResponse);

				if (!augmentGetResponse.Crystals.Any())
					return false;

				switch (itemType)
				{
					case "epic":
						crystalId = augmentGetResponse.Crystals.FirstOrDefault(a => a.Type == "epic")?.Id;
						break;
					case "legendary":
						crystalId = augmentGetResponse.Crystals.FirstOrDefault(a => a.Type == "legendary")?.Id;
						break;
					case "platinum":
						crystalId = augmentGetResponse.Crystals.FirstOrDefault(a => a.Type == "platinum")?.Id;
						break;
					case "diamond":
						crystalId = augmentGetResponse.Crystals.FirstOrDefault(a => a.Type == "diamond")?.Id;
						break;
					case "historical":
						crystalId = augmentGetResponse.Crystals.FirstOrDefault(a => a.Type == "historical")?.Id;
						break;
					default:
						return false;
				}

				var augmentRequest = new { crystal_id = crystalId };
				var result = SendSpecPostReq($"{FTPEndpoint}/character/items/special-bonus/{itemId}", augmentRequest);

				var parsedJson = JsonNode.Parse(result.Item1);
				var bonus = parsedJson?["bonus"];

				Logger.LogD($"Augment item id: {itemId}, new bonus: {bonus}%", opName);

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool ExchangeBoosters(long boosterId, int amount)
		{
			var opName = "AUGMENT";
			try
			{

				var exchangeRequest = new { amount, booster_id = boosterId };
				var result = SendSpecPostReq($"{FTPEndpoint}/character/boosters/exchange-energy", exchangeRequest);

				Logger.LogD($"{NodeParse(result)}", opName);

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool AssignToCardDuel(DuelsResponse.DeckModel deckModel)
		{
			var opName = "CARD-DUEL";
			try
			{
				var duelRequest = new { deck_id = deckModel.Id };
				var result = SendSpecPostReq($"{FTPEndpoint}/duels", duelRequest);

				Logger.LogD($"{NodeParse(result)}", opName);

				if (result.Item2 != HttpStatusCode.OK)
					return false;

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool SelectCardToDuel(DuelsResponse.DeckModel deckModel, long fightId)
		{
			var opName = "PLAY-CARD-DUEL";
			try
			{
				if (assignedCardFightId != fightId)
				{
					var duelRequest = new { card_id = deckModel.Cards.FirstOrDefault()!.Id };
					var result = SendSpecPostReq($"{FTPEndpoint}/duels/{fightId}", duelRequest);

					Logger.LogD($"{NodeParse(result)}", opName);

					if (result.Item2 != HttpStatusCode.OK)
						return false;

					assignedCardFightId = fightId;
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool SparingSignUp(AccountState accState)
		{
			var opName = "SPARING-SIGN-UP";
			try
			{
				if (sparingTeamDay != accState.ServerTimeDay())
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/teams/{accState.TeamId}/control/sparing", new object());
					Logger.LogD($"{NodeParse(result)}", opName);
					sparingTeamDay = accState.ServerTimeDay();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return false;
			}
		}

		public bool GenerateTeamStats(AccountState accState)
		{
			var opName = "GENERATE-FILE-STATS";
			try
			{
				if (dayTeamRaport != accState.ServerTimeDay())
				{
					dayTeamRaport = accState.ServerTimeDay();

					var teamStats = SendGetReq($"{FTPEndpoint}/teams/{accState.TeamId}");
					var teamStatsGetResponse = DeserializeJson<TeamResponse>(teamStats);
					string toFile = string.Empty;
					toFile += "Id,Name,Ranking,AverageTraining,AverageCard," +
						"Reading_All,Pressing_All,Playmaking_All,Defensive_All,Condition_All,Efficacy_All,FreeKicks_All,Offensive_All," +
						"Reading_Training,Pressing_Training,Playmaking_Training,Defensive_Training,Condition_Training,Efficacy_Training,FreeKicks_Training,Offensive_Training," +
						"Reading_Card,Pressing_Card,Playmaking_Card,Defensive_Card,Condition_Card,Efficacy_Card,FreeKicks_Card,Offensive_Card" +
						"\n";

					foreach (var id in teamStatsGetResponse.Composition.Players.Select(a => a.Id))
					{
						var user = SendGetReq($"{FTPEndpoint}/profile/{id}");
						var userGetResponse = DeserializeJson<ProfileResponse>(user);

						var stat = string.Join(",",
							userGetResponse.User.Id,
							userGetResponse.User.Name,
							userGetResponse.User.Ranking_Position,
							userGetResponse.User.Skills.AverageTraining,
							userGetResponse.User.Skills.AverageCards,
							userGetResponse.User.Skills.All.Reading,
							userGetResponse.User.Skills.All.Pressing,
							userGetResponse.User.Skills.All.Playmaking,
							userGetResponse.User.Skills.All.Defensive,
							userGetResponse.User.Skills.All.Condition,
							userGetResponse.User.Skills.All.Efficacy,
							userGetResponse.User.Skills.All.Freekicks,
							userGetResponse.User.Skills.All.Offensive,
							userGetResponse.User.Skills.Training.Reading,
							userGetResponse.User.Skills.Training.Pressing,
							userGetResponse.User.Skills.Training.Playmaking,
							userGetResponse.User.Skills.Training.Defensive,
							userGetResponse.User.Skills.Training.Condition,
							userGetResponse.User.Skills.Training.Efficacy,
							userGetResponse.User.Skills.Training.Freekicks,
							userGetResponse.User.Skills.Training.Offensive,
							userGetResponse.User.Skills.Cards.Reading,
							userGetResponse.User.Skills.Cards.Pressing,
							userGetResponse.User.Skills.Cards.Playmaking,
							userGetResponse.User.Skills.Cards.Defensive,
							userGetResponse.User.Skills.Cards.Condition,
							userGetResponse.User.Skills.Cards.Efficacy,
							userGetResponse.User.Skills.Cards.Freekicks,
							userGetResponse.User.Skills.Cards.Offensive
							);
						toFile += stat + "\n";
					}
					var fileDirectory = Directory.GetCurrentDirectory() + $"\\Configurations\\Stats\\{DateTime.Now.ToString("d-M-yyyy-H-m")}.txt";

					File.WriteAllText(fileDirectory, toFile);

					Logger.LogD($"Create Raport File: {fileDirectory}", opName);

					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.Message, opName);
				return false;
			}
		}

		public void GetSellViewCards()
		{
			var opName = "SELL-CARDS";
			try
			{
				var response = SendGetReq($"{FTPEndpoint}/character/cards/sell");
				var characterGetSellViewResponse = DeserializeJson<SellCardsResponse>(response);

				var cardsToSell = characterGetSellViewResponse.Duplicates.Where(a => a.Amount > 2 && (a.Rarity.Equals("poor") || a.Rarity.Equals("upgraded") || a.Rarity.Equals("rare"))).ToDictionary(a => a.card_id, v => v.Amount - 2);

				var idsToSell = new List<long>();

				foreach (var cardToSell in cardsToSell)
				{
					var lista = characterGetSellViewResponse.Cards.Where(a => a.Card_Id == cardToSell.Key);
					var ids = lista.OrderBy(a => a.Ovr).Take(cardToSell.Value).Select(a => a.Id);
					idsToSell.AddRange(ids);
				}

				foreach (var partIdsToSell in idsToSell.Chunk(50))
				{
					var result = SendSpecPostReq($"{FTPEndpoint}/character/cards/sell", new { cards = partIdsToSell });
					Logger.LogD($"{NodeParse(result)}", opName);
				}

			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
			}
		}

		public (int, int, int) GetPricePack()
		{
			var opName = "PACK-PRICE";
			try
			{
				var response = SendGetReq($"{FTPEndpoint}/auctions/packs?page=1&type=");
				var packPriceViewResponse = DeserializeJson<PackPriceResponse>(response);

				var bronzePrice = packPriceViewResponse.Auctions.OrderBy(a => a.price).First(a => a.type == "bronze").price;
				var silverPrice = packPriceViewResponse.Auctions.OrderBy(a => a.price).First(a => a.type == "silver").price;
				var goldenPrice = packPriceViewResponse.Auctions.OrderBy(a => a.price).First(a => a.type == "gold").price;

				Logger.LogI($"PACK-PRIZE: [{bronzePrice}] - Bronze, [{silverPrice}] - Silver, [{goldenPrice}] - Golden,  ", "PACK-INFO");

				return (bronzePrice, silverPrice, goldenPrice);
			}
			catch (Exception ex)
			{
				Logger.LogE(ex.ToString(), opName);
				return (0, 0, 0);
			}
		}

		[RegularExp.GeneratedRegex("\\d+")]
		private static partial RegularExp.Regex MessageRegex();
	}
}
