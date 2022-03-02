using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// A class for game bosses
	/// </summary>
	public class Boss
	{
		#region Fields

		BossType type;

		bool active = true;

		// drawing & animation support
		Texture2D strip;
		Texture2D rectangle;
		Color[] textureData;
		Rectangle drawRectangle;
		Rectangle sourceRectangle;
		Rectangle healthBar;
		int currentFrame;
		int frameWidth;
		int frameHeight;
		int framesPerRow;
		int numRows;
		int numFrames;
		int elapsedFrameTime;

		// speed & velocity information
		Vector2 velocity = Vector2.Zero;
		Vector2 location = Vector2.Zero;
		float speed;

		// movement behaviour
		String behaviour;
		bool moveLeft;
		int waitedTime;

		// spawn & shoot support
		int elapsedTime;
		int firingDelay;

		// health support
		int health;
		int initialHealth;

		// sound effects
		SoundEffect bounceSound;
		SoundEffect shootSound;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Boss"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="contentManager">Content manager.</param>
		/// <param name="stripName">Sprite name.</param>
		/// <param name="rectangle">Green rectangle.</param>
		/// <param name="location">Location.</param>
		/// <param name="behaviour">Behaviour.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="velocity">Velocity.</param>
		/// <param name="health">Health.</param>
		/// <param name="bounceSound">Bounce sound.</param>
		/// <param name="shootSound">Shoot sound.</param>
		public Boss(BossType type, ContentManager contentManager, string stripName, Texture2D rectangle, Vector2 location,
			String behaviour, float speed, Vector2 velocity, int health, SoundEffect bounceSound, SoundEffect shootSound)
		{
			this.type = type;
			LoadContent(contentManager, stripName, (int)location.X, (int)location.Y);
			this.rectangle = rectangle;
			this.health = health;
			this.initialHealth = health;
			this.bounceSound = bounceSound;
			this.shootSound = shootSound;
			this.behaviour = behaviour.ToLower();
			moveLeft = true;
			this.velocity = velocity;
			this.location = location;
			this.speed = speed;
			firingDelay = GetRandomFiringDelay();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets whether or not the boss is active
		/// </summary>
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		/// <summary>
		/// Gets and sets health of the boss
		/// </summary>
		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		/// <summary>
		/// Gets type of the boss
		/// </summary>
		public BossType Type
		{
			get { return type; }
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
		/// Gets the collision rectangle for the boss
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
		}

		/// <summary>
		/// Gets and sets the velocity of the teddy bear
		/// </summary>
		public Vector2 Velocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		/// <summary>
		/// Gets and sets the draw rectangle for the teddy bear
		/// </summary>
		public Rectangle DrawRectangle
		{
			get { return drawRectangle; }
			set { drawRectangle = value; }
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

		#region Public methods

		/// <summary>
		/// Updates the teddy bear's location, bouncing if necessary. Also has
		/// the teddy bear fire a projectile when it's time to
		/// </summary>
		/// <param name="gameTime">game time</param>
		public void Update(GameTime gameTime)
		{
			
			// move according to selected behaviour
			MoveBoss(gameTime);
			elapsedFrameTime += gameTime.ElapsedGameTime.Milliseconds;
			if (elapsedFrameTime > GameConstants.BossTotalFrameMilliseconds)
			{
				// reset frame timer
				elapsedFrameTime = 0;
				// advance the animation
				AnimateBoss();
			}
			// update life bar
			healthBar.X = (int)(drawRectangle.X + 0.2*frameWidth/2);
			healthBar.Y = drawRectangle.Y-7;
			// integer divison returns integer in C# :-(
			healthBar.Width = (int)((float)health / initialHealth * (frameWidth*0.8));
			// bounce as necessary if its not down forward pattern
			if (behaviour != "down_forward")
			{
				BounceTopBottom();
				BounceLeftRight();
			}
			// fire boss projectile as appropriate
			// use a timer
			elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
			if (elapsedTime > firingDelay)
			{
				elapsedTime = 0;
				firingDelay = GetRandomFiringDelay();
				Game1.AddProjectile(new Projectile(ProjectileType.Boss1, Game1.GetProjectileSprite(ProjectileType.Boss1),
												   drawRectangle.X + frameWidth / 2,
												   drawRectangle.Y + frameHeight * 2 / 3, 0,
												   Game1.GetDirection2Tomcat(new Vector2(drawRectangle.Center.X,
																						 drawRectangle.Center.Y))));
				shootSound.Play(volume: GameConstants.ShotVolume, pitch: 0.0f, pan: 0.0f);
			}
		}

		/// <summary>
		/// Draws the boss
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(strip, drawRectangle, sourceRectangle, Color.White);
			spriteBatch.Draw(rectangle, healthBar, Color.LightCyan);
		}

		/// <summary>
		/// Takes the damage.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void TakeDamage(int amount)
		{
			health -= amount;
			if (health < 0) health = 0;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Moves the boss.
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		private void MoveBoss(GameTime gameTime)
		{
			// move the boss based on the selected behaviour
			float angle;
			switch (behaviour)
			{
				case("down_forward"):
					// move toward to any side in the same row, 
					// then go down and move toward to reverse side in the same row
					if (moveLeft)
					{
						drawRectangle.X -= (int)(speed * gameTime.ElapsedGameTime.Milliseconds);
						if (drawRectangle.X < 0)
						{
							drawRectangle.Y += GameConstants.BossDescentMovement;
							if (drawRectangle.Y > GameConstants.DisplayHeight)
								drawRectangle.Y = GameConstants.SpawnBorderSize;
							drawRectangle.X = 0;
							moveLeft = false;
						}
					}
					else
						// move to right
					{
						drawRectangle.X += (int)(speed * gameTime.ElapsedGameTime.Milliseconds);
						if (drawRectangle.X + drawRectangle.Width > GameConstants.DisplayWidth)
						{
							drawRectangle.Y += GameConstants.BossDescentMovement;
							if (drawRectangle.Y > GameConstants.DisplayHeight)
								drawRectangle.Y = GameConstants.SpawnBorderSize;
							drawRectangle.X = GameConstants.DisplayWidth - drawRectangle.Width;
							moveLeft = true;
						}
					}
					location.X = drawRectangle.X + drawRectangle.Width / 2;
					location.Y = drawRectangle.Y + drawRectangle.Height / 2;
					break;
				case("erratic"):
					// move with initial velocity, then every 2s change the direction
					waitedTime += gameTime.ElapsedGameTime.Milliseconds;
					if (waitedTime > 2000)
					{
						waitedTime = 0;
						// change direction
						angle = (float)(Math.PI * RandomNumberGenerator.NextFloat(2));
						velocity.X = (float)(Math.Cos(angle) * speed);
						velocity.Y = (float)(Math.Sin(angle) * speed);
					}
					location.X += velocity.X * gameTime.ElapsedGameTime.Milliseconds;
					location.Y += velocity.Y * gameTime.ElapsedGameTime.Milliseconds;
					drawRectangle.X = (int)(location.X - drawRectangle.Width / 2);
					drawRectangle.Y = (int)(location.Y - drawRectangle.Height / 2);
					break;
				case("stalker"):
					// persue after tomcat at passed speed
					// use Game1.GetDirection2Tomcat to know where the Tomcat is
					waitedTime += gameTime.ElapsedGameTime.Milliseconds;
					if (waitedTime > 2000)
					{	
						waitedTime = 0;
						velocity = Game1.GetDirection2Tomcat(new Vector2(drawRectangle.Center.X,
						                                                 drawRectangle.Center.Y));
					}
					location.X += velocity.X * speed * gameTime.ElapsedGameTime.Milliseconds;
					location.Y += velocity.Y * speed * gameTime.ElapsedGameTime.Milliseconds;
					drawRectangle.X = (int)(location.X - drawRectangle.Width/2);
					drawRectangle.Y = (int)(location.Y - drawRectangle.Height/2);
					break;
				case("default"):
					// move according to initial velocity, speed is ignored
					location.X += velocity.X * gameTime.ElapsedGameTime.Milliseconds;
					location.Y += velocity.Y * gameTime.ElapsedGameTime.Milliseconds;
					drawRectangle.X = (int)(location.X - drawRectangle.Width / 2);
					drawRectangle.Y = (int)(location.Y - drawRectangle.Height / 2);
					break;
			}
		}

		/// <summary>
		/// Animates the boss.
		/// </summary>
		private void AnimateBoss()
		{
			
			if (currentFrame < numFrames - 1)
			{
					currentFrame++;
			}
			else
			{
				// reached the end of the animation, start it over
				currentFrame = 0;
			}
			sourceRectangle.X = (currentFrame % framesPerRow) * frameWidth;
			sourceRectangle.Y = (currentFrame / framesPerRow) * frameHeight;
		}

		/// <summary>
		/// Loads the content for the boss
		/// </summary>
		/// <param name="contentManager">the content manager to use</param>
		/// <param name="stripName">the name of the sprite strip for the boss</param>
		/// <param name="x">the x location of the center of the boss</param>
		/// <param name="y">the y location of the center of the boss</param>
		private void LoadContent(ContentManager contentManager, string stripName,
		                         int x, int y)
		{
			// load animation strip
			strip = contentManager.Load<Texture2D>(stripName);
			// initialize proper constants based on boss type
			switch (type)
			{
				case(BossType.Boss1):
					framesPerRow = GameConstants.Boss1FramesPerRow;
					numRows = GameConstants.Boss1NumRows;
					numFrames = GameConstants.Boss1NumFrames;
					break;
				case(BossType.Boss2):
					framesPerRow = GameConstants.Boss2FramesPerRow;
					numRows = GameConstants.Boss2NumRows;
					numFrames = GameConstants.Boss2NumFrames;
					break;
				case(BossType.Boss3):
					framesPerRow = GameConstants.Boss3FramesPerRow;
					numRows = GameConstants.Boss3NumRows;
					numFrames = GameConstants.Boss3NumFrames;
					break;
				case(BossType.Boss4):
					framesPerRow = GameConstants.Boss4FramesPerRow;
					numRows = GameConstants.Boss4NumRows;
					numFrames = GameConstants.Boss4NumFrames;
					break;
				case(BossType.Boss5):
					framesPerRow = GameConstants.Boss5FramesPerRow;
					numRows = GameConstants.Boss5NumRows;
					numFrames = GameConstants.Boss5NumFrames;
					break;
				case(BossType.FinalBoss):
					framesPerRow = GameConstants.Boss6FramesPerRow;
					numRows = GameConstants.Boss6NumRows;
					numFrames = GameConstants.Boss6NumFrames;
					break;
			}
			// calculate frame size
			frameWidth = strip.Width / framesPerRow;
			frameHeight = strip.Height / numRows;
			// set initial draw and source rectangles
			drawRectangle = new Rectangle(x, y, frameWidth, frameHeight);
			sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
			// set healthbar
			healthBar = new Rectangle(x, y-7, frameWidth, 7);
			// set texture data for collision detection
			textureData = new Color[frameWidth * frameHeight];
			strip.GetData(0, sourceRectangle, textureData,0,textureData.Length);
		}

		/// <summary>
		/// Bounces the boss off the top and bottom window borders if necessary
		/// </summary>
		private void BounceTopBottom()
		{
			if (drawRectangle.Y < 0)
			{
				// bounce off top
				drawRectangle.Y = 0;
				velocity.Y *= -1;
				bounceSound.Play(volume: GameConstants.BounceVolume, pitch: 0.0f, pan: 0.0f);
			}
			else if ((drawRectangle.Y + drawRectangle.Height) > GameConstants.DisplayHeight)
			{
				// bounce off bottom
				drawRectangle.Y = GameConstants.DisplayHeight - drawRectangle.Height;
				velocity.Y *= -1;
				bounceSound.Play(volume: GameConstants.BounceVolume, pitch: 0.0f, pan: 0.0f);
			}
		}
		/// <summary>
		/// Bounces the boss off the left and right window borders if necessary
		/// </summary>
		private void BounceLeftRight()
		{
			if (drawRectangle.X < 0)
			{
				// bounc off left
				drawRectangle.X = 0;
				velocity.X *= -1;
				bounceSound.Play(volume: GameConstants.BounceVolume, pitch: 0.0f, pan: 0.0f);
			}
			else if ((drawRectangle.X + drawRectangle.Width) > GameConstants.DisplayWidth)
			{
				// bounce off right
				drawRectangle.X = GameConstants.DisplayWidth - drawRectangle.Width;
				velocity.X *= -1;
				bounceSound.Play(volume: GameConstants.BounceVolume, pitch: 0.0f, pan: 0.0f);
			}
		}

		/// <summary>
		/// Gets a random firing delay between MIN_FIRING_DELAY and
		/// MIN_FIRING_DELY + FIRING_RATE_RANGE
		/// </summary>
		/// <returns>the random firing delay</returns>
		private int GetRandomFiringDelay()
		{
			return GameConstants.BaseDelay*(LevelsData.Levels[Game1.GetLevel(), 35] +
			        RandomNumberGenerator.Next(LevelsData.Levels[Game1.GetLevel(), 36]));
		}

		#endregion
	}
}
