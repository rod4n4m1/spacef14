using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// A class for a teddy bear
	/// </summary>
	public class TeddyBear
	{
		#region Fields

		bool active = true;

		BearType type;

		// drawing support
		Texture2D sprite;
		Color[] textureData;
		Rectangle drawRectangle;

		// velocity information
		Vector2 velocity = new Vector2(0, 0);

		// shooting support
		int elapsedShotMilliseconds = 0;
		int firingDelay;

		// health support
		int health;

		// sound effects
		SoundEffect bounceSound;
		SoundEffect shootSound;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.TeddyBear"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="contentManager">Content manager.</param>
		/// <param name="spriteName">Sprite name.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="velocity">Velocity.</param>
		/// <param name="health">Health.</param>
		/// <param name="bounceSound">Bounce sound.</param>
		/// <param name="shootSound">Shoot sound.</param>
		public TeddyBear(BearType type, ContentManager contentManager, string spriteName, int x, int y,
			Vector2 velocity, int health, SoundEffect bounceSound, SoundEffect shootSound)
		{
			this.type = type;
			LoadContent(contentManager, spriteName, x, y);
			this.velocity = velocity;
			this.health = health;
			this.bounceSound = bounceSound;
			this.shootSound = shootSound;
			firingDelay = GetRandomFiringDelay();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets whether or not the teddy bear is active
		/// </summary>
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		/// <summary>
		/// Gets and sets health of the teddy bear
		/// </summary>
		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		/// <summary>
		/// Gets type of the teddy bear
		/// </summary>
		public BearType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the location of the teddy bear
		/// </summary>
		public Point Location
		{
			get { return drawRectangle.Center; }
		}



		/// <summary>
		/// Sets the x location of the center of the teddy bear
		/// </summary>
		public int X
		{
			set { drawRectangle.X = value - drawRectangle.Width / 2; }
		}

		/// <summary>
		/// Sets the y location of the center of the teddy bear
		/// </summary>
		public int Y
		{
			set { drawRectangle.Y = value - drawRectangle.Height / 2; }
		}

		/// <summary>
		/// Gets the collision rectangle for the teddy bear
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
		/// Gets the center.
		/// </summary>
		/// <value>The center.</value>
		public Point Center
		{
			get { return drawRectangle.Center; }
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
			// move the teddy bear
			drawRectangle.X += (int)(velocity.X * gameTime.ElapsedGameTime.Milliseconds);
			drawRectangle.Y += (int)(velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

			// bounce as necessary
			BounceTopBottom();
			BounceLeftRight();

			// fire projectile as appropriate
			// timer concept (for animations) introduced in Chapter 7
			elapsedShotMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
			if (elapsedShotMilliseconds > firingDelay)
			{
				elapsedShotMilliseconds = 0;
				firingDelay = GetRandomFiringDelay();
				Game1.AddProjectile(new Projectile(ProjectileType.Teddy, Game1.GetProjectileSprite(ProjectileType.Teddy),
												   drawRectangle.X + sprite.Width / 2,
												   drawRectangle.Y + sprite.Height / 2 + GameConstants.BearProjectileOffset));
				shootSound.Play(volume: GameConstants.ShotVolume, pitch: 0.0f, pan: 0.0f);
				Game1.UpdateStats(StatsType.enemyFiredShots,1);
			}

		}

		/// <summary>
		/// Draws the teddy bear
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(sprite, drawRectangle, Color.White);
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
		/// Loads the content for the teddy bear
		/// </summary>
		/// <param name="contentManager">the content manager to use</param>
		/// <param name="spriteName">the name of the sprite for the teddy bear</param>
		/// <param name="x">the x location of the center of the teddy bear</param>
		/// <param name="y">the y location of the center of the teddy bear</param>
		private void LoadContent(ContentManager contentManager, string spriteName,
			int x, int y)
		{
			// load content and set remainder of draw rectangle
			sprite = contentManager.Load<Texture2D>(spriteName);
			drawRectangle = new Rectangle(x - sprite.Width / 2,
				y - sprite.Height / 2, sprite.Width,
				sprite.Height);
			textureData = new Color[sprite.Width * sprite.Height];
			sprite.GetData(textureData);
		}

		/// <summary>
		/// Bounces the teddy bear off the top and bottom window borders if necessary
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
		/// Bounces the teddy bear off the left and right window borders if necessary
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
			return GameConstants.BaseDelay*(LevelsData.Levels[Game1.GetLevel(),34] +
			        RandomNumberGenerator.Next(LevelsData.Levels[Game1.GetLevel(), 35]));
		}

		#endregion
	}
}
