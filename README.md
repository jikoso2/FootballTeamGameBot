# FootballteamBOT

The application automates the game of footballteam. He does most of the activities like training, eating meals, completing daily missions, and a few others.
There is one executable .exe file and necessary configuration file.
The user can use several accounts on many servers in parallel.

## Contents
* [Guide](#guide)
* [Technologies](#technologies)
* [Status](#status)
* [Contact](#contact)

## Guide
Prepare to using application:
* Download all files (.exe and configurations + trickFile) - https://drive.google.com/drive/folders/1eBQxVoedWwFPOMfsEYj1QQajbwscC7a9?usp=sharing
* Rename configurationx.json.template to configuration{x}.json (where {x} is a number, default : 1)
* Fill configuration in .json
* Start .exe and select number of configuration
![image](https://user-images.githubusercontent.com/69644118/233806073-f6b063cd-e536-40ab-b05e-f24b8822ba34.png)


### Log colours
* Red - error and details
* Violet - configuration (changed or initialize)
* Green - account state
* White - action performed by application

### How configure app
![image](https://user-images.githubusercontent.com/69644118/233806239-227f8eba-5650-46a5-be9c-01afc7b84fca.png)

* Email - account email
* Password - account password
* FingerPrint - computer fingerprint
* Server - you server: "pl","us" or "en"
* TrainingLimit - trainings per day
* Training
  * Skill - training skill
  * Specialize - training 100+ skills
  * Learning - use booster to learning (100+ skills)
  * Training1 - first skill to train (first,second,third,fourth)
  * Training2 - second skill to train (first,second,third,fourth)
  * UseBot - (true,false)
* TrickLearn - activate trick module (true,false)
* Trick - select trick to training
* BetManager - activate bet manager
* BetMinCourse - course to bet
* BetValue -bet $ (10kk - 10k)
* ClubTraining - activate daily team training (true,false) - regardless of hour
* ClubTrainingSkill - skill
* ClubEuroAutoTransfer - auto transfer euros to building in team ( only team leaders)
* ClubMatchBooster - auto match boosters (true,false)
* ClubBoosterSkill - select skill to boost
* ClubBoosterLevel - level of booster (more than 1 have energy cost)
* ClubBoosterEngagementLevel - level of engagement booster (more than 1 have health cost) - sparing matches set auto 5 level
* GetFreeStarter - get starter(daily) (true,false)
* GetFreeStarterEvent - get event starter(daily) (true,false)
* CleanMailBox - clean training bot notifications
* EatFood - auto eat meals
* Cantinee - resolver cantine (true,false)

FingerPrint
To find fingerprint you have to press F12 and log-in, find 'login' request in Fetch/XHR : 
![image](https://user-images.githubusercontent.com/69644118/233737515-f8eec456-3332-4f43-b30e-538449ee6b2b.png)

Available skills: 
condition, defensive, pressing, efficacy, freekicks, offensive, playmaking, reading

Available tricks:
in order (przewrotka, przerzutka, zonglerka, crossover, nozyce, pietka, rabona, ruletka, wolej, zwod, podcinka, elastico, kopniecie, hokus, xover)

AutoTraining:
In the basic version app training all skills under 100level in the same proportions:
"Specialize": false
Using specialize you can train only one skill with two specializations (first,second,third,fourth)
Using Bot - you can use automatic 5 minutes trainings (good if you have bottraining booster)

#### Bet Manager
Select matches to bet:
- overall proportion more than 0.6 after divide maxOVR/minOVR including 5% bonus home match
- overall better team more than 60

#### Trick Trainer
Trick trainer using binary search algorithm (with 10 possibilites max 4 tries), 5 per day.

#### AutoFood
Between 0AM-15PM you eat only 15min meals
After 15PM eating 4h meal - (start faster when you ate 28/30 meals)
After 17PM eating 12h meal- (start faster when you ate 29/30 meals)

#### CantineeResolver
Checks unfinished and complete daily quests to unlock meals.
* CalendarChecker (1 try per hour)
* Jobs - do 3 jobs
* Goldenballswarehouse - donate 200 goldenballs to team warehouse
* Sellingitems - 1 item for euro and 1 item for golden-balls - only poor items (you need have more than 1 poor item)

## Technologies

* .NET Core 6

![image](https://user-images.githubusercontent.com/69644118/233805567-ca400c8b-892e-485e-aa44-9a9de58adb1c.png)

## Status

The application is still being developed and improved. Bugs are constantly corrected. The bot has several active users.

## Contact
Created by Jarosław Czerniak [@jikoso2](https://github.com/jikoso2) - Odwiedź mój GitHub!
