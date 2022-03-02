#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace SpaceF14
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		// game objects
		static Tomcat tomcat;
		static Boss boss;
		static List<TeddyBear> bears = new List<TeddyBear>();
		static List<Projectile> projectiles = new List<Projectile>();
		static List<Explosion> explosions = new List<Explosion>();
		static List<Pickup> pickups = new List<Pickup>();
		static List<Rock> rocks = new List<Rock>();
		static List<Sign> signs = new List<Sign>();
		static Texture2D[] sprites = new Texture2D[100];

		// scoring support
		int score = 0;
		int globalScore = 0;
		string scoreString = "0";

		// tomcat support
		string shieldString = "0%";
		string weaponString = ProjectileType.Laser.ToString();                              
		bool f14Dead = false;

		// boss support
		bool bossFight = false;

		// lives support
		int deathCooldownMilliseconds = 0;
		int lives = GameConstants.F14InitialLives;

		// pause, skip & quit support
		bool pKeyPressed = false;
		bool escKeyPressed = false;
		bool spaceKeyPressed = false;

		// introduction support
		float introTitleLocation = GameConstants.IntroTitleRow;
		float[] introBodyLocations = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
		// credits support
		float creditsTitleLocation = GameConstants.CreditsTitleRow;
		float[] creditsBodyLocations = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};

		// startup support
		int elapsedTime = 0;
		int waitedTime = 0;

		// spawn support
		int randX;
		int randY;
		float randSpeed;
		float randAngle;
		Vector2 randVelocity = new Vector2();
		TeddyBear newBear;
		Rock newRock;
		Boss newBoss;

		// text and display support
		SpriteFont fontArial20;
		SpriteFont fontArial70B;
		SpriteFont fontArial30B;
		SpriteFont fontIomanoid20B;
		//SpriteFont fontIomanoid30B;
		SpriteFont fontIomanoid70B;
		//SpriteFont fontPerfectDark20B;
		SpriteFont fontPerfectDark30B;
		//SpriteFont fontPerfectDark70B;
		Color healthDisplayColor = GameConstants.InitialHealthDisplayColor;
		Color weaponDisplayColor = Color.DarkBlue;
		Texture2D whiteRectangle;
		Rectangle lifeIcon1;
		Rectangle lifeIcon2;
		Rectangle lifeIcon3;
		int healthWidth;
		char center = 'c';
		char left = 'l';
		//char right = 'r';
		string helpMessage = "";

		// sound effects
		SoundEffect f14Damage;
		SoundEffect f14Death;
		SoundEffect[] f14ShotSounds = new SoundEffect[2];
		SoundEffect[] explosionSounds = new SoundEffect[4];
		SoundEffect[] pickupSounds = new SoundEffect[4];
		SoundEffect[] bossSounds = new SoundEffect[4];
		SoundEffect teddyBounce;
		SoundEffect teddyShot;
		SoundEffect stage1MusicEffect;
		SoundEffect boss1MusicEffect;
		SoundEffect boss2MusicEffect;
		SoundEffectInstance stage1Music;
		SoundEffectInstance boss1Music;
		SoundEffectInstance boss2Music;
		SoundEffectInstance backgroundMusic;

		// level, difficulty & stats support
		static int maxAsteroids = LevelsData.Levels[1, 2];
		static int maxMeteors = LevelsData.Levels[1, 3];
		static int maxYellowBears = LevelsData.Levels[1, 4];
		static int maxGreenBears = LevelsData.Levels[1, 5];
		static int usedPickups = 0;
		static int currentYellowBears = 0;
		static int currentGreenBears = 0;
		static int currentAsteroids = 0;
		static int currentMeteors = 0;
		static int currentLevel = 4;
		static int nextLevel = LevelsData.Levels[1,39];
		static string levelString = currentLevel.ToString();
		static string nextString = nextLevel.ToString();
		static int killedYellowBears = 0;
		static int killedGreenBears = 0;
		static int killedEnemyShots = 0;
		static int destroyedAsteroids = 0;
		static int destroyedMeteors = 0;
		static int firedLaserShots = 0;
		static int firedPlasmShots = 0;
		static int enemyFiredShots = 0;
		static int hitsOnBoss = 0;
		static float overallAccuracy = 0f;
		bool spawnAllowed = true;
		string bossBehaviour = "default";

		// collision handling
		CollisionResolutionInfo collisionInfo;
		Point collisionPoint;

		// game state FSM
		GameState gameState = GameState.Introduction;
		Boolean levelUp = false;

		// Mouse and keyboard
		MouseState mouse;
		KeyboardState keyboard;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            

			// set resolution and make mouse invisible
			graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
			graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
			IsMouseVisible = false;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			RandomNumberGenerator.Initialize();

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// load audio content
			f14Damage = Content.Load<SoundEffect>(@"audio\tomcat_damage");
			f14Death = Content.Load<SoundEffect>(@"audio\tomcat_death");
			f14ShotSounds[0] = Content.Load<SoundEffect>(@"audio\laser_shot");
			f14ShotSounds[1] = Content.Load<SoundEffect>(@"audio\plasm_shot");
			explosionSounds[0] = Content.Load<SoundEffect>(@"audio\bear_explosion");
			explosionSounds[1] = Content.Load<SoundEffect>(@"audio\projectile_explosion");
			explosionSounds[2] = Content.Load<SoundEffect>(@"audio\meteor_explosion");
			explosionSounds[3] = Content.Load<SoundEffect>(@"audio\boss_explosion");
			pickupSounds[0] = Content.Load<SoundEffect>(@"audio\powerup_pickup");
			pickupSounds[1] = Content.Load<SoundEffect>(@"audio\shield_pickup");
			pickupSounds[2] = Content.Load<SoundEffect>(@"audio\health_pickup");
			pickupSounds[3] = Content.Load<SoundEffect>(@"audio\money_pickup");
			teddyBounce = Content.Load<SoundEffect>(@"audio\bear_bounce");
			teddyShot = Content.Load<SoundEffect>(@"audio\bear_shot");
			bossSounds[0] = Content.Load<SoundEffect>(@"audio\boss_bounce");
			bossSounds[1] = Content.Load<SoundEffect>(@"audio\boss_shot");
			bossSounds[2] = Content.Load<SoundEffect>(@"audio\boss_hit");
			bossSounds[3] = Content.Load<SoundEffect>(@"audio\boss_death");
			// load background music
			stage1MusicEffect = Content.Load<SoundEffect>(@"audio\background_music");
			boss1MusicEffect = Content.Load<SoundEffect>(@"audio\background_boss1");
			boss2MusicEffect = Content.Load<SoundEffect>(@"audio\background_boss2");
			stage1Music = stage1MusicEffect.CreateInstance();
			boss1Music = boss1MusicEffect.CreateInstance();
			boss2Music = boss2MusicEffect.CreateInstance();
			stage1Music.IsLooped = true;
			stage1Music.Volume = GameConstants.BackgroundVolume;
			boss1Music.IsLooped = true;
			boss1Music.Volume = GameConstants.BackgroundVolume;
			boss2Music.IsLooped = true;
			boss2Music.Volume = GameConstants.BackgroundVolume;
			backgroundMusic = stage1Music;

			// load sprite font
			fontArial20 = Content.Load<SpriteFont>(@"fonts\Arial20");
			fontArial70B = Content.Load<SpriteFont>(@"fonts\Arial70B");
			fontArial30B = Content.Load<SpriteFont>(@"fonts\Arial30B");
			fontIomanoid20B = Content.Load<SpriteFont>(@"fonts\Iomanoid20B");
			//fontIomanoid30B = Content.Load<SpriteFont>(@"fonts\Iomanoid30B");
			fontIomanoid70B = Content.Load<SpriteFont>(@"fonts\Iomanoid70B");
			//fontPerfectDark20B = Content.Load<SpriteFont>(@"fonts\PerfectDark20B");
			fontPerfectDark30B = Content.Load<SpriteFont>(@"fonts\PerfectDark30B");
			//fontPerfectDark70B = Content.Load<SpriteFont>(@"fonts\PerfectDark70B");

			// load sprites
			// projectiles 0-29
			sprites[0] = Content.Load<Texture2D>(@"graphics\bear_shot");
			sprites[1] = Content.Load<Texture2D>(@"graphics\montage_plasm");
			sprites[2] = Content.Load<Texture2D>(@"graphics\montage_laser");
			sprites[10] = Content.Load<Texture2D>(@"graphics\boss1_shot");
			// explosions 30-49
			sprites[33] = Content.Load<Texture2D>(@"graphics\bear_explosion");
			sprites[34] = Content.Load<Texture2D>(@"graphics\tomcat_explosion");
			sprites[35] = Content.Load<Texture2D>(@"graphics\purple_explosion");
			sprites[36] = Content.Load<Texture2D>(@"graphics\rock_explosion");
			sprites[37] = Content.Load<Texture2D>(@"graphics\blue_explosion");
			sprites[38] = Content.Load<Texture2D>(@"graphics\green_explosion");
			sprites[39] = Content.Load<Texture2D>(@"graphics\boss_explosion");
			sprites[40] = Content.Load<Texture2D>(@"graphics\yellow_explosion");
			// icon and orbs 50-69
			sprites[50] = Content.Load<Texture2D>(@"graphics\tomcat_icon");
			sprites[57] = Content.Load<Texture2D>(@"graphics\blue_orb");
			sprites[58] = Content.Load<Texture2D>(@"graphics\green_orb");
			sprites[59] = Content.Load<Texture2D>(@"graphics\red_orb");
			sprites[60] = Content.Load<Texture2D>(@"graphics\gold_orb");
			// background 70-98
			sprites[70] = Content.Load<Texture2D>(@"graphics\hubble_sky");
			// logo 99
			sprites[99] = Content.Load<Texture2D>(@"graphics\wb_studios");


			// add initial Tomcat
			tomcat = new Tomcat(Content, @"graphics\montage_tomcat",f14ShotSounds);

			// initialize collision point
			collisionPoint = new Point(0,0);

			// create a square rectangle for display
			whiteRectangle = new Texture2D(GraphicsDevice, 1,1);
			whiteRectangle.SetData(new[] { Color.White });

			// create draw rectangles for life icons
			lifeIcon1 = new Rectangle(GameConstants.LivesLocation.X, GameConstants.LivesLocation.Y, 
			                          sprites[50].Width, sprites[50].Height);
			lifeIcon2 = new Rectangle(GameConstants.LivesLocation.X+20, GameConstants.LivesLocation.Y,
									  sprites[50].Width, sprites[50].Height);
			lifeIcon3 = new Rectangle(GameConstants.LivesLocation.X+40, GameConstants.LivesLocation.Y,
									  sprites[50].Width, sprites[50].Height);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState ().IsKeyDown (Keys.Escape)) {
				Exit ();
			}
#endif
			// get current keyboard state
			keyboard = Keyboard.GetState();

			// get current mouse state and update tomcat
			mouse = Mouse.GetState();

			// use FSM to control game states
			switch (gameState)
			{
				case (GameState.Introduction):
					if (waitedTime < 5)
					{
						elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
						waitedTime = (int)(elapsedTime / 1000);
					}
					// check for space
					if (keyboard.IsKeyDown(Keys.Space))
					{
						spaceKeyPressed = true;
					}
					if (spaceKeyPressed && keyboard.IsKeyUp(Keys.Space))
					{
						spaceKeyPressed = false;
						gameState = GameState.StartUp;
						elapsedTime = 0;
						waitedTime = 0;
					}
					else
					{
						if (introTitleLocation > 2*GameConstants.DisplayOffset)
						{
							introTitleLocation -= GameConstants.RollingStringSpeed;
							for (int i = 0; i < GameConstants.IntroBodySize; i++)
							{
								introBodyLocations[i] = introTitleLocation + (i + 3) * GameConstants.DisplayOffset;
							}
						}
					}
					break;
				case (GameState.Credits):
					// destroy all remaining enemies and projectiles
					DestroyGameObjects("enemies+projectiles");
					UpdateGameObjects(gameTime, "explosions+pickups");
					// sanity checking to avoid overflow
					if (waitedTime < 6)
					{
						elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
						waitedTime = (int)(elapsedTime / 1000);
					}
					// check for space key
					if (keyboard.IsKeyDown(Keys.Space))
					{
						spaceKeyPressed = true;
					}
					if (spaceKeyPressed && keyboard.IsKeyUp(Keys.Space))
					{
						spaceKeyPressed = false;
						gameState = GameState.GameOver;
					}
					else if (waitedTime > 5)
					{
						if (creditsTitleLocation > 2* GameConstants.DisplayOffset)
						{
							creditsTitleLocation -= GameConstants.RollingStringSpeed;
							for (int i = 0; i < GameConstants.CreditsBodySize; i++)
							{
								creditsBodyLocations[i] = creditsTitleLocation + (i + 3) * GameConstants.DisplayOffset;
							}
						}
					}
					break;
				case (GameState.StartUp):
					// count down to start the game
					elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
					waitedTime = (int)(elapsedTime / 1000);
					if (waitedTime > 4)
					{
						elapsedTime = 0;
						waitedTime = 0;
						if (bossFight)
						{
							gameState = GameState.BossFight;
						}
						else
						{

							gameState = GameState.Fighting;
						}
                        SetBackgroundMusic("start");
					}
					break;
				case (GameState.Paused):
					// check for unpause
					if (keyboard.IsKeyDown(Keys.P))
					{
						pKeyPressed = true;
					}
					if (pKeyPressed && keyboard.IsKeyUp(Keys.P))
					{
						pKeyPressed = false;
						if (bossFight)
						{
							gameState = GameState.BossFight;
						}
						else
						{

							gameState = GameState.Fighting;
						}
                        SetBackgroundMusic("start");
					}
					break;
				case (GameState.BossFight):
					// check for pause
					if (keyboard.IsKeyDown(Keys.P))
					{
						pKeyPressed = true;
					}
					if (pKeyPressed && keyboard.IsKeyUp(Keys.P))
					{
						pKeyPressed = false;
                        SetBackgroundMusic("stop");
						gameState = GameState.Paused;
					}
					else
					{
						elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
						UpdateGameObjects(gameTime, "boss");
						CheckAllCollisions(gameTime);
                        UpdateEntitiesCounters();
						if (tomcat.Health <= 0)
						{
							lives -= 1;
							elapsedTime = 0;
							f14Dead = true;
							explosions.Add(new Explosion(ExplosionType.TomcatDeath, sprites[34], tomcat.Location));
							f14Death.Play(volume: GameConstants.DeathVolume, pitch: 0.0f, pan: 0.0f);
                            SetBackgroundMusic("stop");
							if (lives < 0)
							{
								gameState = GameState.GameOver;
							}
							else
								gameState = GameState.Death;
						}
						else if (boss.Health <= 0)
						{
							// boss defeated, add explosion and go to new level
                            SetBackgroundMusic("stop");
							bossFight = false;
							// initiave several explosions
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X, 
							                             boss.Location.Y-boss.CollisionRectangle.Height/3, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X, 
							                             boss.Location.Y+boss.CollisionRectangle.Height/3, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X-boss.CollisionRectangle.Width/4, 
							                             boss.Location.Y-boss.CollisionRectangle.Height/4, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X-boss.CollisionRectangle.Width/4,
														 boss.Location.Y+boss.CollisionRectangle.Height/4, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X+boss.CollisionRectangle.Width/4,
														 boss.Location.Y-boss.CollisionRectangle.Height/4, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X-boss.CollisionRectangle.Width/4,
														 boss.Location.Y+boss.CollisionRectangle.Height/4, 50));
							explosions.Add(new Explosion(ExplosionType.BossDeath, sprites[39], boss.Location.X, boss.Location.Y, 50));
							bossSounds[3].Play(volume: GameConstants.DeathVolume, pitch: 0.0f, pan: 0.0f);
							boss.Active = false;
							currentLevel++;
							bossFight = false;
							spawnAllowed = false;
							elapsedTime = 0;
							if (currentLevel > GameConstants.MaxLevel)
							{
								gameState = GameState.Credits;
							}
							else
							{
								maxAsteroids = LevelsData.Levels[currentLevel, 2];
								maxMeteors = LevelsData.Levels[currentLevel, 3];
								maxYellowBears = LevelsData.Levels[currentLevel, 4];
								maxGreenBears = LevelsData.Levels[currentLevel, 5];
								gameState = GameState.LevelUp;
							}
						}
						else if (elapsedTime > GameConstants.PickupCoolDownTime)
						{
							elapsedTime = 0;
							if (usedPickups < LevelsData.Levels[currentLevel, 6])
							{
								CheckForPickupEvent();
							}
						}
					}
					break;
				case (GameState.LevelUp):
					// destroy all remaining enemies and projectiles
					DestroyGameObjects("enemies+projectiles");
					UpdateGameObjects(gameTime, "explosions+pickups");
					elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
					waitedTime = (int)(elapsedTime / 1000);
					if (waitedTime > 15)
					{
						elapsedTime = 0;
						waitedTime = 0;
						spawnAllowed = true;
						ResetStats();
						if (LevelsData.Levels[currentLevel, 1] == 0)
						{
							gameState = GameState.Fighting;
						}
						else
						{
							bossFight = true;
							// spawn a new boss
							switch (currentLevel)
							{
								case (5):
									SpawnBoss(BossType.Boss1);
									break;
								case (10):
									// TODO: replace per proper type Boss2
                                    SpawnBoss(BossType.Boss1);
									break;
								case (15):
									// TODO: replace per proper type Boss3
                                    SpawnBoss(BossType.Boss1);
									break;
								case (20):
									// TODO: replace per proper type Boss4
                                    SpawnBoss(BossType.Boss1);
									break;
								case (25):
									// TODO: replace per proper type Boss5
                                    SpawnBoss(BossType.Boss1);
									break;
								case (30):
									// TODO: replace per proper type FinalBoss
                                    SpawnBoss(BossType.Boss1);
									break;	
							}
							gameState = GameState.BossFight;
						}
                        SetBackgroundMusic("start");
					}
					break;
				case(GameState.Fighting):
					// check for pause
					if (keyboard.IsKeyDown(Keys.P))
					{
						pKeyPressed = true;
					}
					if (pKeyPressed && keyboard.IsKeyUp(Keys.P))
					{
						pKeyPressed = false;
                        SetBackgroundMusic("stop");
						gameState = GameState.Paused;
					}
					else
					{
						elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
						UpdateGameObjects(gameTime, "all");
						CheckAllCollisions(gameTime);
                        UpdateEntitiesCounters();
						if (tomcat.Health <= 0)
						{
							lives -= 1;
							elapsedTime = 0;
							f14Dead = true;
							explosions.Add(new Explosion(ExplosionType.TomcatDeath, sprites[34], tomcat.Location));
							f14Death.Play(volume: GameConstants.DeathVolume, pitch: 0.0f, pan: 0.0f);
                            SetBackgroundMusic("stop");
							if (lives < 0)
								gameState = GameState.GameOver;
							else
								gameState = GameState.Death;
						}
						else if (elapsedTime > GameConstants.PickupCoolDownTime)
						{
							elapsedTime = 0;
							if (usedPickups < LevelsData.Levels[currentLevel, 6])
							{
								CheckForPickupEvent();
							}
						}
						if (levelUp)
						{
							levelUp = false;
							elapsedTime = 0;
							currentLevel++;
							maxAsteroids = LevelsData.Levels[currentLevel, 2];
							maxMeteors = LevelsData.Levels[currentLevel, 3];
							maxYellowBears = LevelsData.Levels[currentLevel, 4];
							maxGreenBears = LevelsData.Levels[currentLevel, 5];
							spawnAllowed = false;
                            SetBackgroundMusic("stop");
							gameState = GameState.LevelUp;
						}
					}                             
					break;
				case (GameState.Death):
					UpdateGameObjects(gameTime, "explosions");
					deathCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
					if (deathCooldownMilliseconds > GameConstants.F14DeathCooldownMilliseconds)
					{
						deathCooldownMilliseconds = 0;
						SpawnTomcat();
						f14Dead = false;
						gameState = GameState.StartUp;
						UpdateEntitiesCounters();
					}
					break;
				case (GameState.GameOver):
					// destroy projectiles
					DestroyGameObjects("projectiles");
					// update explosions
					UpdateGameObjects(gameTime, "explosions");
					// check if ESC key is pressed
					if (keyboard.IsKeyDown(Keys.Escape))
					{
						escKeyPressed = true;
					}
					if (escKeyPressed && keyboard.IsKeyUp(Keys.Escape))
					{
						escKeyPressed = false;
						Exit();
					}
					break;
			}
			// this block executes on all game states
			// clean out inactive teddy bears and add new ones as necessary
			for (int i = bears.Count - 1; i >= 0; i--)
			{
				if (!bears[i].Active)
				{
					if (bears[i].Type == BearType.YellowTrooper) currentYellowBears -= 1;
					else currentGreenBears -= 1;
					if (currentYellowBears < 0) currentYellowBears = 0;
					if (currentGreenBears < 0) currentGreenBears = 0;
					bears.RemoveAt(i);
				}
			}
			// populate world according to max allowed per time
			while (currentYellowBears < maxYellowBears && spawnAllowed)
			{
				SpawnBear(BearType.YellowTrooper);
			}
			while (currentGreenBears < maxGreenBears && spawnAllowed )
			{
				SpawnBear(BearType.GreenTrooper);
			}
			while (currentAsteroids < maxAsteroids && spawnAllowed)
			{
				SpawnRock(RockType.Asteroid);
			}
			while (currentMeteors < maxMeteors && spawnAllowed)
			{
				SpawnRock(RockType.Meteor);
			}

			// clean out inactive projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--)
			{
				if (!projectiles[i].Active) projectiles.RemoveAt(i);
			}
			// clean out inactive rocks
			for (int i = rocks.Count - 1; i >= 0; i--)
			{
				if (!rocks[i].Active)
				{
					if (rocks[i].Type == RockType.Asteroid) currentAsteroids -= 1;
					else currentMeteors -= 1;
					if (currentAsteroids < 0) currentAsteroids = 0;
					if (currentMeteors < 0) currentMeteors = 0;
					rocks.RemoveAt(i);
				}
			}
			// clean out finished explosions
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				if (explosions[i].Finished) explosions.RemoveAt(i);
			}
			// clean out finished signs
			for (int i = signs.Count - 1; i >= 0; i--)
			{
				if (signs[i].Finished) signs.RemoveAt(i);
			}
			// clean out inactive pickups
			for (int i = pickups.Count - 1; i >= 0; i--)
			{
				if (!pickups[i].Active) pickups.RemoveAt(i);
			}
			// update the game world
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);

			spriteBatch.Begin();
			// draw the background space
			DrawBackgroundSpace();
			// visual debug support
			// DrawBorder(tomcat.CollisionRectangle, 1, Color.Red);
			// DrawBorder(new Rectangle(GameConstants.SpawnBorderSize,GameConstants.SpawnBorderSize,
			//                         GameConstants.DisplayWidth - 2*GameConstants.SpawnBorderSize,
			//                         GameConstants.DisplayHeight - 2*GameConstants.SpawnBorderSize), 1, Color.Red);
			// DrawBorder(new Rectangle(tomcat.CollisionRectangle.X - GameConstants.SpawnBorderSize,
										//			   tomcat.CollisionRectangle.Y - GameConstants.SpawnBorderSize,
										//			   tomcat.CollisionRectangle.Width + 2*GameConstants.SpawnBorderSize,
									 //tomcat.CollisionRectangle.Height + 2*GameConstants.SpawnBorderSize), 1, Color.Red);

			// use FSM to control game states
			switch (gameState)
			{
				// create the story behind the game
				case (GameState.Introduction):
					if (waitedTime < 4)
					{
						// show WB studios logo
						spriteBatch.Draw(sprites[99], new Rectangle(0,0, GameConstants.DisplayWidth, GameConstants.DisplayHeight), Color.WhiteSmoke);
					}
					else
					{
						// tell the story
						if (introTitleLocation < GameConstants.DisplayHeight - GameConstants.DisplayOffset * 2)
							DrawString(fontIomanoid70B, center, GameConstants.IntroTitleString, 0, (int)introTitleLocation, GameConstants.IntroTitleColor);
						for (int i = 0; i < (GameConstants.IntroBodySize - 1); i++)
						{
							if (introBodyLocations[i] < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
								DrawString(fontArial20, left, GameConstants.IntroTextBody[i], 0, (int)introBodyLocations[i], GameConstants.IntroTextColor);
						}
						if (introBodyLocations[GameConstants.IntroBodySize - 1] < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
							DrawString(fontArial20, left, GameConstants.IntroTextBody[GameConstants.IntroBodySize - 1], 0,
									   (int)introBodyLocations[GameConstants.IntroBodySize - 1], GameConstants.IntroCallColor);
					}
					// help message
					helpMessage = GameConstants.IntroMessage;
					break;
					// initial or after death stage with a count down
				case (GameState.StartUp):
					// count down
					if (bossFight)
					{
                   		DrawString(fontPerfectDark30B, center, GameConstants.StartUpMessagesBoss[waitedTime], 0, GameConstants.DisplayHeight / 2, Color.LimeGreen);
					}
					else
					{
                   		DrawString(fontPerfectDark30B, center, GameConstants.StartUpMessages[waitedTime], 0, GameConstants.DisplayHeight / 2, Color.LimeGreen);
					}
					helpMessage = GameConstants.StartUpHelp;
					break;
					// main stage for normal levels
				case (GameState.Fighting):
					// draw game objects
					DrawAllGameObjects();
					//help message
					helpMessage = GameConstants.FightingHelp;
					break;
					// between levels stage
				case (GameState.LevelUp):
					// draw game objects
					DrawAllGameObjects();
					// show stats
					if (waitedTime > 0 && waitedTime < 11)
					{
						DrawString(fontArial30B, center, GameConstants.StatsStrings[0], 0, 2 * GameConstants.DisplayOffset, Color.Purple);

						if (waitedTime > 2 && waitedTime < 7)
						{
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[1] + firedLaserShots.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 5, Color.Pink);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[2] + firedPlasmShots.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 6, Color.Pink);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[3] + destroyedAsteroids.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 7, Color.LightBlue);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[4] + destroyedMeteors.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 8, Color.LightBlue);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[5] + killedYellowBears.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 9, Color.LightBlue);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[6] + killedGreenBears.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 10, Color.LightBlue);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[7] + killedEnemyShots.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset * 11, Color.LightBlue);
							DrawString(fontArial20, 'x', GameConstants.StatsStrings[8] + hitsOnBoss.ToString(), 7 * GameConstants.DisplayOffset,
									   GameConstants.DisplayOffset* 12, Color.LightBlue);
						}
						else if (waitedTime > 6 && waitedTime < 11)
						{
							DrawString(fontArial30B, center, GameConstants.StatsStrings[9] + overallAccuracy.ToString("P0"), 0,
									   GameConstants.DisplayHeight / 2, Color.Yellow);
						}
					}
					else if (waitedTime > 10 && waitedTime < 16)
					{
						if (LevelsData.Levels[currentLevel, 1] == 0)
						{
							DrawString(fontPerfectDark30B, center, GameConstants.LevelUpMessages[waitedTime - 11], 0, GameConstants.DisplayHeight / 2, Color.Yellow);
						}
						else
						{
                            DrawString(fontPerfectDark30B, center, GameConstants.LevelUpMessagesBoss[waitedTime - 11], 0, GameConstants.DisplayHeight / 2, Color.Yellow);
						}
					}
					//help message
					helpMessage = GameConstants.LevelUpHelp;
					break;
					// level with a boss
				case(GameState.BossFight):
					// draw game objects
					DrawAllGameObjects();
					//help message
					helpMessage = GameConstants.BossFightHelp;
					break;
					// everytime the game is paused
				case (GameState.Paused):
					// draw game objects
					DrawAllGameObjects();
					// help message
					helpMessage = GameConstants.PausedHelp;
					break;
					// when the player dies
				case (GameState.Death):
					// draw game objects
					DrawAllGameObjects();
					// help message
					helpMessage = GameConstants.DeathHelp;
					break;
					// final state
				case (GameState.GameOver):
					// draw game objects
					DrawAllGameObjects();
					DrawString(fontArial70B, center, GameConstants.GameOverMessage, 0, GameConstants.DisplayHeight / 2, Color.White);
					// help message
					helpMessage = GameConstants.GameOverHelp;
					break;
				case (GameState.Credits):
					if (waitedTime < 6)
					{
						// draw game objects
						DrawAllGameObjects();
					}
					else
					{
						if (creditsTitleLocation < GameConstants.DisplayHeight - GameConstants.DisplayOffset * 2)
							DrawString(fontIomanoid70B, center, GameConstants.CreditsTitleString, 0, (int)creditsTitleLocation, GameConstants.CreditsTitleColor);
						for (int i = 0; i < (GameConstants.CreditsBodySize); i++)
						{
							if (creditsBodyLocations[i] < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
								DrawString(fontArial20, center, GameConstants.CreditsTextBody[i], 0, (int)creditsBodyLocations[i], GameConstants.CreditsTextColor);
						}
					}
					// help message
					helpMessage = GameConstants.CreditsMessage;
					break;
			}
			// draw display area
			DrawDisplayArea();
			// update counters
			if (gameState != GameState.Introduction && gameState != GameState.Credits)
				UpdateDisplayArea();
			// help message
			DrawString(fontIomanoid20B, center, helpMessage, 0, GameConstants.HelpRow, GameConstants.HelpMessageColor);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		#region Public methods

		/// <summary>
		/// Gets the projectile sprite for the given projectile type
		/// </summary>
		/// <param name="type">the projectile type</param>
		/// <returns>the projectile sprite for the type</returns>
		public static Texture2D GetProjectileSprite(ProjectileType type)
		{
			// return correct projectile sprite based on projectile type
			switch (type) {
				case ProjectileType.Plasm:
					return sprites[1];
				case ProjectileType.Laser:
					return sprites[2];
				case ProjectileType.Teddy:
					return sprites[0];
				case ProjectileType.Boss1:
					return sprites[10];
				default:
					return sprites[0];
			}
		}

		/// <summary>
		/// Adds the given projectile to the game
		/// </summary>
		/// <param name="projectile">the projectile to add</param>
		public static void AddProjectile(Projectile projectile)
		{
			projectiles.Add(projectile);
		}

		/// <summary>
		/// Gets the level.
		/// </summary>
		/// <returns>The level.</returns>
		public static int GetLevel()
		{
			return currentLevel;
		}

		/// <summary>
		/// Gets the direction2 tomcat.
		/// </summary>
		/// <returns>The direction2 tomcat.</returns>
		/// <param name="origin">Origin.</param>
		public static Vector2 GetDirection2Tomcat(Vector2 origin)
		{
			Vector2 direction = Vector2.Zero;

			direction.X = tomcat.Location.X - origin.X;
			direction.Y = tomcat.Location.Y - origin.Y;
			
			direction.Normalize();

			return direction;
		}

		/// <summary>
		/// Updates the stats.
		/// </summary>
		/// <param name="type">Type.</param>
		public static void UpdateStats(StatsType type, int amount)
		{
			int totalFired = 0;
			switch (type)
			{
				case(StatsType.firedLaserShots): 
					firedLaserShots += amount;
					break;
				case(StatsType.firedPlasmShots):
					firedPlasmShots += amount;
					break;
				case(StatsType.killedYellowBears):
					killedYellowBears += amount;
					break;
				case (StatsType.killedGreenBears):
					killedGreenBears += amount;
					break;
				case (StatsType.killedEnemyShots):
					killedEnemyShots += amount;
					break;
				case (StatsType.destroyedAsteroids):
					destroyedAsteroids += amount;
					break;
				case (StatsType.destroyedMeteors):
					destroyedMeteors += amount;
					break;
				case (StatsType.enemyFiredShots):
					enemyFiredShots += amount;
					break;
				case(StatsType.hitsOnBoss):
					hitsOnBoss += amount;
					break;
			}
			totalFired = firedLaserShots + firedPlasmShots;
			if (totalFired > 0)
			{
				overallAccuracy = (float)(killedYellowBears + killedGreenBears + destroyedAsteroids + destroyedMeteors + killedEnemyShots + hitsOnBoss) / totalFired;
			}
		}

		/// <summary>
		/// Resets the stats.
		/// </summary>
		public static void ResetStats()
		{
			firedLaserShots = 0;
			firedPlasmShots = 0;
			killedYellowBears = 0;
			killedGreenBears = 0;
			killedEnemyShots = 0;
			destroyedAsteroids = 0;
			destroyedMeteors = 0;
			enemyFiredShots = 0;
			hitsOnBoss = 0;
		}

		/// <summary>
		/// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
		/// of the specified color.
		///
		/// By Sean Colombo, from http://bluelinegamestudios.com/blog
		/// </summary>
		/// <param name="rectangleToDraw"></param>
		/// <param name="thicknessOfBorder"></param>
		public void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
		{
			// Draw top line
			spriteBatch.Draw(whiteRectangle, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

			// Draw left line
			spriteBatch.Draw(whiteRectangle, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

			// Draw right line
			spriteBatch.Draw(whiteRectangle, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
											rectangleToDraw.Y,
											thicknessOfBorder,
											rectangleToDraw.Height), borderColor);
			// Draw bottom line
			spriteBatch.Draw(whiteRectangle, new Rectangle(rectangleToDraw.X,
											rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
											rectangleToDraw.Width,
											thicknessOfBorder), borderColor);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Sets the background music.
		/// </summary>
		/// <param name="command">Command.</param>
		private void SetBackgroundMusic(string command)
		{
			if (bossFight)
			{
				switch (boss.Type)
				{
					case (BossType.Boss1):
						backgroundMusic = boss1Music;
						break;
					case (BossType.Boss2):
						backgroundMusic = boss2Music;
						break;
					case (BossType.Boss3):
						backgroundMusic = boss2Music;
						// TODO: replace it per bossStageMusic = boss3StageMusic;
						break;
					case (BossType.Boss4):
						backgroundMusic = boss2Music;
						// TODO: replace it per bossStageMusic = boss3StageMusic;
						break;
					case (BossType.Boss5):
						backgroundMusic = boss2Music;
						// TODO: replace it per bossStageMusic = boss3StageMusic;
						break;
					case (BossType.FinalBoss):
						backgroundMusic = boss2Music;
						// TODO: replace it per bossStageMusic = boss3StageMusic;
						break;
				}
			}
			else
			{
				backgroundMusic = stage1Music;
			}
			if (command == "start")
			{
				backgroundMusic.Play();
			}
			else if (command == "stop")
			{
				backgroundMusic.Stop();
			}
		}

		/// <summary>
		/// Updates the score.
		/// </summary>
		/// <param name="amount">Amount.</param>
		private void UpdateScore(int amount)
		{
			score += amount;
			globalScore += amount;
		}


		/// <summary>
		/// Checks for pickup event.
		/// </summary>
		private void CheckForPickupEvent()
		{
			int orb = RandomNumberGenerator.Next(4);
			// if health is low, send an orb
			if (tomcat.Health < 50)
			{
				pickups.Add(new Pickup(PickupType.GreenOrb, sprites[58]));
				usedPickups++;
			}
			else if (tomcat.Shield < 50) {
				// decide wether to help or not
				switch(orb) {
					case (1):
						pickups.Add(new Pickup(PickupType.BlueOrb, sprites[57]));
						usedPickups++;
						break;
					case (2):
						pickups.Add(new Pickup(PickupType.RedOrb, sprites[59]));
						usedPickups++;
						break;
					case (3):
						pickups.Add(new Pickup(PickupType.GoldOrb, sprites[60]));
						usedPickups++;
						break;
				}
			}
			else
			{
				// decide wether to help or not
				switch (orb)
				{
					case (1):
						pickups.Add(new Pickup(PickupType.RedOrb, sprites[59]));
						usedPickups++;
						break;
					case (2):
						pickups.Add(new Pickup(PickupType.GoldOrb, sprites[60]));
						usedPickups++;
						break;
				}
			}
		}

		/// <summary>
		/// Updates all game objects.
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		private void UpdateGameObjects(GameTime gameTime, String scope)
		{
			if (scope.ToLower() == "all")
			{
				// pass game elapsed time and mouse state
				tomcat.Update(gameTime, mouse, keyboard);
				// update other game objects
				foreach (TeddyBear bear in bears)
				{
					if (bear.Active) bear.Update(gameTime);
				}
				foreach (Projectile projectile in projectiles)
				{
					if (projectile.Active) projectile.Update(gameTime);
				}
				foreach (Rock rock in rocks)
				{
					if (rock.Active) rock.Update(gameTime);
				}
				foreach (Explosion explosion in explosions)
				{
					explosion.Update(gameTime);
				}
				foreach (Sign sign in signs)
				{
					sign.Update(gameTime);
				}
				foreach (Pickup pickup in pickups)
				{
					pickup.Update(gameTime);
				}
			}
			else if (scope.ToLower() == "explosions+pickups")
			{
				foreach (Explosion explosion in explosions)
				{
					explosion.Update(gameTime);
				}
				foreach (Sign sign in signs)
				{
					sign.Update(gameTime);
				}
				foreach (Pickup pickup in pickups)
				{
					pickup.Update(gameTime);
				}
			}
			else if (scope.ToLower() == "boss")
			{
				// update main objects
				tomcat.Update(gameTime, mouse, keyboard);
				boss.Update(gameTime);
				// update other game objects
				foreach (TeddyBear bear in bears)
				{
					if (bear.Active) bear.Update(gameTime);
				}
				foreach (Projectile projectile in projectiles)
				{
					if (projectile.Active) projectile.Update(gameTime);
				}
				foreach (Pickup pickup in pickups)
				{
					pickup.Update(gameTime);
				}
				foreach (Explosion explosion in explosions)
				{
					explosion.Update(gameTime);
				}
				foreach (Sign sign in signs)
				{
					sign.Update(gameTime);
				}
			}
			else if (scope.ToLower() == "explosions")
			{
				foreach (Explosion explosion in explosions)
				{
					explosion.Update(gameTime);
				}
				foreach (Sign sign in signs)
				{
					sign.Update(gameTime);
				}
			}
		}

		private void DestroyGameObjects(String scope)
		{
			if (scope.ToLower() == "enemies+projectiles")
			{
				foreach (TeddyBear bear in bears)
				{
					if (bear.Active)
					{
						bear.Active = false;
						explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, sprites[33],
														 bear.CollisionRectangle.Center));
						explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
					}
				}
				foreach (Rock rock in rocks)
				{
					if (rock.Active)
					{
						rock.Active = false;
						explosions.Add(new Explosion(ExplosionType.RockDeath, sprites[36],
														 rock.CollisionRectangle.Center));
						explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
					}
				}
				foreach (Projectile projectile in projectiles)
				{
					if (projectile.Active)
					{
						projectile.Active = false;
						if (projectile.Type == ProjectileType.Teddy)
						{
							explosions.Add(new Explosion(ExplosionType.ProjectileHit, sprites[35],
														 projectile.CollisionRectangle.Center.X,
														 projectile.CollisionRectangle.Center.Y));
							explosionSounds[1].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
						}
					}
				}
			}
			else if (scope.ToLower() == "projectiles")
			{
				foreach (Projectile projectile in projectiles)
				{
					if (projectile.Active)
					{
						projectile.Active = false;
						if (projectile.Type == ProjectileType.Teddy)
						{
							explosions.Add(new Explosion(ExplosionType.ProjectileHit, sprites[35],
														 projectile.CollisionRectangle.Center.X,
														 projectile.CollisionRectangle.Center.Y));
							explosionSounds[1].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
						}
					}
				}
			}
		}

		/// <summary>
		/// Checks all collisions.
		/// </summary>
		private void CheckAllCollisions(GameTime gameTime)
		{
			// check and resolve collisions between projectiles
			foreach (Projectile projectile1 in projectiles)
			{
				if (projectile1.Active)
				{
					foreach (Projectile projectile2 in projectiles)
					{
						if (projectile2.Active && !projectile2.Equals(projectile1) &&
							projectile1.Type != projectile2.Type)
						{
							if (CollisionUtils.DetectCollision(projectile2.CollisionRectangle, projectile2.TextureData, 
							                    projectile1.CollisionRectangle, projectile1.TextureData, ref collisionPoint) &&
								(projectile1.Type == ProjectileType.Teddy || projectile2.Type == ProjectileType.Teddy))
							{
								projectile1.Active = false;
								projectile2.Active = false;
								explosions.Add(new Explosion(ExplosionType.ProjectileHit, sprites[35],
								                             collisionPoint.X,collisionPoint.Y));
								signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 9].ToString(),
								                   GameConstants.SignDefaultColor, collisionPoint));
								explosionSounds[1].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
								// give some points since it hit bear projectile
								UpdateScore(LevelsData.Levels[currentLevel, 9]);
								UpdateStats(StatsType.killedEnemyShots, 1);
							}
						}
					}
				}
			}
			// check and resolve collisions between teddy bears
			// if they collide, they will bounce from each other
			for (int i = 0; i < bears.Count; i++)
			{
				for (int j = i + 1; j < bears.Count; j++)
				{
					if (bears[i].Active && bears[j].Active)
					{
						collisionInfo = CollisionUtils.CheckCollision(gameTime.ElapsedGameTime.Milliseconds,
																  GameConstants.WindowWidth,
																  GameConstants.WindowHeight,
																  bears[i].Velocity,
																  bears[i].DrawRectangle,
																  bears[j].Velocity,
																  bears[j].DrawRectangle);
						if (collisionInfo != null)
						{
							if (collisionInfo.FirstOutOfBounds)
							{
								bears[i].Active = false;
							}
							else {
								bears[i].Velocity = collisionInfo.FirstVelocity;
								bears[i].DrawRectangle = collisionInfo.FirstDrawRectangle;
							}
							if (collisionInfo.SecondOutOfBounds)
							{
								bears[j].Active = false;
							}
							else {
								bears[j].Velocity = collisionInfo.SecondVelocity;
								bears[j].DrawRectangle = collisionInfo.SecondDrawRectangle;
							}
						}
					}
				}
			}
			// check and resolve collisions between teddy bears and rocks
			foreach (TeddyBear bear in bears)
			{
				if (bear.Active)
				{
					foreach (Rock rock in rocks)
					{
						if (rock.Active)
						{
							if (CollisionUtils.DetectCollision(rock.CollisionRectangle, rock.TextureData,
							                    bear.CollisionRectangle, bear.TextureData, ref collisionPoint))
							{
								bear.Active = false;
								rock.Active = false;
								explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, sprites[33],
															 bear.CollisionRectangle.Center));
								explosions.Add(new Explosion(ExplosionType.RockDeath, sprites[36],
															 rock.CollisionRectangle.Center));
								explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
								explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
							}
						}
					}
				}
			}
			// check and resolve collisions between tomcat and teddy bears
			foreach (TeddyBear bear in bears)
			{
				if (bear.Active)
				{
					if (CollisionUtils.DetectCollision(tomcat.CollisionRectangle, tomcat.TextureData,
					                    bear.CollisionRectangle, bear.TextureData, ref collisionPoint))
					{
						bear.Active = false;
						if (bear.Type == BearType.YellowTrooper)
						{
							tomcat.TakeDamage(LevelsData.Levels[currentLevel, 14]);
							signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 7].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
							UpdateScore(LevelsData.Levels[currentLevel, 7]);
							UpdateStats(StatsType.killedYellowBears, 1);
						}
						else {
							tomcat.TakeDamage(LevelsData.Levels[currentLevel, 15]);
							signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 8].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
							UpdateScore(LevelsData.Levels[currentLevel, 8]);
							UpdateStats(StatsType.killedGreenBears, 1);
						}
						explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, sprites[33],
													 bear.CollisionRectangle.Center));
						f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
						explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
					}
				}
			}
			// check and resolve collisions between tomcat and pickups
			foreach (Pickup pickup in pickups)
			{
				if (pickup.Active)
				{
					if (CollisionUtils.DetectCollision(tomcat.CollisionRectangle, tomcat.TextureData,
					                    pickup.CollisionRectangle, pickup.TextureData, ref collisionPoint))
					{
						pickup.Active = false;
						// TODO: Add a sign when tomcat grabs a pickup
						if (pickup.Type == PickupType.RedOrb)
						{
							tomcat.PowerItUp(GameConstants.F14PowerAid);
							signs.Add(new Sign(SignType.Colorful, fontIomanoid20B, "Power UP", GameConstants.SignDefaultColor, pickup.Center));
							pickupSounds[0].Play();
						}
						else if (pickup.Type == PickupType.BlueOrb)
						{
							tomcat.RaiseShield(GameConstants.F14ShieldAid);
							signs.Add(new Sign(SignType.Colorful, fontIomanoid20B, "Shield", GameConstants.SignDefaultColor, pickup.Center));
							pickupSounds[1].Play();

						}
						else if (pickup.Type == PickupType.GreenOrb)
						{
							tomcat.FixIt(GameConstants.F14HealthAid);
							signs.Add(new Sign(SignType.Colorful, fontIomanoid20B, "Health", GameConstants.SignDefaultColor, pickup.Center));
							pickupSounds[2].Play();
						}
						else if (pickup.Type == PickupType.GoldOrb)
						{
							UpdateScore(GameConstants.F14ScoreBonus);
							signs.Add(new Sign(SignType.Colorful, fontIomanoid20B, "Money", GameConstants.SignDefaultColor, pickup.Center));
							pickupSounds[3].Play();
						}
					}
				}
			}
			// check and resolve collisions between tomcat and rocks
			foreach (Rock rock in rocks)
			{
				if (rock.Active)
				{
					if (CollisionUtils.DetectCollision(tomcat.CollisionRectangle, tomcat.TextureData,
					                    rock.CollisionRectangle, rock.TextureData, ref collisionPoint))
					{
						rock.Active = false;
						if (rock.Type == RockType.Asteroid)
						{
							tomcat.TakeDamage(LevelsData.Levels[currentLevel,16]);
							signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 10].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
							UpdateScore(LevelsData.Levels[currentLevel, 10]);
							UpdateStats(StatsType.destroyedAsteroids, 1);
						}
						else {
							tomcat.TakeDamage(LevelsData.Levels[currentLevel, 17]);
							signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 11].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
							UpdateScore(LevelsData.Levels[currentLevel, 11]);
							UpdateStats(StatsType.destroyedMeteors, 1);
						}
						explosions.Add(new Explosion(ExplosionType.RockDeath, sprites[36],
													 rock.CollisionRectangle.Center));
						f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
						explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
					}
				}
			}
			// check and resolve collisions between tomcat and projectiles
			foreach (Projectile projectile in projectiles)
			{
				if (projectile.Active && (projectile.Type == ProjectileType.Teddy ||
				                          projectile.Type == ProjectileType.Boss1 ||
				                          projectile.Type == ProjectileType.Boss2 ||
				                          projectile.Type == ProjectileType.Boss3 ||
				                          projectile.Type == ProjectileType.Boss4 ||
				                          projectile.Type == ProjectileType.Boss5 ||
				                          projectile.Type == ProjectileType.FinalBoss))
				{
					if (CollisionUtils.DetectCollision(tomcat.CollisionRectangle, tomcat.TextureData,
					                    projectile.CollisionRectangle, projectile.TextureData, ref collisionPoint))
					{
						// bear projectile
						if (projectile.Type == ProjectileType.Teddy)
						{
							tomcat.TakeDamage(LevelsData.Levels[currentLevel, 12]);
							explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, sprites[33],
														 collisionPoint.X, collisionPoint.Y));
							f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
						}
						// boss projectile
						else
						{
							tomcat.TakeDamage(LevelsData.Levels[currentLevel, 13]);
							explosions.Add(new Explosion(ExplosionType.TomcatHit, sprites[38],
														 collisionPoint.X, collisionPoint.Y));
							f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
						}
						projectile.Active = false;
					}
				}
			}
			// check and resolve collisions between boss and projectiles and tomcat
			if (bossFight)
			{
				foreach (Projectile projectile in projectiles)
				{
					if (projectile.Active && (projectile.Type == ProjectileType.Laser
											  || projectile.Type == ProjectileType.Plasm))
					{
						if (CollisionUtils.DetectCollision(boss.CollisionRectangle, boss.TextureData,
						                    projectile.CollisionRectangle, projectile.TextureData, ref collisionPoint))
						{
							if (projectile.Type == ProjectileType.Laser)
							{
								boss.TakeDamage(GameConstants.LaserProjectileDamage + tomcat.PowerUp);
							}
							else
							{
								boss.TakeDamage(GameConstants.PlasmProjectileDamage + tomcat.PowerUp);

							}
							UpdateStats(StatsType.hitsOnBoss, 1);
							explosions.Add(new Explosion(ExplosionType.BossHit, sprites[37],
														 collisionPoint.X, collisionPoint.Y));
							bossSounds[2].Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
							projectile.Active = false;
						}
					}
				}
				if (boss.Active && tomcat.Health>0)
				{
					if (CollisionUtils.DetectCollision(tomcat.CollisionRectangle, tomcat.TextureData,
					                    boss.CollisionRectangle, boss.TextureData, ref collisionPoint))
					{
						tomcat.TakeDamage(LevelsData.Levels[currentLevel, 18]);
						tomcat.Repulse(collisionPoint, boss.Location);
						boss.TakeDamage(tomcat.Shield);
						explosions.Add(new Explosion(ExplosionType.BossClash, sprites[40],
						                             collisionPoint.X, collisionPoint.Y));
						f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
						explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
					}
				}
			}

			// check and resolve collisions between teddy bears and projectiles
			foreach (TeddyBear bear in bears)
			{
				if (bear.Active)
				{
					foreach (Projectile projectile in projectiles)
					{
						if (projectile.Active && (projectile.Type == ProjectileType.Laser ||
												  projectile.Type == ProjectileType.Plasm))
						{
							if (CollisionUtils.DetectCollision(bear.CollisionRectangle, bear.TextureData,
							                    projectile.CollisionRectangle, projectile.TextureData, ref collisionPoint))
							{
								if (projectile.Type == ProjectileType.Laser)
								{
									bear.TakeDamage(GameConstants.LaserProjectileDamage + tomcat.PowerUp);
									if (bear.Health == 0)
									{
										bear.Active = false;
										if (bear.Type == BearType.YellowTrooper)
										{
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 7].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 7]);
											UpdateStats(StatsType.killedYellowBears, 1);
										}
										else 
										{
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 8].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 8]);
											UpdateStats(StatsType.killedGreenBears, 1);
										}
									}
									projectile.Active = false;
								}
								else
								{
									bear.TakeDamage(GameConstants.PlasmProjectileDamage + tomcat.PowerUp);
									if (bear.Health == 0)
									{
										bear.Active = false;
										if (bear.Type == BearType.YellowTrooper)
										{
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 7].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 7]);
											UpdateStats(StatsType.killedYellowBears, 1);
										}
										else {
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 8].ToString(),
								                   GameConstants.SignDefaultColor, bear.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 8]);
											UpdateStats(StatsType.killedGreenBears, 1);
										}
									}
								}
								explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, sprites[33], bear.DrawRectangle.Center));
								explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
							}
						}
					}
				}
			}
			// check and resolve collisions between rocks and projectiles
			foreach (Rock rock in rocks)
			{
				if (rock.Active)
				{
					foreach (Projectile projectile in projectiles)
					{
						if (projectile.Active)
						{
							if (CollisionUtils.DetectCollision(rock.CollisionRectangle, rock.TextureData,
							                    projectile.CollisionRectangle, projectile.TextureData, ref collisionPoint))
							{
								if (projectile.Type == ProjectileType.Laser)
								{
									rock.TakeDamage(GameConstants.LaserProjectileDamage + tomcat.PowerUp);
									if (rock.Health == 0)
									{
										rock.Active = false;
										if (rock.Type == RockType.Asteroid)
										{
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 10].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 10]);
											UpdateStats(StatsType.destroyedAsteroids, 1);
										}
										else {
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 11].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 11]);
											UpdateStats(StatsType.destroyedMeteors , 1);
										}
									}
									projectile.Active = false;
								}
								else if (projectile.Type == ProjectileType.Plasm)
								{
									rock.TakeDamage(GameConstants.PlasmProjectileDamage + tomcat.PowerUp);
									if (rock.Health == 0)
									{
										rock.Active = false;
										if (rock.Type == RockType.Asteroid)
										{
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 10].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 10]);
											UpdateStats(StatsType.destroyedAsteroids, 1);
										}
										else {
											signs.Add(new Sign(SignType.Emerging, fontIomanoid20B, LevelsData.Levels[currentLevel, 11].ToString(),
								                   GameConstants.SignDefaultColor, rock.Center));
											UpdateScore(LevelsData.Levels[currentLevel, 11]);
                                            UpdateStats(StatsType.destroyedMeteors, 1);
										}
									}
								}
								else
								{
									rock.TakeDamage(LevelsData.Levels[currentLevel, 12]);
									if (rock.Health == 0)
									{
										rock.Active = false;
									}
									projectile.Active = false;
								}
								explosions.Add(new Explosion(ExplosionType.RockDeath, sprites[36], rock.DrawRectangle.Center));
								explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
							}
						}
					}
				}
			}
		}
					
		/// <summary>
		/// Draws all game objects.
		/// </summary>
		private void DrawAllGameObjects()
		{
			// draw game objects
			if (!f14Dead)
			{
				tomcat.Draw(spriteBatch);
			}
			if (bossFight)
			{
				if (boss.Active) boss.Draw(spriteBatch);
			}
			foreach (TeddyBear bear in bears)
			{
				if (bear.Active) bear.Draw(spriteBatch);
			}
			foreach (Rock rock in rocks)
			{
				if (rock.Active) rock.Draw(spriteBatch);
			}
			foreach (Projectile projectile in projectiles)
			{
				if (projectile.Active) projectile.Draw(spriteBatch);
			}
			foreach (Explosion explosion in explosions)
			{
				explosion.Draw(spriteBatch);
			}
			foreach (Pickup pickup in pickups)
			{
				pickup.Draw(spriteBatch);
			}
			foreach (Sign sign in signs)
			{
				sign.Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Draws the display area.
		/// </summary>
		private void DrawDisplayArea()
		{
			spriteBatch.Draw(whiteRectangle, new Rectangle(0, GameConstants.DisplayHeight + 1,
															   GameConstants.DisplayWidth, GameConstants.WindowWidth),
								 GameConstants.DisplayColor);
			spriteBatch.DrawString(fontArial20, GameConstants.ScorePrefix, GameConstants.ScorePrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.HealthPrefix, GameConstants.HealthPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.ShieldPrefix, GameConstants.ShieldPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.LivesPrefix , GameConstants.LivesPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.LevelPrefix, GameConstants.LevelPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.WeaponPrefix, GameConstants.WeaponPrefixLocation, GameConstants.StringColor);
			if (bossFight)
			{
				spriteBatch.DrawString(fontArial20, GameConstants.BossPrefix, GameConstants.NextPrefixLocation, GameConstants.StringColor);
			}
			else
			{
				spriteBatch.DrawString(fontArial20, GameConstants.NextPrefix, GameConstants.NextPrefixLocation, GameConstants.StringColor);
			}
		}

		/// <summary>
		/// Draws the space background.
		/// </summary>
		private void DrawBackgroundSpace()
		{
			// TODO: implement random background sprites
			spriteBatch.Draw(sprites[70], new Rectangle(0,0,GameConstants.DisplayWidth,GameConstants.DisplayHeight), Color.WhiteSmoke);
		}

		/// <summary>
		/// Draws the string.
		/// </summary>
		/// <param name="font">Font.</param>
		/// <param name="alignment">Alignment.</param>
		/// <param name="text">Text.</param>
		/// <param name="Y">Row.</param>
		/// <param name="color">Color.</param>
		private void DrawString(SpriteFont font, char alignment, String text, int X, int Y, Color color)
		{
			Vector2 location = Vector2.Zero;
			Vector2 size = Vector2.Zero;
			if (location.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
			{
				size = font.MeasureString(text);
				switch (alignment)
				{
					case ('c'):
					case ('C'):
						location.X = (GameConstants.DisplayWidth / 2) - (size.X / 2);
						location.Y = Y - size.Y / 2;
						break;
					case ('l'):
					case ('L'):
						location.X = GameConstants.DisplayOffset;
						location.Y = Y - size.Y / 2;
						break;
					case ('r'):
					case ('R'):
						location.X = GameConstants.DisplayWidth - (GameConstants.DisplayOffset + size.X);
						location.Y = Y - size.Y / 2;
						break;
					case('x'):
					case('X'):
						location.X = X;
						location.Y = Y - size.Y / 2;
						break;
				}
				spriteBatch.DrawString(font, text, location, color);

			}
		}

		/// <summary>
		/// Updates the display area counters.
		/// </summary>
		private void UpdateDisplayArea()
		{
			// draw game info
			spriteBatch.DrawString(fontArial20, scoreString, GameConstants.ScoreLocation, Color.White);
			spriteBatch.Draw(whiteRectangle, new Rectangle(GameConstants.HealthLocation.X,
			                                               GameConstants.HealthLocation.Y,
			                                               healthWidth,GameConstants.HealthBarHeight), healthDisplayColor);
			spriteBatch.DrawString(fontArial20, shieldString, GameConstants.ShieldLocation, Color.Aquamarine);
			switch (lives)
			{
				case(1):
					spriteBatch.Draw(sprites[50], lifeIcon1, Color.White);
					break;
				case(2):
					spriteBatch.Draw(sprites[50], lifeIcon1, Color.White);
					spriteBatch.Draw(sprites[50], lifeIcon2, Color.White);
					break;
				case(3):
					spriteBatch.Draw(sprites[50], lifeIcon1, Color.White);
					spriteBatch.Draw(sprites[50], lifeIcon2, Color.White);
					spriteBatch.Draw(sprites[50], lifeIcon3, Color.White);
					break;
			}
			spriteBatch.DrawString(fontArial20, levelString, GameConstants.LevelLocation, Color.DarkSalmon);
			spriteBatch.DrawString(fontArial20, weaponString, GameConstants.WeaponLocation, weaponDisplayColor);
			spriteBatch.DrawString(fontArial20, nextString, GameConstants.NextLocation, Color.DarkSalmon);
		}

		/// <summary>
		/// Spawns a new teddy bear at a random location
		/// </summary>
		private void SpawnBear(BearType type)
		{
			// generate random location
			randX = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize);
			randY = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize);

			// generate random velocity
			if (type == BearType.YellowTrooper)
				randSpeed = GetRandomSpeed(LevelsData.Levels[currentLevel, 24], LevelsData.Levels[currentLevel, 25]);
			else
				randSpeed = GetRandomSpeed(LevelsData.Levels[currentLevel, 26], LevelsData.Levels[currentLevel, 27]);

			// generate random angle in radians, from 0 to 2*PI
			randAngle = GetRandomAngle();

			randVelocity.X = (float)(Math.Cos(randAngle) * randSpeed);
			randVelocity.Y = (float)(Math.Sin(randAngle) * randSpeed);

			// create new bear
			if (type == BearType.YellowTrooper)
			{
				newBear = new TeddyBear(BearType.YellowTrooper, Content, @"graphics\bear_yellow", randX, randY, randVelocity,
				                        LevelsData.Levels[currentLevel, 19], teddyBounce, teddyShot);
				currentYellowBears += 1;
			}
			else {
				newBear = new TeddyBear(BearType.GreenTrooper, Content, @"graphics\bear_green", randX, randY, randVelocity, 
				                        LevelsData.Levels[currentLevel, 20], teddyBounce, teddyShot);
				currentGreenBears += 1;
			}
			// make sure we don't spawn into a collision
			while (!CollisionUtils.IsCollisionFree(newBear.CollisionRectangle, GetCollisionRectangles()))
			{
				newBear.X = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize);
				newBear.Y = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize);
			}
			// add new bear to list
			bears.Add(newBear);
		}

		/// <summary>
		/// Spawns a new rock at a random location
		/// </summary>
		private void SpawnRock(RockType type)
		{
			// generate random location
			randX = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize);
			randY = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize);

			// generate random velocity
			if (type == RockType.Asteroid)
				randSpeed = GetRandomSpeed(LevelsData.Levels[currentLevel, 28], LevelsData.Levels[currentLevel, 29]);
			else
				randSpeed = GetRandomSpeed(LevelsData.Levels[currentLevel, 30], LevelsData.Levels[currentLevel, 31]);

			// generate random angle in radians, from 0 to 2*PI
			randAngle = GetRandomAngle();

		 	randVelocity.X = (float)(Math.Cos(randAngle) * randSpeed);
			randVelocity.Y = (float)(Math.Sin(randAngle) * randSpeed);

			// create new rock
			if (type == RockType.Asteroid)
			{
				newRock = new Rock(RockType.Asteroid, Content, @"graphics\rock_asteroid", randX, randY, randVelocity, LevelsData.Levels[currentLevel, 21]);
				currentAsteroids += 1;
			}
			else {
				newRock = new Rock(RockType.Meteor, Content, @"graphics\rock_meteor", randX, randY, randVelocity, LevelsData.Levels[currentLevel, 22]);
				currentMeteors += 1;
			}
			// make sure we don't spawn into a collision
			while (!CollisionUtils.IsCollisionFree(newRock.CollisionRectangle, GetCollisionRectangles()))
			{
				newRock.X = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize);
				newRock.Y = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize);
			}
			// add new rock to list
			rocks.Add(newRock);
		}

		/// <summary>
		/// Spawns the tomcat.
		/// </summary>
		private void SpawnTomcat()
		{
			Tomcat newTomcat;
			newTomcat = new Tomcat(Content, @"graphics\montage_tomcat",f14ShotSounds);
			// make sure we don't spawn into a collision
			while (!CollisionUtils.IsCollisionFree(newTomcat.CollisionRectangle, GetCollisionRectangles()))
			{
				newTomcat.X = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize);
				newTomcat.Y = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize);
			}
			tomcat = newTomcat;
		}

		/// <summary>
		/// Spawns the boss.
		/// </summary>
		private void SpawnBoss(BossType bossType)
		{
			Vector2 randLoc = Vector2.Zero;
			string bossSprite = "";
			int randBehaviour;
			// generate random location
			randLoc.X = GetRandomLocation(GameConstants.SpawnBorderSize * 2, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize * 2);
			randLoc.Y = GetRandomLocation(GameConstants.SpawnBorderSize * 2, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize * 2);
			randSpeed = GetRandomSpeed(LevelsData.Levels[currentLevel, 32], LevelsData.Levels[currentLevel, 33]);
			// generate random angle in radians, from 0 to 2*PI
			randAngle = GetRandomAngle();
			randVelocity.X = (float)(Math.Cos(randAngle) * randSpeed);
			randVelocity.Y = (float)(Math.Sin(randAngle) * randSpeed);
			// generate random behaviuour
			randBehaviour = RandomNumberGenerator.Next(4);
			switch (randBehaviour)
			{
				case(0):
					bossBehaviour = "down_forward";
					break;
				case(1):
					bossBehaviour = "erratic";
					break;
				case(2):
					bossBehaviour = "stalker";
					break;
				case(3):
					bossBehaviour = "default";
					break;
			}
			// debug: bossBehaviour = "default";
			// check level and boss type
			switch (bossType)
			{
				case (BossType.Boss1):
					bossSprite = @"graphics\montage_boss1";
					break;
				case (BossType.Boss2):
					bossSprite = @"graphics\montage_boss1";
					// TODO: Replate per bossSprite = @"graphics\boss2";
					break;
				case (BossType.Boss3):
					bossSprite = @"graphics\montage_boss1";
					// TODO: Replate per bossSprite = @"graphics\boss3";
					break;
				case (BossType.Boss4):
					bossSprite = @"graphics\montage_boss1";
					// TODO: Replate per bossSprite = @"graphics\boss4";
					break;
				case (BossType.Boss5):
					bossSprite = @"graphics\montage_boss1";
					// TODO: Replate per bossSprite = @"graphics\boss5";
					break;
				case (BossType.FinalBoss):
					bossSprite = @"graphics\montage_boss1";
					// TODO: Replate per bossSprite = @"graphics\final_boss";
					break;	
			}

			newBoss = new Boss(bossType, Content, bossSprite, whiteRectangle, randLoc, bossBehaviour, randSpeed, randVelocity,
			                   LevelsData.Levels[currentLevel, 23], bossSounds[0], bossSounds[1]);
			while (!CollisionUtils.IsCollisionFree(newBoss.CollisionRectangle, GetCollisionRectangles()))
			{
				newBoss.X = GetRandomLocation(GameConstants.SpawnBorderSize * 2, GameConstants.DisplayWidth - GameConstants.SpawnBorderSize * 2);
				newBoss.Y = GetRandomLocation(GameConstants.SpawnBorderSize * 2, GameConstants.DisplayHeight - GameConstants.SpawnBorderSize * 2);;
			}
			boss = newBoss;
		}

		/// <summary>
		/// Gets a random location using the given min and range
		/// </summary>
		/// <param name="min">the minimum</param>
		/// <param name="range">the range</param>
		/// <returns>the random location</returns>
		private int GetRandomLocation(int min, int range)
		{
			return min + RandomNumberGenerator.Next(range - min);
		}

		/// <summary>
		/// Gets the random speed.
		/// </summary>
		/// <returns>The random speed.</returns>
		/// <param name="min">Minimum.</param>
		/// <param name="range">Range.</param>
		private float GetRandomSpeed(float min, float range)
		{
			return (min + RandomNumberGenerator.NextFloat(range - min))*GameConstants.BaseSpeed;
		}

		/// <summary>
		/// Gets the random angle.
		/// </summary>
		/// <returns>The random angle.</returns>
		private float GetRandomAngle()
		{
			return (float)(Math.PI * RandomNumberGenerator.NextFloat(2));
		}

		/// <summary>
		/// Update counters and display
		/// </summary>
		private void UpdateEntitiesCounters()
		{
			if (!bossFight)
			{
				if (score >= (LevelsData.Levels[currentLevel, 40]))
				{
					levelUp = true;
					score = 0;
					nextLevel = LevelsData.Levels[currentLevel, 40];
				}
				else
				{
					nextLevel = LevelsData.Levels[currentLevel, 40] - score;
				}
			}
			else
			{
				// if is a boss fight, then next level is boss health
				nextLevel = boss.Health;

			}
			scoreString = globalScore.ToString();
			levelString = currentLevel.ToString();
			nextString = nextLevel.ToString();
			if (tomcat.PowerUp > 30)
			{
				weaponString = "Supreme " + tomcat.Weapon.ToString();
				weaponDisplayColor = Color.Pink;
			}
			else if (tomcat.PowerUp > 20)
			{
				weaponString = "Outstanding "+ tomcat.Weapon.ToString();
				weaponDisplayColor = Color.BlueViolet;
			}
			else if (tomcat.PowerUp > 10)
			{
				weaponString = "Improved "+ tomcat.Weapon.ToString();
				weaponDisplayColor = Color.LightBlue;
			}
			else if (tomcat.PowerUp > 0)
			{
				weaponString = "Powered " + tomcat.Weapon.ToString();
				weaponDisplayColor = Color.Blue;
			}
			else {
				weaponString = tomcat.Weapon.ToString();
				weaponDisplayColor = Color.DarkBlue;
			}
			shieldString = tomcat.Shield.ToString() + "%";
			switch (tomcat.Health / 10)
			{
				case(10): healthDisplayColor = Color.Purple;
					break;
				case (9): healthDisplayColor = Color.DarkBlue;
					break;
				case (8): healthDisplayColor = Color.LightBlue;
					break;
				case (7): healthDisplayColor = Color.DarkGreen;
					break;
				case (6): healthDisplayColor = Color.LightGreen;
					break;
				case (5): healthDisplayColor = Color.YellowGreen;
					break;
				case (4): healthDisplayColor = Color.Yellow;
					break;
				case (3): healthDisplayColor = Color.Orange;
					break;
				case (2): healthDisplayColor = Color.Red;
					break;
				case (1): healthDisplayColor = Color.DarkRed;
					break;
				case (0): healthDisplayColor = Color.Black;
					break;
			}
			healthWidth = (int)((float)tomcat.Health / GameConstants.F14InitialHealth * GameConstants.HealthBarWidth);
		}


		/// <summary>
		/// Gets a list of collision rectangles for all the objects in the game world
		/// </summary>
		/// <returns>the list of collision rectangles</returns>
		private List<Rectangle> GetCollisionRectangles()
		{
			Rectangle expandedRectangle = new Rectangle(0,0,0,0);
			List<Rectangle> collisionRectangles = new List<Rectangle>();
			if (tomcat != null)
			{
				expandedRectangle.X = tomcat.CollisionRectangle.X - 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Y = tomcat.CollisionRectangle.Y - 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Width = tomcat.CollisionRectangle.Width + 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Height = tomcat.CollisionRectangle.Height + 2 * GameConstants.SpawnBorderSize;
				collisionRectangles.Add(expandedRectangle);
			}
			if (boss != null)
			{
				expandedRectangle.X = boss.CollisionRectangle.X - 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Y = boss.CollisionRectangle.Y - 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Width = boss.CollisionRectangle.Width + 2 * GameConstants.SpawnBorderSize;
				expandedRectangle.Height = boss.CollisionRectangle.Height + 2 * GameConstants.SpawnBorderSize;
				collisionRectangles.Add(expandedRectangle);
			}
			foreach (TeddyBear bear in bears)
			{
				collisionRectangles.Add(bear.CollisionRectangle);
			}
			foreach (Projectile projectile in projectiles)
			{
				collisionRectangles.Add(projectile.CollisionRectangle);
			}
			foreach (Explosion explosion in explosions)
			{
				collisionRectangles.Add(explosion.CollisionRectangle);
			}
			foreach (Rock rock in rocks)
			{
				collisionRectangles.Add(rock.CollisionRectangle);
			}
			// do not add pickups as they are on another plan
			return collisionRectangles;
		}

		#endregion
	}
}
