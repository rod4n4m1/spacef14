using System;

using Microsoft.Xna.Framework;

namespace SpaceF14
{
	/// <summary>
	/// All the constants used in the game
	/// Some of the parameters here could be set on an UIa
	/// </summary>
	public static class GameConstants
	{
		// resolution
		public const int WindowWidth = 800;
		public const int WindowHeight = 800;

		// sound elements
		public const float BackgroundVolume = 1.0f;
		public const float DeathVolume = 0.8f;
		public const float DamageVolume = 0.3f;
		public const float ShotVolume = 0.2f;
		public const float ExplosionVolume = 0.4f;
		public const float BounceVolume = 0.2f;

		// global definitions
		public const float BaseSpeed = 0.05f;
		public const int BaseDelay = 500;
		public const int MaxLevel = 30;

		// Bear characteristics
		public const int BearProjectileOffset = 20;

		// tomcat characteristics
		public const int F14InitialLives =3;
		public const int F14InitialHealth = 100;
		public const float F14MovementAmount = 0.02f;
		public const float F14MaximumSpeed = 8 * BaseSpeed;
		public const float F14InitialSpeed = 0f;
		public const int F14AngleIncreasing = 1;
		public const int F14TotalCooldownMilliseconds = BaseDelay;
		public const int F14DeathCooldownMilliseconds = 6*BaseDelay;
		public const int F14InitialX = DisplayWidth / 2;
		public const int F14InitialY = DisplayHeight - DisplayHeight / 8;
		public const int F14ShieldAid = 50;
		public const int F14PowerAid = 10;
		public const int F14HealthAid = 50;
		public const int F14ScoreBonus = 500;
		public const float PlasmProjectileSpeed = 4*BaseSpeed;
		public const int PlasmProjectileDamage = 10;
		public const float LaserProjectileSpeed = 10*BaseSpeed;
		public const int LaserProjectileDamage = 5;
		public const int LaserTurretXDist = 10;
		public const int LaserTurretYDist = -5;
		public const int PlasmTurretXDist = 0;
		public const int PlasmTurretYDist = -35;
		public const int MontageFramesPerRow = 18;
		public const int MontageNumRows = 10;
		public const int MontageNumFrames = MontageFramesPerRow * MontageNumRows;

		// boss intrisic characteristics
		public const int BossDescentMovement = 10;
		public const string BossPrefix = "Boss Health: ";

		// explosion hard-coded animation info
		public const int BearExplosionFramesPerRow = 3;
		public const int BearExplosionNumRows = 3;
		public const int BearExplosionNumFrames = BearExplosionFramesPerRow * BearExplosionNumRows;
		public const int TomcatExplosionFramesPerRow = 7;
		public const int TomcatExplosionNumRows = 7;
		public const int TomcatExplosionNumFrames = TomcatExplosionFramesPerRow * TomcatExplosionNumRows;
		public const int PurpleExplosionFramesPerRow = 8;
		public const int PurpleExplosionNumRows = 4;
		public const int PurpleExplosionNumFrames = PurpleExplosionFramesPerRow * PurpleExplosionNumRows;
		public const int YellowExplosionFramesPerRow = 8;
		public const int YellowExplosionNumRows = 4;
		public const int YellowExplosionNumFrames = YellowExplosionFramesPerRow * YellowExplosionNumRows;
		public const int RockExplosionFramesPerRow = 5;
		public const int RockExplosionNumRows = 4;
		public const int RockExplosionNumFrames = RockExplosionFramesPerRow * RockExplosionNumRows;
		public const int BlueExplosionFramesPerRow = 5;
		public const int BlueExplosionNumRows = 4;
		public const int BlueExplosionNumFrames = BlueExplosionFramesPerRow * BlueExplosionNumRows;
		public const int GreenExplosionFramesPerRow = 3;
		public const int GreenExplosionNumRows = 3;
		public const int GreenExplosionNumFrames = GreenExplosionFramesPerRow * GreenExplosionNumRows;
		public const int BossExplosionFramesPerRow = 3;
		public const int BossExplosionNumRows = 3;
		public const int BossExplosionNumFrames = BossExplosionFramesPerRow * BossExplosionNumRows;
		public const int ExplosionTotalFrameMilliseconds = 15;

		// boss hard-coded animation info
		public const int Boss1FramesPerRow = 3;
		public const int Boss1NumRows = 2;
		public const int Boss1NumFrames = Boss1FramesPerRow * Boss1NumRows;
		public const int Boss2FramesPerRow = 3;
		public const int Boss2NumRows = 2;
		public const int Boss2NumFrames = Boss2FramesPerRow * Boss2NumRows;
		public const int Boss3FramesPerRow = 3;
		public const int Boss3NumRows = 2;
		public const int Boss3NumFrames = Boss3FramesPerRow * Boss3NumRows;
		public const int Boss4FramesPerRow = 3;
		public const int Boss4NumRows = 2;
		public const int Boss4NumFrames = Boss4FramesPerRow * Boss4NumRows;
		public const int Boss5FramesPerRow = 3;
		public const int Boss5NumRows = 2;
		public const int Boss5NumFrames = Boss5FramesPerRow * Boss5NumRows;
		public const int Boss6FramesPerRow = 3;
		public const int Boss6NumRows = 2;
		public const int Boss6NumFrames = Boss6FramesPerRow * Boss6NumRows;
		public const int BossTotalFrameMilliseconds = 20;

		// sign hard-coded animation info
		public const int SignNumFrames = 10;
		public const int SignTotalFrameMilliseconds = 50;
		public static readonly Color[] ColorfulSign = { Color.AliceBlue, Color.Beige, Color.BlueViolet, Color.Coral };
		public static Color SignDefaultColor = Color.Bisque;

		// pickup hard-coded animation info
		public const int RedOrbFramesPerRow = 3;
		public const int RedOrbNumRows = 2;
		public const int RedOrbNumFrames = RedOrbFramesPerRow * RedOrbNumRows;
		public const int BlueOrbFramesPerRow = 3;
		public const int BlueOrbNumRows = 2;
		public const int BlueOrbNumFrames = BlueOrbFramesPerRow * BlueOrbNumRows;
		public const int GreenOrbFramesPerRow = 3;
		public const int GreenOrbNumRows = 2;
		public const int GreenOrbNumFrames = GreenOrbFramesPerRow * GreenOrbNumRows;
		public const int GoldOrbFramesPerRow = 3;
		public const int GoldOrbNumRows = 2;
		public const int GoldOrbNumFrames = GoldOrbFramesPerRow * GoldOrbNumRows;
		public const int PickupTotalFrameMilliseconds = 180;
		public static Vector2 PickupVelocity = new Vector2(0.09f, 0.07f);
		public const int PickupCoolDownTime = 22000;

		// display support
		public const int DisplayWidth = 800;
		public const int DisplayHeight = 600;
		public static Color DisplayColor = Color.DodgerBlue;
		public static Color StringColor = Color.Blue;
		public const int DisplayOffset = 35;
		public const int StringOffSet = 30;
		// Score
		public const string ScorePrefix = "Score: ";
		public static readonly Vector2 ScorePrefixLocation =
			new Vector2(DisplayOffset, DisplayHeight + StringOffSet);
		public static readonly Vector2 ScoreLocation =
			new Vector2(DisplayOffset*5, DisplayHeight + StringOffSet);
		//Health
		public const string HealthPrefix = "Health: ";
		public static readonly Vector2 HealthPrefixLocation =
			new Vector2(DisplayOffset, DisplayHeight + 2 * StringOffSet);
		public static readonly Point HealthLocation =
			new Point(DisplayOffset * 5, (int)(DisplayHeight + 2.15 * StringOffSet));
		public const int HealthBarWidth = 250;
		public const int HealthBarHeight = 20;
		public static Color InitialHealthDisplayColor = Color.Purple;
		// Shield
		public const string ShieldPrefix = "Shield: ";
		public static readonly Vector2 ShieldPrefixLocation =
			new Vector2(DisplayOffset, DisplayHeight + 3 * StringOffSet);
		public static readonly Vector2 ShieldLocation =
			new Vector2(DisplayOffset * 5, DisplayHeight + 3 * StringOffSet);
		// Weapon
		public const string WeaponPrefix = "Weapon: ";
		public static readonly Vector2 WeaponPrefixLocation =
			new Vector2(DisplayOffset, DisplayHeight + 4 * StringOffSet);
		public static readonly Vector2 WeaponLocation =
			new Vector2(DisplayOffset*5, DisplayHeight + 4 * StringOffSet);
		// Lives
		public const string LivesPrefix = "Lives: ";
		public static readonly Vector2 LivesPrefixLocation = 
			new Vector2(DisplayWidth - 9 * DisplayOffset, DisplayHeight + StringOffSet);
		public static readonly Point LivesLocation =
			new Point(DisplayWidth - (int)(4.5 * DisplayOffset), DisplayHeight + StringOffSet);
		// Level
		public const string LevelPrefix = "Level: ";
		public static readonly Vector2 LevelPrefixLocation =
			new Vector2(DisplayWidth - 9 * DisplayOffset, DisplayHeight + 2 * StringOffSet);
		public static readonly Vector2 LevelLocation =
			new Vector2(DisplayWidth - (int)(4.5 * DisplayOffset), DisplayHeight + 2 * StringOffSet);
		// Next
		public const string NextPrefix = "Next Level: ";
		public static readonly Vector2 NextPrefixLocation =
			new Vector2(DisplayWidth - 9 * DisplayOffset, DisplayHeight + 3 * StringOffSet);
		public static readonly Vector2 NextLocation =
			new Vector2(DisplayWidth - (int)(4.5 * DisplayOffset), DisplayHeight + 3 * StringOffSet);

		// help message
		public const int HelpRow = WindowHeight - StringOffSet;
		public static Color HelpMessageColor = Color.DarkGreen;

		// introduction
		public static Color IntroTitleColor = Color.Red;
		public static Color IntroTextColor = Color.White;
		public static Color IntroCallColor = Color.Yellow;
		public const float IntroTitleRow = DisplayHeight - DisplayOffset * 3;
		public const string IntroTitleString = "Space #F14";
		public static readonly string[] IntroTextBody = {"Earth had been invaded and we have been defeated by alien",
			"forces. Modern warfare was destroyed and only obsolete",
			"equipments are available to the fewer remaining defenders.",
			"You are an ace acrobat pilot using an old F14 Tomcat fighter",
			"to protect what is left of your city. ",
			"", 
			"Hope is a dying sentiment and you don't stand a chance.",
			"During a recon flight, an intense light appeared in front of",
			"you and suddenly you are in orbit now. You changed, your",
			"ship changed, but you are not alone ... teddy bears !?!?",
			"",
			"It is time for fighting back ..."};
		public const int IntroBodySize = 12;
		public const string IntroMessage = "Press space to start it up";
		public const float RollingStringSpeed = 0.6f;

		// credits
		public static Color CreditsTitleColor = Color.Red;
		public static Color CreditsTextColor = Color.Yellow;
		public const float CreditsTitleRow = DisplayHeight - DisplayOffset * 3;
		public const string CreditsTitleString = "Space #F14";
		public static readonly string[] CreditsTextBody = {"Earth has been saved ... for now ...",
			"",
			"Credits and Mentions",
			"Game Design, Lead Developer and Lead Tester",
			"Rod Anami (@ranami)",
			"",
			"Thanks to the inumerous people who created",
			"sprites and sound effects used on this game.",
			"",
			"Special thanks to my wife Tati for her support",
			"and my son Gabe who was its first player.",
			""};
		public const int CreditsBodySize = 12;
		public const string CreditsMessage = "Press space to end game";

		// startup
		public static readonly string[] StartUpMessages = {"Get Ready","3","2","1","Light them up"};
		public static readonly string[] StartUpMessagesBoss = { "Try again", "3", "2", "1", "Kill it"};
		public const string StartUpHelp = "Initiating systems";

		// fighting
		public const string FightingHelp = "Use arrows for control and 'space' to fire";

		// level up
		public static readonly string[] LevelUpMessages = { "New level", "3", "2", "1", "Bring them down" };
		public static readonly string[] LevelUpMessagesBoss = { "BOSS level", "3", "2", "1", "Destroy it" };
		public const string LevelUpHelp = "Warping to next zone";
		public static readonly string[] StatsStrings = { "Your Stats",
			"Fired Laser Shots: ",
			"Fired Plasm Shots: ",
			"Destroyed Asteroids: ",
			"Destroyed Meteors: ",
			"Killed Yellow Bears: ",
			"Killed Green Bears: ",
			"Killed Enemy Shots: ",
			"Hits on the Boss: ",
			"Accuracy: "
		};

		// boss fight
		public const string BossFightHelp = "You better run";

		// death
		public const string DeathHelp = "You've died noobie";

		// game over
		public const string GameOverMessage = "Game Over";
		public const string GameOverHelp = "That's all folks, press 'Esc' to quit";

		// paused
		public const string PausedHelp = "Paused - press 'p' to continue";

		// spawn location support
		public const int SpawnBorderSize = 60;
	}
}
