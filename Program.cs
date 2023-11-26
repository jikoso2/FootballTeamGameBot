using FootballteamBOT.ApiHelper;
using FootballteamBOT;
using System.Globalization;
using System.Text.Json;

StartupProcedure();
ReadRuntimeProperties(true);

var FtpApi = new FTPApi(RuntimeProps.Server, Configuration);
FtpApi.Login(RuntimeProps.Email, RuntimeProps.Password, RuntimeProps.FingerPrint);


//FtpApi.GenerateTeamStats(FtpApi.GetAccountState());

//for (int i = 0; i < 40; i++)
//{
//	FtpApi.OpenPack("bronze");
//	Thread.Sleep(1000);
//}
//for (int i = 0; i < 13; i++)
//{
//	int itemidd = 12473595;
//	FtpApi.Augment(itemidd, "epic");
//}

//for (int i = 0; i < 35; i++)
//{
//	int itemid = 4847682;
//	int enchantLevel = 13;

//	var itemInfo = FtpApi.GetItemInfo(itemid);

//	if (itemInfo.Level == enchantLevel + 1)
//		break;

//	FtpApi.Enchanting(itemid, itemInfo);
//	Thread.Sleep(1500);
//}


while (true)
{
	SomethingDoneInLoop = false;
	ReadRuntimeProperties();
	var accountState = FtpApi.GetAccountState();

	if (accountState.Overall == 0)
	{
		Thread.Sleep(50000);
		FtpApi.Login(RuntimeProps.Email, RuntimeProps.Password, RuntimeProps.FingerPrint);
		continue;
	}

	LogAccountState(accountState);

	if (RuntimeProps.Cantinee.Resolver)
		CantineeTasksResolver(accountState);

	if (RuntimeProps.GetFreeStarter)
		SomethingDoneInLoop |= FtpApi.GetFreeStarter(accountState);

	if (RuntimeProps.GetFreeStarterEvent)
		SomethingDoneInLoop |= FtpApi.GetFreeStarterEvent(accountState);

	if (RuntimeProps.CleanMailBox)
		SomethingDoneInLoop |= FtpApi.CleanMailBox();

	if (RuntimeProps.Team.EuroAutoTransfer && accountState.Team.Euro > 0)
		SomethingDoneInLoop |= FtpApi.TeamTransferEuro(accountState.TeamId, accountState.Team.Euro);

	if (RuntimeProps.BetManager && accountState.Bet.BetsLeft > 0)
		SomethingDoneInLoop |= FtpApi.BetManager(accountState.Bet.Matches, RuntimeProps.BetValue, RuntimeProps.BetMinCourse, accountState);

	if (RuntimeProps.TrickLearn && accountState.Trick.Queue == null)
		SomethingDoneInLoop |= FtpApi.LearnTrick(RuntimeProps.Trick, accountState.Trick.Tricks);

	if (RuntimeProps.EatFood && accountState.Canteen.Queue == null)
		FoodResolver(accountState);

	if (RuntimeProps.TrainingLimit > accountState.TrainedToday && accountState.Energy > 100)
	{
		if (RuntimeProps.Training.Specialize)
		{
			if (RuntimeProps.Training.UseBot)
				SomethingDoneInLoop |= FtpApi.BotTrainingSpecialization(accountState.Energy, RuntimeProps.Training.Learning, RuntimeProps.Training.Skill, RuntimeProps.Training.Training1, RuntimeProps.Training.Training2);
			else
				SomethingDoneInLoop |= FtpApi.TrainingSpecialization(10, RuntimeProps.Training.Learning, RuntimeProps.Training.Skill, RuntimeProps.Training.Training1, RuntimeProps.Training.Training2);
		}
		else
			if (RuntimeProps.Training.UseBot)
			SomethingDoneInLoop |= FtpApi.NormalBotTraining(accountState.Energy);
		else
			SomethingDoneInLoop |= FtpApi.NormalTraining(60);
	}

	if (RuntimeProps.Team.Training && accountState.ServerTimeHour() >= accountState.Team.TrainingHour && accountState.ServerTimeHour() < accountState.Team.TrainingHour + 1)
		SomethingDoneInLoop |= FtpApi.TeamTraining(accountState, RuntimeProps.Team.TrainingSkill, RuntimeProps.Team.MessageNotification);

	if (accountState.Team.NextMatch != null && RuntimeProps.Team.MatchBooster)
		SomethingDoneInLoop |= FtpApi.MatchBooster(accountState.Team.NextMatch, RuntimeProps.Team, accountState.TeamId);

	if (RuntimeProps.TrainingCenterAfterLimit && accountState.TrainingCenterUsedToday == 0 && accountState.TrainedToday > RuntimeProps.TrainingLimit && accountState.Energy >= RuntimeProps.TrainingCenterAmount)
		SomethingDoneInLoop |= FtpApi.TrainingCenter(RuntimeProps.TrainingCenterSkill, RuntimeProps.TrainingCenterAmount);

	if (RuntimeProps.Team.Salary)
		SomethingDoneInLoop |= FtpApi.GetSalaryFromTeam(accountState);

	if (RuntimeProps.TrickPlayer)
		SomethingDoneInLoop |= FtpApi.TrickFight(accountState);

	if (RuntimeProps.AutoGetCardPack && !accountState.Canteen.CanteenTasks.Where(a => !a.Finished).Any())
		SomethingDoneInLoop |= FtpApi.GetCardPack(accountState);

	if (RuntimeProps.AutoOpenCardPacks && accountState.Packs.Card > 0)
		SomethingDoneInLoop |= FtpApi.OpenCardPacks(accountState.Packs.Card);

	if (RuntimeProps.Team.AutoSparingSignUp)
		SomethingDoneInLoop |= FtpApi.SparingSignUp(accountState);

	if (RuntimeProps.TargetEuro > accountState.Euro && RuntimeProps.JobType >= 1 && RuntimeProps.JobType <= 9 && accountState.Job.Queue == null)
		SomethingDoneInLoop |= FtpApi.StartJob(RuntimeProps.JobType);

	if (RuntimeProps.Team.GenerateRaportFile)
		SomethingDoneInLoop |= FtpApi.GenerateTeamStats(accountState);

	if (!SomethingDoneInLoop)
		Thread.Sleep(40000);
}

void LogAccountState(AccountState accState)
{
	Console.Title = $"Nick: {accState.Name}, Conf: {Configuration}";
	var currentCanteenState = accState.Canteen.Queue != null ? $"Current meal: {accState.Canteen.Queue.Canteen_id} - Remaining: {TimeSpan.FromSeconds(ConvertDateFromUnixTime(accState.Canteen.Queue.End)):hh\\:mm\\:ss}" : string.Empty;

	Logger.LogI($"Name: {accState.Name}[{accState.Overall}], Euros: {accState.Euro.ToString("C", CultureInfo.CurrentCulture)}, Energy: {accState.Energy}", "USERSTATE");
	Logger.LogI($"MAIN STATS: [{accState.Defensive},{accState.Condition},{accState.Pressing},{accState.Freekicks}]", "USERSTATE");
	Logger.LogI($"Canteen: {accState.Canteen.Used} / {accState.Canteen.Limit}. {currentCanteenState}", "USERSTATE-CANTEEN");
	Logger.LogI($"Premium keys: {accState.Packs.PremiumKeys}, Free keys: {accState.Packs.FreeKeys}. Packs: bronze:{accState.Packs.Bronze}, silver:{accState.Packs.Silver}, gold:{accState.Packs.Gold}, energy:{accState.Packs.Energy}", "USERSTATE-PACKS");
	Logger.LogI($"Bets left: {accState.Bet.BetsLeft}. Today points: {Math.Round(accState.Bet.DayPoints, 2)}, profit: {accState.Bet.DayProfit.ToString("C", CultureInfo.CurrentCulture)} ", "USERSTATE-BETS");

	if (accState.Job.Queue != null)
		Logger.LogI($"Current job: {accState.Job.Queue.Job_id}", "USERSTATE-JOB");

	if (accState.Trick.Queue != null)
		Logger.LogI($"Current training trick: {accState.Trick.Queue.Trick_name}. Remaining: {TimeSpan.FromSeconds(accState.Trick.Queue.Left_seconds):hh\\:mm\\:ss}", "USERSTATE-TRICKS");

	Logger.LogI($"{accState.Team.Name}: [{accState.Team.Ovr}] Building Euro: {accState.Team.EuroBuilding.ToString("C", CultureInfo.CurrentCulture)}", "USERSTATE-TEAM");

}


void CantineeTasksResolver(AccountState accState)
{
	var opName = "CANTEEN-TASKS";
	foreach (var task in accState.Canteen.CanteenTasks.Where(a => a.Finished == false))
	{
		switch (task.Key)
		{
			case "calendar":
				Logger.LogI("CALENDAR - left to do", opName);
				if (RuntimeProps.Cantinee.CalendarChecker && !accState.CalendarFinished)
					SomethingDoneInLoop |= FtpApi.CalendarChecker();
				break;

			case "jobs":
				Logger.LogI("JOBS - left to do", opName);
				if (RuntimeProps.Cantinee.Jobs && accState.Job.Queue == null && accState.Energy > 24)
					SomethingDoneInLoop |= FtpApi.StartJob(7);
				break;

			case "golden_balls_warehouse":
				Logger.LogI("GB-WAREHOUSE - left to do", opName);
				if (RuntimeProps.Cantinee.GoldenBallsWarehouse)
					SomethingDoneInLoop |= FtpApi.DonateWarehouse(accState, "golden_balls", RuntimeProps.Cantinee.AmountGoldenBallsWarehouse);
				break;

			case "material_warehouse":
				Logger.LogI("ITEM-WAREHOUSE - left to do", opName);
				if (RuntimeProps.Cantinee.DonateItemWarehouse && accState.Item.ItemStats.Poor > 0)
					SomethingDoneInLoop |= FtpApi.DonateWarehouse(accState, "items");
				break;

			case "sell_items_for_golden_balls":
				Logger.LogI("SELL ITEM FOR GOLDEN BALL - left to do ", opName);
				if (RuntimeProps.Cantinee.SellingItems && accState.Item.ItemStats.Poor > 0)
				{
					var soldItem = FtpApi.SellWeakestItem(accState.Item.Items.Where(a => a.Rarity == "poor").ToArray(), true);
					if (soldItem != null)
					{
						var list = accState.Item.Items.ToList();
						list.Remove(soldItem);
						accState.Item.Items = list.ToArray();
						accState.Item.ItemStats.Poor -= 1;
						SomethingDoneInLoop |= true;
					}
				}
				break;

			case "sell_items_for_euro":
				Logger.LogI("SELL ITEM FOR EURO - left to do", opName);
				if (RuntimeProps.Cantinee.SellingItems && accState.Item.ItemStats.Poor > 0)
				{
					var soldItem = FtpApi.SellWeakestItem(accState.Item.Items.Where(a => a.Rarity == "poor").ToArray());
					if (soldItem != null)
					{
						var list = accState.Item.Items.ToList();
						list.Remove(soldItem);
						accState.Item.Items = list.ToArray();
						accState.Item.ItemStats.Poor -= 1;
						SomethingDoneInLoop |= true;
					}
				}
				break;

			case "augment":
				Logger.LogI("AUGMENT - left to do", opName);
				if (RuntimeProps.Cantinee.Augment)
					SomethingDoneInLoop |= FtpApi.Augment(RuntimeProps.Cantinee.AugmentItemId, RuntimeProps.Cantinee.AugmentItemType);
				break;

			case "boosters":
				Logger.LogI("EXCHANGE-BOOSTERS - left to do", opName);
				if (RuntimeProps.Cantinee.ExchangeBoosters)
					SomethingDoneInLoop |= FtpApi.ExchangeBoosters(RuntimeProps.Cantinee.BoosterId, 3);
				break;

			case "training_bonus":
				Logger.LogI($"TRAINING (1250/{accState.TrainedToday}) - left to do ", opName);
				break;

			default:
				break;
		}
	}
}

void FoodResolver(AccountState accState)
{
	var mealLeft = accState.Canteen.Limit - accState.Canteen.Used;

	switch (mealLeft)
	{
		case 0:
			return;
		case 1:
			if (accState.ServerTimeHour() >= 18 || accState.Canteen.Used == 29)
				SomethingDoneInLoop |= FtpApi.EatMeal(6);
			break;
		case 2:
			if (accState.ServerTimeHour() >= 16 || accState.Canteen.Used == 28)
				SomethingDoneInLoop |= FtpApi.EatMeal(4);
			break;
		default:
			SomethingDoneInLoop |= FtpApi.EatMeal(1);
			break;
	}
}


public partial class Program
{
	static readonly RuntimeProperties RuntimeProps = new();
	static string ConfigurationFileName = string.Empty;
	static int Configuration;
	static bool SomethingDoneInLoop;

	public static void StartupProcedure()
	{
		var opName = "STARTUP-CONFIGURATION";
		Logger.LogI("Start App", opName);
		while (String.IsNullOrEmpty(ConfigurationFileName))
		{
			try
			{
				Logger.LogConfiguration("Choose your configuration: ", opName);
				string? input = Console.ReadLine();
				if (!string.IsNullOrEmpty(input))
				{
					Configuration = Int32.Parse(input);
					ConfigurationFileName = $"configuration{Configuration}.json";
					RuntimeProps.NotifyChange = true;
					Console.Title = $"Configuration: {Configuration}";
				}
				else
					throw new ArgumentException();
			}

			catch (Exception)
			{
				Logger.LogE("Configuration must be a number.", opName);
				continue;
			}
		}
	}

	public static void ReadRuntimeProperties(bool initialize = false)
	{
		if (initialize)
		{
			using StreamReader r = new(Directory.GetCurrentDirectory() + $"//Configurations//{ConfigurationFileName}");
			var runtimePropsFromConfig = JsonSerializer.Deserialize<RuntimeProperties>(r.BaseStream);

			if (runtimePropsFromConfig != null)
			{
				RuntimeProps.Email = runtimePropsFromConfig.Email;
				RuntimeProps.Password = runtimePropsFromConfig.Password;
				RuntimeProps.FingerPrint = runtimePropsFromConfig.FingerPrint;
			}
			else
				throw new ArgumentException("Runtimeproperties doesn't exist.");
		}

		using (StreamReader r = new(Directory.GetCurrentDirectory() + $"//Configurations//{ConfigurationFileName}"))
		{
			var runtimePropsFromConfig = JsonSerializer.Deserialize<RuntimeProperties>(r.BaseStream);

			if (runtimePropsFromConfig != null)
			{
				RuntimeProps.Server = runtimePropsFromConfig.Server;

				RuntimeProps.TrainingLimit = runtimePropsFromConfig.TrainingLimit;
				RuntimeProps.TrainingCenterAfterLimit = runtimePropsFromConfig.TrainingCenterAfterLimit;
				RuntimeProps.TrainingCenterAmount = runtimePropsFromConfig.TrainingCenterAmount;
				RuntimeProps.TrainingCenterSkill = runtimePropsFromConfig.TrainingCenterSkill;
				RuntimeProps.Training.UseBot = runtimePropsFromConfig.Training.UseBot;
				RuntimeProps.Training.Training1 = runtimePropsFromConfig.Training.Training1;
				RuntimeProps.Training.Training2 = runtimePropsFromConfig.Training.Training2;
				RuntimeProps.Training.Learning = runtimePropsFromConfig.Training.Learning;
				RuntimeProps.Training.Skill = runtimePropsFromConfig.Training.Skill;
				RuntimeProps.Training.Specialize = runtimePropsFromConfig.Training.Specialize;

				RuntimeProps.TrickLearn = runtimePropsFromConfig.TrickLearn;
				RuntimeProps.Trick = runtimePropsFromConfig.Trick;
				RuntimeProps.TrickPlayer = runtimePropsFromConfig.TrickPlayer;

				RuntimeProps.BetManager = runtimePropsFromConfig.BetManager;
				RuntimeProps.BetMinCourse = runtimePropsFromConfig.BetMinCourse;
				RuntimeProps.BetValue = runtimePropsFromConfig.BetValue;

				RuntimeProps.Team.Training = runtimePropsFromConfig.Team.Training;
				RuntimeProps.Team.TrainingSkill = runtimePropsFromConfig.Team.TrainingSkill;
				RuntimeProps.Team.EuroAutoTransfer = runtimePropsFromConfig.Team.EuroAutoTransfer;
				RuntimeProps.Team.MatchBooster = runtimePropsFromConfig.Team.MatchBooster;
				RuntimeProps.Team.BoosterSkill = runtimePropsFromConfig.Team.BoosterSkill;
				RuntimeProps.Team.Salary = runtimePropsFromConfig.Team.Salary;
				RuntimeProps.Team.AutoSparingSignUp = runtimePropsFromConfig.Team.AutoSparingSignUp;
				RuntimeProps.Team.MessageNotification = runtimePropsFromConfig.Team.MessageNotification;
				RuntimeProps.Team.GenerateRaportFile = runtimePropsFromConfig.Team.GenerateRaportFile;

				RuntimeProps.Team.CountryBoosterLevel = runtimePropsFromConfig.Team.CountryBoosterLevel;
				RuntimeProps.Team.CountryEngagementLevel = runtimePropsFromConfig.Team.CountryEngagementLevel;

				RuntimeProps.Team.LeagueBoosterLevel = runtimePropsFromConfig.Team.LeagueBoosterLevel;
				RuntimeProps.Team.LeagueBoosterEngagementLevel = runtimePropsFromConfig.Team.LeagueBoosterEngagementLevel;

				RuntimeProps.Team.TournamentBoosterLevel = runtimePropsFromConfig.Team.TournamentBoosterLevel;
				RuntimeProps.Team.TournamentBoosterEngagementLevel = runtimePropsFromConfig.Team.TournamentBoosterEngagementLevel;

				RuntimeProps.Team.SparingBoosterLevel = runtimePropsFromConfig.Team.SparingBoosterLevel;
				RuntimeProps.Team.SparingBoosterEngagementLevel = runtimePropsFromConfig.Team.SparingBoosterEngagementLevel;

				RuntimeProps.GetFreeStarter = runtimePropsFromConfig.GetFreeStarter;
				RuntimeProps.GetFreeStarterEvent = runtimePropsFromConfig.GetFreeStarterEvent;

				RuntimeProps.TargetEuro = runtimePropsFromConfig.TargetEuro;
				RuntimeProps.JobType = runtimePropsFromConfig.JobType;

				RuntimeProps.CleanMailBox = runtimePropsFromConfig.CleanMailBox;
				RuntimeProps.AutoGetCardPack = runtimePropsFromConfig.AutoGetCardPack;
				RuntimeProps.AutoOpenCardPacks = runtimePropsFromConfig.AutoOpenCardPacks;
				RuntimeProps.EatFood = runtimePropsFromConfig.EatFood;

				RuntimeProps.Cantinee.Resolver = runtimePropsFromConfig.Cantinee.Resolver;
				RuntimeProps.Cantinee.CalendarChecker = runtimePropsFromConfig.Cantinee.CalendarChecker;
				RuntimeProps.Cantinee.Jobs = runtimePropsFromConfig.Cantinee.Jobs;
				RuntimeProps.Cantinee.SellingItems = runtimePropsFromConfig.Cantinee.SellingItems;
				RuntimeProps.Cantinee.GoldenBallsWarehouse = runtimePropsFromConfig.Cantinee.GoldenBallsWarehouse;
				RuntimeProps.Cantinee.AmountGoldenBallsWarehouse = runtimePropsFromConfig.Cantinee.AmountGoldenBallsWarehouse;
				RuntimeProps.Cantinee.DonateItemWarehouse = runtimePropsFromConfig.Cantinee.DonateItemWarehouse;
				RuntimeProps.Cantinee.Augment = runtimePropsFromConfig.Cantinee.Augment;
				RuntimeProps.Cantinee.AugmentItemId = runtimePropsFromConfig.Cantinee.AugmentItemId;
				RuntimeProps.Cantinee.AugmentItemType = runtimePropsFromConfig.Cantinee.AugmentItemType;
				RuntimeProps.Cantinee.ExchangeBoosters = runtimePropsFromConfig.Cantinee.ExchangeBoosters;
				RuntimeProps.Cantinee.BoosterId = runtimePropsFromConfig.Cantinee.BoosterId;

			}
			else
				throw new ArgumentException("Runtimeproperties doesn't exist.");
		}
	}

	public static double ConvertDateFromUnixTime(long unixTime)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		epoch = epoch.AddSeconds(unixTime);
		return (epoch - DateTime.UtcNow).TotalSeconds;
	}

	public static DateTime GetNowServerDataTime(TimeZoneInfo timeZone) => TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
}
