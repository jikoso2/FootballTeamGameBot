using FootballteamBOT.ApiHelper;
using FootballteamBOT;
using System.Globalization;
using System.Text.Json;

StartupProcedure();
ReadRuntimeProperties(true);

var FtpApi = new FTPApi(RuntimeProps.Server, Configuration);
FtpApi.Login(RuntimeProps.Email, RuntimeProps.Password, RuntimeProps.FingerPrint);

//for (int i = 0; i < 25; i++)
//{
//	int itemid = 35685320;
//	int enchantLevel = 10;

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

	LogUserState(accountState);

	if (RuntimeProps.Cantinee.Resolver)
		CantineeTasksResolver(accountState);

	if (RuntimeProps.GetFreeStarter)
		SomethingDoneInLoop |= FtpApi.GetFreeStarter();

	if (RuntimeProps.GetFreeStarterEvent)
		SomethingDoneInLoop |= FtpApi.GetFreeStarterEvent();

	if (RuntimeProps.CleanMailBox)
		SomethingDoneInLoop |= FtpApi.CleanMailBox();

	if (RuntimeProps.ClubEuroAutoTransfer && accountState.Team.Euro > 0)
		SomethingDoneInLoop |= FtpApi.ClubTransferEuro(accountState.TeamId, accountState.Team.Euro);

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

	if (RuntimeProps.ClubTraining && GetNowServerDataTime(accountState.TimeZone).Hour >= accountState.Team.TrainingHour && GetNowServerDataTime(accountState.TimeZone).Hour < accountState.Team.TrainingHour + 1)
		SomethingDoneInLoop |= FtpApi.TeamTraining(accountState.TeamId, RuntimeProps.ClubTrainingSkill, RuntimeProps.ClubMessageNotification);

	if (accountState.Team.NextMatch != null && RuntimeProps.ClubMatchBooster)
		SomethingDoneInLoop |= FtpApi.MatchBooster(accountState.Team.NextMatch, RuntimeProps.ClubBoosterSkill, RuntimeProps.ClubBoosterLevel, RuntimeProps.ClubBoosterEngagementLevel, RuntimeProps.ClubMessageNotification, accountState.TeamId);

	if (RuntimeProps.TrainingCenterAfterLimit && accountState.TrainingCenterUsedToday == 0 && accountState.TrainedToday > RuntimeProps.TrainingLimit && accountState.Energy >= RuntimeProps.TrainingCenterAmount)
		SomethingDoneInLoop |= FtpApi.TrainingCenter(RuntimeProps.TrainingCenterSkill, RuntimeProps.TrainingCenterAmount);

	if (RuntimeProps.ClubSalary)
		SomethingDoneInLoop |= FtpApi.GetSalaryFromTeam(accountState.TeamId);

	if (RuntimeProps.TrickPlayer)
		SomethingDoneInLoop |= FtpApi.TrickFight(accountState);

	if (!SomethingDoneInLoop)
		Thread.Sleep(40000);
}

void LogUserState(AccountState userState)
{
	Console.Title = $"Nick: {userState.Name}, Conf: {Configuration}";
	var currentCanteenState = userState.Canteen.Queue != null ? $"Current meal: {userState.Canteen.Queue.Canteen_id} - Remaining: {TimeSpan.FromSeconds(ConvertDateFromUnixTime(userState.Canteen.Queue.End)):hh\\:mm\\:ss}" : string.Empty;

	Logger.LogI($"Name: {userState.Name}[{userState.Overall}], Euros: {userState.Euro.ToString("C", CultureInfo.CurrentCulture)}, Energy: {userState.Energy}", "USERSTATE");
	Logger.LogI($"MAIN STATS: [{userState.Defensive},{userState.Condition},{userState.Pressing},{userState.Freekicks}]", "USERSTATE");
	Logger.LogI($"Canteen: {userState.Canteen.Used} / {userState.Canteen.Limit}. {currentCanteenState}", "USERSTATE-CANTEEN");
	Logger.LogI($"Premium keys: {userState.Packs.PremiumKeys}, Free keys: {userState.Packs.FreeKeys}. Packs: bronze:{userState.Packs.Bronze}, silver:{userState.Packs.Silver}, gold:{userState.Packs.Gold}, energy:{userState.Packs.Energy}", "USERSTATE-PACKS");
	Logger.LogI($"Bets left: {userState.Bet.BetsLeft}. Today points: {Math.Round(userState.Bet.DayPoints, 2)}, profit: {userState.Bet.DayProfit.ToString("C", CultureInfo.CurrentCulture)} ", "USERSTATE-BETS");

	if (userState.Job.Queue != null)
		Logger.LogI($"Current job: {userState.Job.Queue.Job_id}", "USERSTATE-JOB");

	if (userState.Trick.Queue != null)
		Logger.LogI($"Current training trick: {userState.Trick.Queue.Trick_name}. Remaining: {TimeSpan.FromSeconds(userState.Trick.Queue.Left_seconds):hh\\:mm\\:ss}", "USERSTATE-TRICKS");

	Logger.LogI($"{userState.Team.Name}: [{userState.Team.Ovr}] Building Euro: {userState.Team.EuroBuilding.ToString("C", CultureInfo.CurrentCulture)}", "USERSTATE-TEAM");

}


void CantineeTasksResolver(AccountState userState)
{
	var opName = "CANTEEN-TASKS";
	foreach (var task in userState.Canteen.CanteenTasks.Where(a => a.Finished == false))
	{
		switch (task.Key)
		{
			case "calendar":
				Logger.LogI("CALENDAR - left to do", opName);
				if (RuntimeProps.Cantinee.CalendarChecker && !userState.CalendarFinished)
					SomethingDoneInLoop |= FtpApi.CalendarChecker();
				break;

			case "jobs":
				Logger.LogI("JOBS - left to do", opName);
				if (RuntimeProps.Cantinee.Jobs && userState.Job.Queue == null && userState.Energy > 24)
					SomethingDoneInLoop |= FtpApi.StartJob(7);
				break;

			case "golden_balls_warehouse":
				Logger.LogI("GB-WAREHOUSE - left to do", opName);
				if (RuntimeProps.Cantinee.GoldenBallsWarehouse)
					SomethingDoneInLoop |= FtpApi.DonateWarehouse(userState, "golden_balls", RuntimeProps.Cantinee.AmountGoldenBallsWarehouse);
				break;

			case "material_warehouse":
				Logger.LogI("ITEM-WAREHOUSE - left to do", opName);
				if (RuntimeProps.Cantinee.DonateItemWarehouse && userState.Item.ItemStats.Poor > 0)
					SomethingDoneInLoop |= FtpApi.DonateWarehouse(userState, "items");
				break;

			case "sell_items_for_golden_balls":
				Logger.LogI("SELL ITEM FOR GOLDEN BALL - left to do ", opName);
				if (RuntimeProps.Cantinee.SellingItems && userState.Item.ItemStats.Poor > 0)
				{
					var soldItem = FtpApi.SellWeakestItem(userState.Item.Items.Where(a => a.Rarity == "poor").ToArray(), true);
					if (soldItem != null)
					{
						var list = userState.Item.Items.ToList();
						list.Remove(soldItem);
						userState.Item.Items = list.ToArray();
						userState.Item.ItemStats.Poor -= 1;
						SomethingDoneInLoop |= true;
					}
				}
				break;

			case "sell_items_for_euro":
				Logger.LogI("SELL ITEM FOR EURO - left to do", opName);
				if (RuntimeProps.Cantinee.SellingItems && userState.Item.ItemStats.Poor > 0)
				{
					var soldItem = FtpApi.SellWeakestItem(userState.Item.Items.Where(a => a.Rarity == "poor").ToArray());
					if (soldItem != null)
					{
						var list = userState.Item.Items.ToList();
						list.Remove(soldItem);
						userState.Item.Items = list.ToArray();
						userState.Item.ItemStats.Poor -= 1;
						SomethingDoneInLoop |= true;
					}
				}
				break;

			case "training_bonus":
				Logger.LogI($"TRAINING (1250/{userState.TrainedToday}) - left to do ", opName);
				break;

			default:
				break;
		}
	}
}

void FoodResolver(AccountState userState)
{
	var mealLeft = userState.Canteen.Limit - userState.Canteen.Used;

	switch (mealLeft)
	{
		case 0:
			return;
		case 1:
			if (GetNowServerDataTime(userState.TimeZone).Hour >= 18 || userState.Canteen.Used == 29)
				SomethingDoneInLoop |= FtpApi.EatMeal(6);
			break;
		case 2:
			if (GetNowServerDataTime(userState.TimeZone).Hour >= 16 || userState.Canteen.Used == 28)
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

				RuntimeProps.ClubTraining = runtimePropsFromConfig.ClubTraining;
				RuntimeProps.ClubTrainingSkill = runtimePropsFromConfig.ClubTrainingSkill;
				RuntimeProps.ClubEuroAutoTransfer = runtimePropsFromConfig.ClubEuroAutoTransfer;
				RuntimeProps.ClubMatchBooster = runtimePropsFromConfig.ClubMatchBooster;
				RuntimeProps.ClubBoosterSkill = runtimePropsFromConfig.ClubBoosterSkill;
				RuntimeProps.ClubBoosterLevel = runtimePropsFromConfig.ClubBoosterLevel;
				RuntimeProps.ClubBoosterEngagementLevel = runtimePropsFromConfig.ClubBoosterEngagementLevel;
				RuntimeProps.ClubSalary = runtimePropsFromConfig.ClubSalary;
				RuntimeProps.ClubMessageNotification = runtimePropsFromConfig.ClubMessageNotification;

				RuntimeProps.GetFreeStarter = runtimePropsFromConfig.GetFreeStarter;
				RuntimeProps.GetFreeStarterEvent = runtimePropsFromConfig.GetFreeStarterEvent;

				RuntimeProps.CleanMailBox = runtimePropsFromConfig.CleanMailBox;

				RuntimeProps.EatFood = runtimePropsFromConfig.EatFood;

				RuntimeProps.Cantinee.Resolver = runtimePropsFromConfig.Cantinee.Resolver;
				RuntimeProps.Cantinee.CalendarChecker = runtimePropsFromConfig.Cantinee.CalendarChecker;
				RuntimeProps.Cantinee.Jobs = runtimePropsFromConfig.Cantinee.Jobs;
				RuntimeProps.Cantinee.SellingItems = runtimePropsFromConfig.Cantinee.SellingItems;
				RuntimeProps.Cantinee.GoldenBallsWarehouse = runtimePropsFromConfig.Cantinee.GoldenBallsWarehouse;
				RuntimeProps.Cantinee.AmountGoldenBallsWarehouse = runtimePropsFromConfig.Cantinee.AmountGoldenBallsWarehouse;
				RuntimeProps.Cantinee.DonateItemWarehouse = runtimePropsFromConfig.Cantinee.DonateItemWarehouse;
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
