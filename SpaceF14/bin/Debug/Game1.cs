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
		Tomcat tomcat;
		List<TeddyBear> bears = new List<TeddyBear>();
		static List<Projectile> projectiles = new List<Projectile>();
		List<Explosion> explosions = new List<Explosion>();
		List<Rock> rocks = new List<Rock>();
		List<Texture2D> sprites = new List<Texture2D>[7];

		// scoring support
		int score = 0;
		string scoreString = "0";

		// tomcat support
		string healthString = GameConstants.InitialHealthBar;
		string weaponString = ProjectileType.Laser.ToString();                               
		bool f14Dead = false;

		// lifes support
		int deathCooldownMilliseconds = 0;
		int lifes = GameConstants.F14InitialLifes;
		string lifesString = GameConstants.InitialLifeBar;

		// pause, skip & quit support
		bool pKeyPressed = false;
		bool escKeyPressed = false;
		bool sKeyPressed = false;

		// introduction support
		int rollingOffSet = 1;
		Vector2 introTitleLocation = GameConstants.IntroTitleLocation;
		Vector2 introBodyLocation1 = GameConstants.IntroBodyLocation;
		Vector2 introBodyLocation2 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation3 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation4 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation5 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation6 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation7 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation8 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation9 = new Vector2(GameConstants.IntroBodyLocation.X, 0);
		Vector2 introBodyLocation10 = new Vector2(GameConstants.IntroBodyLocation.X, 0);

		// spawn support
		int randX;
		int randY;
		float randSpeed;
		float randAngle;
		Vector2 randVelocity = new Vector2();
		TeddyBear newBear;
		Rock newRock;

		// text and display support
		SpriteFont fontArial20;
		SpriteFont fontArial70B;
		SpriteFont fontArial30B;
		Color healthDisplayColor = GameConstants.InitialHealthDisplayColor;
		int healthRange;
		Texture2D whiteRectangle;

		// sound effects
		SoundEffect f14Damage;
		SoundEffect f14Death;
		SoundEffect[] f14ShotSounds = new SoundEffect[2];
		SoundEffect[] explosionSounds = new SoundEffect[3];
		SoundEffect teddyBounce;
		SoundEffect teddyShot;
		SoundEffectInstance backgroundMusic;

		// level & difficulty support
		int maxYellowBears = GameConstants.MaxSpawnedYellowBears;
		int maxGreenBears = GameConstants.MaxSpawnedGreenBears;
		int maxAsteroids = GameConstants.MaxSpawnedAsteroids;
		int maxMeteors = GameConstants.MaxSpawnedMeteors;
		int currentYellowBears = 0;
		int currentGreenBears = 0;
		int currentAsteroids = 0;
		int currentMeteors = 0;
		int currentLevel = 1;
		int nextLevel = GameConstants.LevelCap;
		string levelString = "1";
		string nextString = GameConstants.LevelCap.ToString();

		// collision handling
		CollisionResolutionInfo collisionInfo;

		// game state FSM
		GameState gameState = GameState.Introduction;

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
			f14Damage = Content.Load<SoundEffect>(@"audio\F14Damage");
			f14Death = Content.Load<SoundEffect>(@"audio\F14Death");
			f14ShotSounds[0] = Content.Load<SoundEffect>(@"audio\F14LaserShot");
			f14ShotSounds[1] = Content.Load<SoundEffect>(@"audio\F14PlasmShot");
			explosionSounds[0] = Content.Load<SoundEffect>(@"audio\Explosion");
			explosionSounds[1] = Content.Load<SoundEffect>(@"audio\ProjectileExplosion");
			explosionSounds[2] = Content.Load<SoundEffect>(@"audio\MeteorExplosion");
			teddyBounce = Content.Load<SoundEffect>(@"audio\TeddyBounce");
			teddyShot = Content.Load<SoundEffect>(@"audio\TeddyShot");

			// load and start playing background music
			SoundEffect backgroundMusicEffect = Content.Load<SoundEffect>(@"audio\backgroundMusic");
			backgroundMusic = backgroundMusicEffect.CreateInstance();
			backgroundMusic.IsLooped = true;
			backgroundMusic.Volume = GameConstants.BackgroundVolume;

			// load sprite font
			fontArial20 = Content.Load<SpriteFont>(@"fonts\Arial20");
			fontArial70B = Content.Load<SpriteFont>(@"fonts\Arial70");
			fontArial30B = Content.Load<SpriteFont>(@"fonts\Arial30B");

			// load projectile and explosion sprites
			sprites[0] = Content.Load<Texture2D>(@"graphics\teddybearprojectile");
			sprites[1] = Content.Load<Texture2D>(@"graphics\plasm_shoot");
			sprites[2] = Content.Load<Texture2D>(@"graphics\laser_shoot");
			sprites[3] = Content.Load<Texture2D>(@"graphics\explosion");
			sprites[4] = Content.Load<Texture2D>(@"graphics\tomcat_explosion");
			sprites[5] = Content.Load<Texture2D>(@"graphics\purple_explosion");
			sprites[6] = Content.Load<Texture2D>(@"graphics\rock_explosion");
			sprites[7] = Content.Load<Texture2D>(@"graphics\huble_sky");

			// add initial game objects
			tomcat = new Tomcat(Content, @"graphics\f14_tomcat",f14ShotSounds);

			// create a square rectangle for display
			whiteRectangle = new Texture2D(GraphicsDevice, 1,1);
			whiteRectangle.SetData(new[] { Color.White });

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
			KeyboardState keyboard = Keyboard.GetState();

			// get current mouse state and update tomcat
			MouseState mouse = Mouse.GetState();

			// use FSM to control game states
			switch(gameState) {
				case (GameState.Introduction):
					// check for unpause
					if (keyboard.IsKeyDown(Keys.S))
					{
						sKeyPressed = true;
					}
					if (sKeyPressed && keyboard.IsKeyUp(Keys.S))
					{
						sKeyPressed = false;
						gameState = GameState.StartUp;
					}
					else
					{
						if (introTitleLocation.Y > GameConstants.DisplayOffset) introTitleLocation.Y -= rollingOffSet;
						introBodyLocation1.Y = introTitleLocation.Y + 4 * GameConstants.DisplayOffset;
						introBodyLocation2.Y = introTitleLocation.Y + 5 * GameConstants.DisplayOffset;
						introBodyLocation3.Y = introTitleLocation.Y + 6 * GameConstants.DisplayOffset;
						introBodyLocation4.Y = introTitleLocation.Y + 7 * GameConstants.DisplayOffset;
						introBodyLocation5.Y = introTitleLocation.Y + 8 * GameConstants.DisplayOffset;
						introBodyLocation6.Y = introTitleLocation.Y + 9 * GameConstants.DisplayOffset;
						introBodyLocation7.Y = introTitleLocation.Y + 10 * GameConstants.DisplayOffset;
						introBodyLocation8.Y = introTitleLocation.Y + 11 * GameConstants.DisplayOffset;
						introBodyLocation9.Y = introTitleLocation.Y + 12 * GameConstants.DisplayOffset;
						introBodyLocation10.Y = introTitleLocation.Y + 13 * GameConstants.DisplayOffset;
					}
					break;
				case (GameState.StartUp):
					
					gameState = GameState.Paused;
					break;
				case (GameState.Paused):
					backgroundMusic.Stop();
					// check for unpause
					if (keyboard.IsKeyDown(Keys.P))
					{
						pKeyPressed = true;
					}
					if (pKeyPressed && keyboard.IsKeyUp(Keys.P))
					{
						pKeyPressed = false;
						gameState = GameState.Fighting;
					}
					break;
				case (GameState.BossFight):
					break;
				case (GameState.LevelUp):

					gameState = GameState.Fighting;
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
						gameState = GameState.Paused;
					}
					else
					{
						backgroundMusic.Play();
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
											if (projectile2.CollisionRectangle.Intersects(projectile1.CollisionRectangle) &&
												(projectile1.Type == ProjectileType.Teddy || projectile2.Type == ProjectileType.Teddy))
											{
												projectile1.Active = false;
												projectile2.Active = false;
												explosions.Add(new Explosion(ExplosionType.ProjectileVersus, purpleExplosionSprite,
																			 (projectile1.CollisionRectangle.Center.X + projectile2.CollisionRectangle.Center.X) / 2,
																			 (projectile1.CollisionRectangle.Center.Y + projectile2.CollisionRectangle.Center.Y) / 2));
												explosionSounds[1].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
												// give some points since it hit bear projectile
												score += GameConstants.BearProjectilePoints;
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
											if (rock.CollisionRectangle.Intersects(bear.CollisionRectangle))
											{
												bear.Active = false;
												rock.Active = false;
												explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, bearExplosionSprite,
																			 bear.CollisionRectangle.Center));
												explosions.Add(new Explosion(ExplosionType.RockDeath, rockExplosionSprite,
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
									if (tomcat.CollisionRectangle.Intersects(bear.CollisionRectangle))
									{
										bear.Active = false;
										if (bear.Type == BearType.YellowTrooper)
										{
											tomcat.TakeDamage(GameConstants.BearYellowCollisionDamage);
											score += GameConstants.BearYellowPoints;
										}
										else {
											tomcat.TakeDamage(GameConstants.BearGreenCollisionDamage);
											score += GameConstants.BearGreenPoints;
										}
										explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, bearExplosionSprite,
																	 bear.CollisionRectangle.Center));
										f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
										explosionSounds[0].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
									}
								}
							}
							// check and resolve collisions between tomcat and rocks
							foreach (Rock rock in rocks)
							{
								if (rock.Active)
								{
									if (tomcat.CollisionRectangle.Intersects(rock.CollisionRectangle))
									{
										rock.Active = false;
										if (rock.Type == RockType.Asteroid)
										{
											tomcat.TakeDamage(GameConstants.AsteroidCollisionDamage);
											score += GameConstants.AsteroidPoints;
										}
										else {
											tomcat.TakeDamage(GameConstants.MeteorCollisionDamage);
											score += GameConstants.MeteorPoints;
										}
										explosions.Add(new Explosion(ExplosionType.RockDeath, rockExplosionSprite,
																	 rock.CollisionRectangle.Center));
										f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
										explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
									}
								}
							}
							// check and resolve collisions between tomcat and projectiles
							foreach (Projectile projectile in projectiles)
							{
								if (projectile.Active && projectile.Type == ProjectileType.Teddy)
								{
									if (tomcat.CollisionRectangle.Intersects(projectile.CollisionRectangle))
									{
										tomcat.TakeDamage(GameConstants.BearProjectileDamage);
										projectile.Active = false;
										explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, bearExplosionSprite,
																	 projectile.CollisionRectangle.Center));
										f14Damage.Play(volume: GameConstants.DamageVolume, pitch: 0.0f, pan: 0.0f);
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
											if (bear.CollisionRectangle.Intersects(projectile.CollisionRectangle))
											{
												if (projectile.Type == ProjectileType.Laser)
												{
													bear.TakeDamage(GameConstants.LaserProjectileDamage + tomcat.PowerUp);
													if (bear.Health == 0)
													{
														bear.Active = false;
														if (bear.Type == BearType.YellowTrooper)
														{
															score += GameConstants.BearYellowPoints;
														}
														else {
															score += GameConstants.BearGreenPoints;
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
															score += GameConstants.BearYellowPoints;
														}
														else {
															score += GameConstants.BearGreenPoints;
														}
													}
												}
												explosions.Add(new Explosion(ExplosionType.TeddyBearDeath, bearExplosionSprite, bear.DrawRectangle.Center));
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
											if (rock.CollisionRectangle.Intersects(projectile.CollisionRectangle))
											{
												if (projectile.Type == ProjectileType.Laser)
												{
													rock.TakeDamage(GameConstants.LaserProjectileDamage + tomcat.PowerUp);
													if (rock.Health == 0)
													{
														rock.Active = false;
														if (rock.Type == RockType.Asteroid)
														{
															score += GameConstants.AsteroidPoints;
														}
														else {
															score += GameConstants.MeteorPoints;
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
															score += GameConstants.AsteroidPoints;
														}
														else {
															score += GameConstants.MeteorPoints;
														}
													}
												}
												else
												{
													rock.TakeDamage(GameConstants.TeddyBearProjectileDamage);
													if (rock.Health == 0)
													{
														rock.Active = false;
													}
													projectile.Active = false;
												}
												explosions.Add(new Explosion(ExplosionType.RockDeath, rockExplosionSprite, rock.DrawRectangle.Center));
												explosionSounds[2].Play(volume: GameConstants.ExplosionVolume, pitch: 0.0f, pan: 0.0f);
											}
										}
									}
								}
							}
						if (TomcatDead())
						{
							f14Dead = true;
							explosions.Add(new Explosion(ExplosionType.TomcatDeath, tomcatExplosionSprite, tomcat.X, tomcat.Y));
							f14Death.Play(volume: GameConstants.DeathVolume, pitch: 0.0f, pan: 0.0f);
						}
						UpdateEntitiesCounters();
					}
						                             
					break;
				case (GameState.Death):
					foreach (Explosion explosion in explosions)
					{
						explosion.Update(gameTime);
					}
					deathCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
					if (deathCooldownMilliseconds > GameConstants.F14DeathCooldownMilliseconds)
					{
						deathCooldownMilliseconds = 0;
						tomcat = new Tomcat(Content, @"graphics\f14_tomcat", f14ShotSounds);
						f14Dead = false;
						gameState = GameState.Fighting;
						UpdateEntitiesCounters();
					}
					break;
				case (GameState.GameOver):
					backgroundMusic.Stop();
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
			while (currentYellowBears < maxYellowBears)
			{
				SpawnBear(BearType.YellowTrooper);
			}
			while (currentGreenBears < maxGreenBears)
			{
				SpawnBear(BearType.GreenTrooper);
			}
			while (currentAsteroids < maxAsteroids)
			{
				SpawnRock(RockType.Asteroid);
			}
			while (currentMeteors < maxMeteors)
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
			// draw display area
			DrawDisplayArea();
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
				case (GameState.Introduction):

					if (introTitleLocation.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial70B, GameConstants.IntroTitleString, introTitleLocation, Color.OrangeRed);
					}
					if (introBodyLocation1.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString1, introBodyLocation1, Color.White);
					}
					if (introBodyLocation2.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString2, introBodyLocation2, Color.White);
					}
					if (introBodyLocation3.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString3, introBodyLocation3, Color.White);
					}
					if (introBodyLocation4.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString4, introBodyLocation4, Color.White);
					}
					if (introBodyLocation5.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString5, introBodyLocation5, Color.White);
					}
					if (introBodyLocation6.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString6, introBodyLocation6, Color.White);
					}
					if (introBodyLocation7.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString7, introBodyLocation7, Color.White);
					}
					if (introBodyLocation8.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString8, introBodyLocation8, Color.White);
					}
					if (introBodyLocation9.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString9, introBodyLocation9, Color.White);
					}
					if (introBodyLocation10.Y < GameConstants.DisplayHeight - GameConstants.DisplayOffset)
					{
						spriteBatch.DrawString(fontArial20, GameConstants.IntroBodyString10, introBodyLocation10, Color.Yellow);
					}

					spriteBatch.DrawString(fontArial30B, GameConstants.StartString, GameConstants.StartLocation, Color.Yellow);
					break;
				case (GameState.Fighting):
					// draw game objects
					DrawAllGameObjects();

					// update display area
					UpdateDisplayArea();

					break;
				case (GameState.Paused):
					// draw game objects
					DrawAllGameObjects();

					// update counters
					UpdateDisplayArea();
					spriteBatch.DrawString(fontArial30B, GameConstants.PausedString, GameConstants.PausedLocation, Color.Yellow);

					break;
				case (GameState.Death):
					// draw game objects
					DrawAllGameObjects();

					break;
				case (GameState.GameOver):
					// update counters
					UpdateDisplayArea();
					spriteBatch.DrawString(fontArial70B, GameConstants.GameOverString, GameConstants.GameOverLocation, Color.White);

					break;
			}
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
			// replace with code to return correct projectile sprite based on projectile type
			switch (type) {

				case ProjectileType.Plasm:
					return sprites[2];
				case ProjectileType.Laser:
					return sprites[1];
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

		#endregion

		#region Private methods

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
			foreach (TeddyBear bear in bears)
			{
				if (bear.Active) bear.Draw(spriteBatch);
			}
			foreach (Rock rock in rocks)
			{
				if (rock.Active)
				{
					rock.Draw(spriteBatch);
				}
			}
			foreach (Projectile projectile in projectiles)
			{
				if (projectile.Active) projectile.Draw(spriteBatch);
			}
			foreach (Explosion explosion in explosions)
			{
				if (!explosion.Finished) explosion.Draw(spriteBatch);
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
			spriteBatch.DrawString(fontArial20, GameConstants.LifesPrefix , GameConstants.LifesPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.LevelPrefix, GameConstants.LevelPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.WeaponPrefix, GameConstants.WeaponPrefixLocation, GameConstants.StringColor);
			spriteBatch.DrawString(fontArial20, GameConstants.NextPrefix, GameConstants.NextPrefixLocation, GameConstants.StringColor);
		}

		//private void DrawSpaceBackground()
		//{
		//	spriteBatch.Draw(spaceBackgroundSprite, new Rectangle(0,0,GameConstants.DisplayWidth,GameConstants.DisplayHeight), Color.Black);
		//}

		/// <summary>
		/// Updates the display area counters.
		/// </summary>
		private void UpdateDisplayArea()
		{
			// draw game info
			spriteBatch.DrawString(fontArial20, scoreString, GameConstants.ScoreLocation, Color.White);
			spriteBatch.DrawString(fontArial20, healthString, GameConstants.HealthLocation, healthDisplayColor);
			spriteBatch.DrawString(fontArial20, lifesString, GameConstants.LifesLocation, Color.Gold);
			spriteBatch.DrawString(fontArial20, levelString, GameConstants.LevelLocation, Color.DarkSalmon);
			spriteBatch.DrawString(fontArial20, weaponString, GameConstants.WeaponLocation, Color.BlueViolet);
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
				randSpeed = GetRandomSpeed(GameConstants.BearYellowMinSpeed, GameConstants.BearYellowMaxSpeed);
			else
				randSpeed = GetRandomSpeed(GameConstants.BearGreenMinSpeed, GameConstants.BearGreenMaxSpeed);

			// generate random angle in radians, from 0 to 2*PI
			randAngle = GetRandomAngle();

			randVelocity.X = (float)(Math.Cos(randAngle) * randSpeed);
			randVelocity.Y = (float)(Math.Sin(randAngle) * randSpeed);

			// create new bear
			if (type == BearType.YellowTrooper)
			{
				newBear = new TeddyBear(BearType.YellowTrooper, Content, @"graphics\teddybear0", randX, randY, randVelocity,
				                        GameConstants.BearYellowInitialHealth, teddyBounce, teddyShot);
				currentYellowBears += 1;
			}
			else {
				newBear = new TeddyBear(BearType.GreenTrooper, Content, @"graphics\teddybear1", randX, randY, randVelocity, 
				                        GameConstants.BearGreenInitialHealth, teddyBounce, teddyShot);
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
				randSpeed = GetRandomSpeed(GameConstants.AsteroidMinSpeed, GameConstants.AsteroidMaxSpeed);
			else
				randSpeed = GetRandomSpeed(GameConstants.MeteorMinSpeed, GameConstants.MeteorMaxSpeed);

			// generate random angle in radians, from 0 to 2*PI
			randAngle = GetRandomAngle();

		 	randVelocity.X = (float)(Math.Cos(randAngle) * randSpeed);
			randVelocity.Y = (float)(Math.Sin(randAngle) * randSpeed);

			// create new rock
			if (type == RockType.Asteroid)
			{
				newRock = new Rock(RockType.Asteroid, Content, @"graphics\asteroid", randX, randY, randVelocity, GameConstants.AsteroidInitialHealth);
				currentAsteroids += 1;
			}
			else {
				newRock = new Rock(RockType.Meteor, Content, @"graphics\meteor", randX, randY, randVelocity, GameConstants.MeteorInitialHealth);
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
			return min + RandomNumberGenerator.NextFloat(range - min);
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
			currentLevel = score / GameConstants.LevelCap + 1;
			nextLevel = currentLevel * GameConstants.LevelCap - score;
			maxYellowBears = RandomNumberGenerator.Next(currentLevel) + GameConstants.MaxSpawnedYellowBears;
			maxGreenBears = RandomNumberGenerator.Next(currentLevel) + GameConstants.MaxSpawnedGreenBears;
			maxAsteroids = RandomNumberGenerator.Next(currentLevel) + GameConstants.MaxSpawnedAsteroids;
			maxMeteors = RandomNumberGenerator.Next(currentLevel) + GameConstants.MaxSpawnedMeteors;
			scoreString = score.ToString();
			levelString = currentLevel.ToString();
			nextString = nextLevel.ToString();
			weaponString = tomcat.Weapon.ToString();
			healthRange = tomcat.Health / 10;
			switch (healthRange)
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
			healthString = "";
			lifesString = "";
			for (int i = 1; i <= lifes; i++)
			{
				lifesString += GameConstants.LifeSymbol;
			}
			if (tomcat.Health > 0)
			{
				for (int i = 0; i <= healthRange; i++)
				{
					healthString += GameConstants.HealthSymbol;
				}
			}
			else {
				healthString += "0";
			}
		}

		/// <summary>
		/// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
		/// of the specified color.
		///
		/// By Sean Colombo, from http://bluelinegamestudios.com/blog
		/// </summary>
		/// <param name="rectangleToDraw"></param>
		/// <param name="thicknessOfBorder"></param>
		private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
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

		/// <summary>
		/// Gets a list of collision rectangles for all the objects in the game world
		/// </summary>
		/// <returns>the list of collision rectangles</returns>
		private List<Rectangle> GetCollisionRectangles()
		{
			Rectangle expandedRectangle = new Rectangle(tomcat.CollisionRectangle.X - GameConstants.SpawnBorderSize,
			                                           tomcat.CollisionRectangle.Y - GameConstants.SpawnBorderSize,
			                                           tomcat.CollisionRectangle.Width + 2*GameConstants.SpawnBorderSize,
			                                           tomcat.CollisionRectangle.Height + 2*GameConstants.SpawnBorderSize);
			List<Rectangle> collisionRectangles = new List<Rectangle>();
			collisionRectangles.Add(expandedRectangle);
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
			return collisionRectangles;
		}

		/// <summary>
		/// Checks to see if the tomcat has just been killed
		/// </summary>
		private Boolean TomcatDead()
		{
			if (tomcat.Health <= 0)
			{
				lifes -= 1;
				if (lifes < 0)
					gameState = GameState.GameOver;
				else
					gameState = GameState.Death;
				return true;
			}
			else return false;
		}

		#endregion
	}
}
