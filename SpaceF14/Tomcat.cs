using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceF14
{
	/// <summary>
	/// A tomcat
	/// </summary>
	public class Tomcat
	{
		
		#region Fields

		// graphic and drawing info
		Texture2D strip;
		Color[] textureData;
		Rectangle drawRectangle;
		Rectangle sourceRectangle;

		// tomcat aspects
		int health;
		float speed;
		float previousSpeed;
		float angle = 0f;
		int shield = 0;
		int powerup = 0;
		Vector2 velocity;
		ProjectileType weapon = ProjectileType.Laser;

		// tomcat rotation support
		Point center, laserPos1, laserPos2, plasmPos;
		Point laserTurret1, laserTurret2, plasmTurret;
		int frameWidth;
		int frameHeight;
		int currentFrame;

		// shooting support
		bool canShoot = true;
		int elapsedCooldownMilliseconds = 0;

		// sound support
		SoundEffect laserSoundEffect;
		SoundEffect plasmSoundEffect;

		// click processing
		bool rightClickStarted = false;
		bool rightButtonReleased = true;
		bool oKeyPressed = false;

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets tomcat health
		/// </summary>
		public int Health
		{
			get { return health; }
		}

		/// <summary>
		/// Gets the weapon.
		/// </summary>
		/// <value>The weapon.</value>
		public ProjectileType Weapon
		{
			get { return weapon; }
		}

		/// <summary>
		/// Gets the power up.
		/// </summary>
		/// <value>The power up.</value>
		public int PowerUp
		{
			get { return powerup; }
		}

		/// <summary>
		/// Gets the shield.
		/// </summary>
		/// <value>The shield.</value>
		public int Shield
		{
			get { return shield; }
		}

		/// <summary>
		/// Gets the collision rectangle for the tomcat
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
		}

		/// <summary>
		/// Gets the location of the boss
		/// </summary>
		public Point Location
		{
		get { return drawRectangle.Center; }
		}

		/// <summary>
		/// Sets the x location of the center of the boss
		/// </summary>
		public int X
		{
		set { drawRectangle.X = value - drawRectangle.Width / 2; }
		}

		/// <summary>
		/// Sets the y location of the center of the boss
		/// </summary>
		public int Y
		{
			set { drawRectangle.Y = value - drawRectangle.Height / 2; }
		}

		/// <summary>
		/// Gets and sets the velocity of the tomcat
		/// </summary>
		public Vector2 Velocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		/// <summary>
		/// Gets the texture data.
		/// </summary>
		/// <value>The texture data.</value>
		public Color[] TextureData
		{
			get { return textureData; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Tomcat"/> class.
		/// </summary>
		/// <param name="contentManager">Content manager.</param>
		/// <param name="spriteName">Sprite name.</param>
		/// <param name="soundEffects">Sound effects.</param>
		public Tomcat(ContentManager contentManager, string spriteName,
					  SoundEffect[] soundEffects)
			: this(contentManager, spriteName, GameConstants.F14InitialX, GameConstants.F14InitialY,
				   GameConstants.F14InitialHealth, GameConstants.F14InitialSpeed, soundEffects[0], soundEffects[1], ProjectileType.Laser)
		{
		}

		/// <summary>
		///  Constructs a tomcat on default location, health, speed and weapon
		/// </summary>
		/// <param name="contentManager">the content manager for loading content</param>
		/// <param name="spriteName">the sprite name</param>
		/// <param name="laserSoundEffect">the sound effect for laser weapon</param>
		/// <param name="plasmSoundEffect">the sound effect for plasm weapon</param>
		public Tomcat(ContentManager contentManager, string spriteName,
					  SoundEffect laserSoundEffect, SoundEffect plasmSoundEffect) 
			: this(contentManager, spriteName, GameConstants.F14InitialX, GameConstants.F14InitialY, 
			       GameConstants.F14InitialHealth, GameConstants.F14InitialSpeed, laserSoundEffect, plasmSoundEffect, ProjectileType.Laser)
		{
		}

		/// <summary>
		///  Constructs a tomcat
		/// </summary>
		/// <param name="contentManager">the content manager for loading content</param>
		/// <param name="spriteName">the sprite name</param>
		/// <param name="x">the x location of the center of the tomcat</param>
		/// <param name="y">the y location of the center of the tomcat</param>
		/// <param name="health">the initial health of the tomcat</param>
		/// <param name="speed">the initial linear speed of the tomcat</param>
		/// <param name="laserSoundEffect">the sound effect for laser weapon</param>
		/// <param name="plasmSoundEffect">the sound effect for plasm weapon</param>
		/// <param name="weapon">the initial active weapon of the tomcat</param>
		public Tomcat(ContentManager contentManager, string spriteName, int x, int y, int health, float speed,
		              SoundEffect laserSoundEffect, SoundEffect plasmSoundEffect, ProjectileType weapon)
		{
            Initialize(contentManager, spriteName, x, y);
			this.laserSoundEffect = laserSoundEffect;
			this.plasmSoundEffect = plasmSoundEffect;
			this.health = health;
			this.speed = speed;
			this.weapon = weapon;
		}

		#endregion


		#region Public methods

		/// <summary>
		/// Updates the tomcat's location based on mouse. Also fires 
		/// french fries as appropriate
		/// </summary>
		/// <param name="gameTime">game time</param>
		/// <param name="mouse">the current state of the mouse</param>
		/// <param name="keyboard">the current state of the keyboard</param>
		public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
		{
			// tomcat should only respond to input if it still has health
			if (health > 0)
			{
				// update source rectangle
				SetSourceRectangleLocation(currentFrame);
				// update texture color data for collision detection
				strip.GetData(0, sourceRectangle, textureData,0,textureData.Length);
				// move tomcat
				drawRectangle.X += (int)(velocity.X * gameTime.ElapsedGameTime.Milliseconds);
				drawRectangle.Y += (int)(velocity.Y * gameTime.ElapsedGameTime.Milliseconds);
				// update tomcat center position
				center.X = drawRectangle.Center.X;
				center.Y = drawRectangle.Center.Y;
				// update turrets positions based on the new center
				laserTurret1.X = center.X + GameConstants.LaserTurretXDist;
				laserTurret1.Y = center.Y + GameConstants.LaserTurretYDist;
				laserTurret2.X = center.X - GameConstants.LaserTurretXDist;
				laserTurret2.Y = center.Y + GameConstants.LaserTurretYDist;
				plasmTurret.X = center.X + GameConstants.PlasmTurretXDist;
				plasmTurret.Y =	 center.Y + GameConstants.PlasmTurretYDist;
				RotateTurrets();

				// process keyboard
				if (keyboard.IsKeyDown(Keys.A) ||
					keyboard.IsKeyDown(Keys.Left))
				{
					// Left + Up
					if (keyboard.IsKeyDown(Keys.W) ||
						keyboard.IsKeyDown(Keys.Up))
					{
						RotateLeft(GameConstants.F14AngleIncreasing);
						SpeedUp(GameConstants.F14MovementAmount);
						SetVelocity();
					}
					// Left + Down
					else if (keyboard.IsKeyDown(Keys.S) ||
							 keyboard.IsKeyDown(Keys.Down))
					{
						RotateLeft(GameConstants.F14AngleIncreasing);
						SpeedDown(GameConstants.F14MovementAmount);
						SetVelocity();

					}
					// Just left
					else
					{
						RotateLeft(GameConstants.F14AngleIncreasing);
					}
				}
				else if (keyboard.IsKeyDown(Keys.D) ||
						 keyboard.IsKeyDown(Keys.Right))
				{
					// Right + Up
					if (keyboard.IsKeyDown(Keys.W) ||
						keyboard.IsKeyDown(Keys.Up))
					{
						RotateRight(GameConstants.F14AngleIncreasing);
						SpeedUp(GameConstants.F14MovementAmount);
						SetVelocity();
					}
					// Right + Down
					else if (keyboard.IsKeyDown(Keys.S) ||
							 keyboard.IsKeyDown(Keys.Down))
					{
						RotateRight(GameConstants.F14AngleIncreasing);
						SpeedDown(GameConstants.F14MovementAmount);
						SetVelocity();
					}
					// Just right
					else
					{
						RotateRight(GameConstants.F14AngleIncreasing);
					}
				}
				// just down
				else if (keyboard.IsKeyDown(Keys.S) ||
							 keyboard.IsKeyDown(Keys.Down))
				{
					SpeedDown(GameConstants.F14MovementAmount);
					SetVelocity();
				}
				// just up
				else if (keyboard.IsKeyDown(Keys.W) ||
						 keyboard.IsKeyDown(Keys.Up))
				{
					SpeedUp(GameConstants.F14MovementAmount);
					SetVelocity();
				}

				// warp tomcat if outside window
				if (drawRectangle.Left + drawRectangle.Width < 0)
				{
					drawRectangle.X = GameConstants.DisplayWidth;
				}
				else if (drawRectangle.Right - drawRectangle.Width > GameConstants.DisplayWidth)
				{
					drawRectangle.X = 0;
				}
				if (drawRectangle.Top + drawRectangle.Height < 0)
				{
					drawRectangle.Y = GameConstants.DisplayHeight;
				}
				else if (drawRectangle.Bottom - drawRectangle.Height > GameConstants.DisplayHeight)
				{
					drawRectangle.Y = 0;
				}


				// change weapon with right button
				if (mouse.RightButton == ButtonState.Pressed &&
				rightButtonReleased)
				{
					rightClickStarted = true;
					rightButtonReleased = false;
				}
				else if (mouse.RightButton == ButtonState.Released)
				{
					rightButtonReleased = true;

					// if right click finished, change main weapon
					if (rightClickStarted)
					{
						rightClickStarted = false;
						CycleWeapon();
					}
				}

				// change weapon with O key
				if (keyboard.IsKeyDown(Keys.O)) oKeyPressed = true;

				if (oKeyPressed && keyboard.IsKeyUp(Keys.O))
				{
					oKeyPressed = false;
					CycleWeapon();
				}

				// shoot if appropriate
				if ((mouse.LeftButton.Equals(ButtonState.Pressed) || keyboard.IsKeyDown(Keys.Space)) && canShoot)
				{
					
					if (weapon == ProjectileType.Laser)
					{
						Game1.AddProjectile(new Projectile(ProjectileType.Laser, Game1.GetProjectileSprite(ProjectileType.Laser),
						                                   laserPos1, currentFrame));
						Game1.AddProjectile(new Projectile(ProjectileType.Laser, Game1.GetProjectileSprite(ProjectileType.Laser),
						                                   laserPos2, currentFrame));
						laserSoundEffect.Play(volume: GameConstants.ShotVolume, pitch:0.0f,pan:0.0f);
						Game1.UpdateStats(StatsType.firedLaserShots,2);
					}
					else {
						Game1.AddProjectile(new Projectile(ProjectileType.Plasm, Game1.GetProjectileSprite(ProjectileType.Plasm),
						                                   plasmPos, currentFrame));
						plasmSoundEffect.Play(volume: GameConstants.ShotVolume, pitch: 0.0f, pan: 0.0f);
						Game1.UpdateStats(StatsType.firedPlasmShots , 1);
					}
					canShoot = false;
				}
				if (!canShoot)
				{
					elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
					if (elapsedCooldownMilliseconds > GameConstants.F14TotalCooldownMilliseconds ||
					    (mouse.LeftButton.Equals(ButtonState.Released) && keyboard.IsKeyUp(Keys.Space)))
					{
						canShoot = true;
						elapsedCooldownMilliseconds = 0;
					}

				}
			}
		}

		/// <summary>
		/// Draws the tomcat
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(strip, drawRectangle, sourceRectangle, Color.White);
		}

		/// <summary>
		/// Repulse the specified collidedPoint.
		/// </summary>
		/// <returns>The repulse.</returns>
		/// <param name="collidedPoint">Collided point.</param>
		public void Repulse(Point collidedPoint, Point bossCenter)
		{
			speed = GameConstants.F14MaximumSpeed;
			double repelAngle=0;
			if (collidedPoint.X >= bossCenter.X && collidedPoint.Y >= bossCenter.Y)
			{
				repelAngle = Math.PI * 1 / 4;
			}
			else if (collidedPoint.X <= bossCenter.X && collidedPoint.Y >= bossCenter.Y)
			{
				repelAngle = Math.PI * 3 / 4;
			}
			else if (collidedPoint.X <= bossCenter.X && collidedPoint.Y <= bossCenter.Y)
			{
				repelAngle = Math.PI * 5 / 4;
			}
			else if (collidedPoint.X >= bossCenter.X && collidedPoint.Y <= bossCenter.Y)
			{
				repelAngle = Math.PI * 7 / 4;
			}
			velocity.X = (float)(Math.Cos(repelAngle) * speed);
			velocity.Y = (float)(Math.Sin(repelAngle) * speed);
		}

		/// <summary>
		/// Takes the damage.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void TakeDamage(int amount)
		{
			if (shield == 0)
			{
				health -= amount;
				if (health < 0) health = 0;
			}
			else
			{
				shield -= amount;
				if (shield < 0) shield = 0;
			}
		}

		/// <summary>
		/// Powers it up.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void PowerItUp(int amount)
		{
			powerup += amount;
		}

		/// <summary>
		/// Shields up.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void RaiseShield(int amount)
		{
			shield = amount;
			if (shield > 100) shield = 100;
		}

		/// <summary>
		/// Fixs it.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void FixIt(int amount)
		{
			health += amount;
			if (health > GameConstants.F14InitialHealth) health = GameConstants.F14InitialHealth;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Rotates the turrets.
		/// </summary>
		/// <returns>The turrets.</returns>
		private void RotateTurrets()
		{
			angle = (float)(currentFrame* Math.PI) / 90;
			double cosTheta = Math.Cos(angle);
			double sinTheta = Math.Sin(angle);
			laserPos1.X = (int)(cosTheta * (laserTurret1.X - center.X) -
								sinTheta * (laserTurret1.Y - center.Y) + center.X);
			laserPos1.Y = (int)(sinTheta * (laserTurret1.X - center.X) +
			 cosTheta * (laserTurret1.Y - center.Y) + center.Y);
			laserPos2.X = (int)(cosTheta * (laserTurret2.X - center.X) -
								sinTheta * (laserTurret2.Y - center.Y) + center.X);
			laserPos2.Y = (int)(sinTheta * (laserTurret2.X - center.X) +
			 					cosTheta * (laserTurret2.Y - center.Y) + center.Y);
			plasmPos.X = (int)(cosTheta * (plasmTurret.X - center.X) -
								sinTheta * (plasmTurret.Y - center.Y) + center.X);
			plasmPos.Y = (int)(sinTheta * (plasmTurret.X - center.X) +
			 					cosTheta * (plasmTurret.Y - center.Y) + center.Y);
		}

		/// <summary>
		/// Sets the source rectangle location.
		/// </summary>
		/// <param name="frameNumber">Frame number.</param>
		private void SetSourceRectangleLocation(int frameNumber)
		{
			sourceRectangle.X = (frameNumber % GameConstants.MontageFramesPerRow) * frameWidth;
			sourceRectangle.Y = (frameNumber / GameConstants.MontageFramesPerRow) * frameHeight;
		}

		/// <summary>
		/// Initialize the specified contentManager, spriteName, x and y.
		/// </summary>
		/// <returns>The initialize.</returns>
		/// <param name="contentManager">Content manager.</param>
		/// <param name="spriteName">Sprite name.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private void Initialize(ContentManager contentManager, string spriteName, int x, int y)
		{
			// load the animation strip
			strip = contentManager.Load<Texture2D>(spriteName);
			// calculate frame size
			frameWidth = strip.Width / GameConstants.MontageFramesPerRow;
			frameHeight = strip.Height / GameConstants.MontageNumRows;
			// set initial draw and source rectangles
			drawRectangle = new Rectangle(x - frameWidth/2, y-frameHeight/2, frameWidth, frameHeight);
			sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
			// set current and previous frames to 0
			currentFrame = 0;
			// initialize texture color data for collision detection
			textureData = new Color[frameWidth * frameHeight];
			strip.GetData(0, sourceRectangle, textureData,0,textureData.Length);
			// set initial sprite center
			center = new Point(drawRectangle.Center.X, drawRectangle.Center.Y);
			// use center to calculate original turret positions and save them
			laserTurret1 = new Point(center.X + GameConstants.LaserTurretXDist,
							center.Y + GameConstants.LaserTurretYDist);
			laserTurret2 = new Point(center.X - GameConstants.LaserTurretXDist,
			                      center.Y + GameConstants.LaserTurretYDist);
			plasmTurret = new Point(center.X + GameConstants.PlasmTurretXDist, 
			                     center.Y + GameConstants.PlasmTurretYDist);
			// initialize turrets positions
			laserPos1 = new Point(laserTurret1.X,laserTurret1.Y);
			laserPos2 = new Point(laserTurret2.X, laserTurret2.Y);
			plasmPos = new Point(plasmTurret.X, plasmTurret.Y);
		}

		/// <summary>
		/// Adjusts velocity
		/// </summary>
		private void SetVelocity()
		{
			// subtract 90 degrees as zero radians has a different referential
			velocity.X = (float)(Math.Cos(angle-MathHelper.PiOver2) * speed);
			velocity.Y = (float)(Math.Sin(angle-MathHelper.PiOver2) * speed);
		}

		/// <summary>
		/// Adjusts the angle.
		/// </summary>
		/// <returns>The angle.</returns>
		/// <param name="acceleration">Acceleration.</param>
		private void SpeedUp(float acceleration)
		{
			if (speed < GameConstants.F14MaximumSpeed)
			{
				previousSpeed = speed;
				speed += acceleration;
				if (previousSpeed < 0 && speed > 0)
				{
					// transition detected, reverse direction
					velocity.X *= -1;
					velocity.Y *= -1;
				}
			}
		}

		/// <summary>
		/// Speeds down.
		/// </summary>
		/// <param name="deacceleration">Deacceleration.</param>
		private void SpeedDown(float deacceleration)
		{
			if (speed > -GameConstants.F14MaximumSpeed/2)
			{
				previousSpeed = speed;
				speed -= deacceleration;
				if (previousSpeed > 0 && speed < 0)
				{
					// transition detected, reverse direction
					velocity.X *= -1;
					velocity.Y *= -1;
				}
			}
		}

		/// <summary>
		/// Rotates the right.
		/// </summary>
		/// <param name="amount">Amount.</param>
		private void RotateRight(int amount)
		{
			currentFrame += amount;
			if (currentFrame > (GameConstants.MontageNumFrames-1)) currentFrame = 0;
		}

		/// <summary>
		/// Rotates the left.
		/// </summary>
		/// <param name="amount">Amount.</param>
		private void RotateLeft(int amount)
		{
			currentFrame -= amount;
			if (currentFrame < 0) currentFrame = GameConstants.MontageNumFrames - 1;
		}

		/// <summary>
		/// Cycles the weapon.
		/// </summary>
		private void CycleWeapon()
		{
			if (weapon == ProjectileType.Laser)
			weapon = ProjectileType.Plasm;
			else weapon = ProjectileType.Laser;
		}

		#endregion
	}
}
