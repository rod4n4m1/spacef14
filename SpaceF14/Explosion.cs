using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// An animated explosion object
	/// </summary>
	public class Explosion
	{
		#region Fields

		// object location
		Rectangle drawRectangle;
		ExplosionType type;

		// animation strip info
		Texture2D strip;
		int frameWidth;
		int frameHeight;

		// fields used to track and draw animations
		Rectangle sourceRectangle;
		int currentFrame;
		int lastFrame;
		int elapsedFrameTime = 0;
		int frameTime;

		// playing or not
		bool playing = false;
		bool finished = false;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Explosion"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="spriteStrip">Sprite strip.</param>
		/// <param name="location">Location.</param>
		public Explosion(ExplosionType type, Texture2D spriteStrip, Point location) : 
		this(type, spriteStrip, location.X, location.Y, GameConstants.ExplosionTotalFrameMilliseconds)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Explosion"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="spriteStrip">Sprite strip.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Explosion(ExplosionType type, Texture2D spriteStrip, int x, int y) : 
		this(type, spriteStrip, x, y, GameConstants.ExplosionTotalFrameMilliseconds)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Explosion"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="spriteStrip">Sprite strip.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="frameTime">Frame time.</param>
		public Explosion(ExplosionType type, Texture2D spriteStrip, int x, int y, int frameTime)
		{
			this.type = type;
			// initialize animation to start at frame 0
			currentFrame = 0;
			// set frameTime
			this.frameTime = frameTime;
			// set last frame
			switch (type)
			{
				case(ExplosionType.TeddyBearDeath):
					lastFrame = GameConstants.BearExplosionNumFrames - 1;
					break;
				case(ExplosionType.TomcatDeath):
					lastFrame = GameConstants.TomcatExplosionNumFrames - 1;
					break;
				case(ExplosionType.ProjectileHit):
					lastFrame = GameConstants.PurpleExplosionNumFrames - 1;
					break;
				case(ExplosionType.RockDeath):
					lastFrame = GameConstants.RockExplosionNumFrames - 1;
					break;
				case(ExplosionType.BossHit):
					lastFrame = GameConstants.BlueExplosionNumFrames - 1;
					break;
				case(ExplosionType.TomcatHit):
					lastFrame = GameConstants.GreenExplosionNumFrames - 1;
					break;
				case(ExplosionType.BossDeath):
					lastFrame = GameConstants.BossExplosionNumFrames - 1;
					break;
				case(ExplosionType.BossClash):
					lastFrame = GameConstants.YellowExplosionNumFrames - 1;
					break;
			}
			Initialize(spriteStrip);
			Play(x, y);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the collision rectangle for the explosion
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
		}

		/// <summary>
		/// Gets whether or not the explosion is finished
		/// </summary>
		public bool Finished
		{
			get { return finished; }
		}

		/// <summary>
		/// Gets the explosion type
		/// </summary>
		public ExplosionType Type
		{
			get { return type; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Updates the explosion. This only has an effect if the explosion animation is playing
		/// </summary>
		/// <param name="gameTime">the game time</param>
		public void Update(GameTime gameTime)
		{
			if (playing)
			{
				// check for advancing animation frame
				elapsedFrameTime += gameTime.ElapsedGameTime.Milliseconds;
				if (elapsedFrameTime > frameTime)
				{
					// reset frame timer
					elapsedFrameTime = 0;
					// advance the animation
					if (currentFrame < lastFrame)
					{
						currentFrame++;
						SetSourceRectangleLocation(currentFrame);
					}
					else
					{
						// reached the end of the animation
						playing = false;
						finished = true;
					}
				}
			}
		}

		/// <summary>
		/// Draws the explosion. This only has an effect if the explosion animation is playing
		/// </summary>
		/// <param name="spriteBatch">the spritebatch</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (playing)
			{
				spriteBatch.Draw(strip, drawRectangle, sourceRectangle, Color.White);
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Loads the content for the explosion
		/// </summary>
		/// <param name="spriteStrip">the sprite strip for the explosion</param>
		private void Initialize(Texture2D spriteStrip)
		{
			// load the animation strip
			strip = spriteStrip;
			switch (type)
			{
				// calculate frame size
				case(ExplosionType.TeddyBearDeath):
					frameWidth = strip.Width / GameConstants.BearExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.BearExplosionNumRows;
					break;
				case(ExplosionType.TomcatDeath):
					frameWidth = strip.Width / GameConstants.TomcatExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.TomcatExplosionNumRows;
					break;
				case(ExplosionType.ProjectileHit):
					frameWidth = strip.Width / GameConstants.PurpleExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.PurpleExplosionNumRows;
					break;
				case(ExplosionType.RockDeath):
					frameWidth = strip.Width / GameConstants.RockExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.RockExplosionNumRows;
					break;
				case(ExplosionType.BossHit):
					frameWidth = strip.Width / GameConstants.BlueExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.BlueExplosionNumRows;
					break;
				case(ExplosionType.TomcatHit):
					frameWidth = strip.Width / GameConstants.GreenExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.GreenExplosionNumRows;
					break;
				case(ExplosionType.BossDeath):
					frameWidth = strip.Width / GameConstants.BossExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.BossExplosionNumRows;
					break;
				case(ExplosionType.BossClash):
					frameWidth = strip.Width / GameConstants.YellowExplosionFramesPerRow;
					frameHeight = strip.Height / GameConstants.YellowExplosionNumRows;
					break;
			}
			// set initial draw and source rectangles
			drawRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
			sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
		}

		/// <summary>
		/// Starts playing the animation for the explosion
		/// </summary>
		/// <param name="x">the x location of the center of the explosion</param>
		/// <param name="y">the y location of the center of the explosion</param>
		private void Play(int x, int y)
		{
			// reset tracking values
			playing = true;
			elapsedFrameTime = 0;
			currentFrame = 0;

			// set draw location and source rectangle
			drawRectangle.X = x - drawRectangle.Width / 2;
			drawRectangle.Y = y - drawRectangle.Height / 2;
			SetSourceRectangleLocation(currentFrame);
		}

		/// <summary>
		/// Sets the source rectangle location to correspond with the given frame
		/// </summary>
		/// <param name="frameNumber">the frame number</param>
		private void SetSourceRectangleLocation(int frameNumber)
		{
			switch (type)
			{
				// calculate X and Y based on frame number
				case(ExplosionType.TeddyBearDeath):
					sourceRectangle.X = (frameNumber % GameConstants.BearExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.BearExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.TomcatDeath):
					sourceRectangle.X = (frameNumber % GameConstants.TomcatExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.TomcatExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.ProjectileHit):
					sourceRectangle.X = (frameNumber % GameConstants.PurpleExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.PurpleExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.RockDeath):
					sourceRectangle.X = (frameNumber % GameConstants.RockExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.RockExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.BossHit):
					sourceRectangle.X = (frameNumber % GameConstants.BlueExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.BlueExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.TomcatHit):
					sourceRectangle.X = (frameNumber % GameConstants.GreenExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.GreenExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.BossDeath):
					sourceRectangle.X = (frameNumber % GameConstants.BossExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.BossExplosionFramesPerRow) * frameHeight;
					break;
				case(ExplosionType.BossClash):
					sourceRectangle.X = (frameNumber % GameConstants.YellowExplosionFramesPerRow) * frameWidth;
					sourceRectangle.Y = (frameNumber / GameConstants.YellowExplosionFramesPerRow) * frameHeight;
					break;
			}
		}
		#endregion
	}
}
