using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// An animated pickup object
	/// </summary>
	public class Pickup
	{
		#region Fields

		// object location & velocity
		Rectangle drawRectangle;
		PickupType type;
		Vector2 velocity;
		int corner;

		// animation strip info
		Texture2D strip;
		Color[] textureData;
		int frameWidth;
		int frameHeight;

		// fields used to track and draw animations
		Rectangle sourceRectangle;
		int currentFrame;
		int elapsedFrameTime = 0;

		// playing or not
		bool active = false;

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Pickup"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="spriteStrip">Sprite strip.</param>
		public Pickup(PickupType type, Texture2D spriteStrip) : this(type, spriteStrip, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Pickup"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="spriteStrip">Sprite strip.</param>
		/// <param name="location">Location.</param>
		public Pickup(PickupType type, Texture2D spriteStrip, Point location) : this(type, spriteStrip, location.X, location.Y)
		{
		}

		/// <summary>
		/// Constructs a new pickup object
		/// </summary>
		/// <param name="spriteStrip">the sprite strip for the pickup</param>
		/// <param name="x">the x location of the center of the pickup</param>
		/// <param name="y">the y location of the center of the pickup</param>
		public Pickup(PickupType type, Texture2D spriteStrip, int x, int y)
		{
			this.type = type;
			// initialize animation to start at frame 0
			currentFrame = 0;

			Initialize(spriteStrip);
			Play(x, y);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets whether or not the pickup is active
		/// </summary>
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}
		/// <summary>
		/// Gets the collision rectangle for the pickup
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
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
		/// Gets the projectile type
		/// </summary>
		public PickupType Type
		{
			get { return type; }
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
		/// Updates the pickup. This only has an effect if the pickup animation is playing
		/// </summary>
		/// <param name="gameTime">the game time</param>
		public void Update(GameTime gameTime)
		{
			if (active)
			{
				// check for advancing animation frame
				elapsedFrameTime += gameTime.ElapsedGameTime.Milliseconds;

				// move the pickup
				drawRectangle.X += (int)(velocity.X * gameTime.ElapsedGameTime.Milliseconds);
				drawRectangle.Y += (int)(velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

				if (elapsedFrameTime > GameConstants.PickupTotalFrameMilliseconds)
				{
					// reset frame timer
					elapsedFrameTime = 0;

					// advance the animation
					if (type == PickupType.RedOrb)
						if (currentFrame < GameConstants.RedOrbNumFrames - 1)
						{
							currentFrame++;
						}
						else
						{
							// reached the end of the animation, start it over
							currentFrame = 0;
						}
					else if (type == PickupType.BlueOrb)
					{
						if (currentFrame < GameConstants.BlueOrbNumFrames - 1)
						{
							currentFrame++;
						}
						else
						{
							// reached the end of the animation, start it over
							currentFrame = 0;
						}
					}
					else if (type == PickupType.GreenOrb)
					{
						if (currentFrame < GameConstants.GreenOrbNumFrames - 1)
						{
							currentFrame++;
						}
						else
						{
							// reached the end of the animation, start it over
							currentFrame = 0;
						}
					}
					else if (type == PickupType.GoldOrb)
					{
						if (currentFrame < GameConstants.GoldOrbNumFrames - 1)
						{
							currentFrame++;
						}
						else
						{
							// reached the end of the animation, start it over
							currentFrame = 0;
						}
					}
					SetSourceRectangleLocation(currentFrame);
				}
			}
			// check for outside game window
			if (drawRectangle.Y > GameConstants.DisplayHeight ||
				drawRectangle.Y + strip.Height < 0 ||
				drawRectangle.X > GameConstants.DisplayWidth ||
				drawRectangle.X + strip.Width < 0)
			{
				active = false;
			}
		}

		/// <summary>
		/// Draws the pickup. This only has an effect if the pickup animation is playing
		/// </summary>
		/// <param name="spriteBatch">the spritebatch</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (active)
			{
				spriteBatch.Draw(strip, drawRectangle, sourceRectangle, Color.White);
			}
		}

		#endregion

		#region Private methods
		/// <summary>
		/// Loads the content for the pickup
		/// </summary>
		/// <param name="spriteStrip">the sprite strip for the pickup</param>
		private void Initialize(Texture2D spriteStrip)
		{
			// load the animation strip
			strip = spriteStrip;

			if (type == PickupType.RedOrb)
			{
				// calculate frame size
				frameWidth = strip.Width / GameConstants.RedOrbFramesPerRow;
				frameHeight = strip.Height / GameConstants.RedOrbNumRows;
			}
			else if (type == PickupType.BlueOrb)
			{
				// calculate frame size
				frameWidth = strip.Width / GameConstants.BlueOrbFramesPerRow;
				frameHeight = strip.Height / GameConstants.BlueOrbNumRows;
			}
			else if (type == PickupType.GreenOrb)
			{
				// calculate frame size
				frameWidth = strip.Width / GameConstants.GreenOrbFramesPerRow;
				frameHeight = strip.Height / GameConstants.GreenOrbNumRows;
			}
			else if (type == PickupType.GoldOrb)
			{
				// calculate frame size
				frameWidth = strip.Width / GameConstants.GoldOrbFramesPerRow;
				frameHeight = strip.Height / GameConstants.GoldOrbNumRows;
			}
			// set initial draw and source rectangles
			drawRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
			sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);

			// set velocity
			velocity = GameConstants.PickupVelocity;

			// set texture data for collision detection
			textureData = new Color[frameWidth * frameHeight];
			strip.GetData(0, sourceRectangle, textureData,0,textureData.Length);
		}

		/// <summary>
		/// Starts playing the animation for the pickup
		/// </summary>
		/// <param name="x">the x location of the center of the pickup</param>
		/// <param name="y">the y location of the center of the pickup</param>
		private void Play(int x, int y)
		{
			// reset tracking values
			active = true;
			elapsedFrameTime = 0;
			currentFrame = 0;

			if (x == 0 && y == 0)
			{
				corner = RandomNumberGenerator.Next(4);
				switch (corner)
				{
					case(0):
						x = GameConstants.DisplayOffset;
						y = GameConstants.DisplayOffset;
						break;
					case (1):
						x = GameConstants.DisplayWidth - GameConstants.DisplayOffset;
						y = GameConstants.DisplayOffset;
						velocity.X *= -1;
						break;
					case (2):
						x = GameConstants.DisplayWidth - GameConstants.DisplayOffset;
						y = GameConstants.DisplayHeight - GameConstants.DisplayOffset;
						velocity.X *= -1;
						velocity.Y *= -1;
						break;
					case (3):
						x = GameConstants.DisplayOffset;
						y = GameConstants.DisplayHeight - GameConstants.DisplayOffset;
						velocity.Y *= -1;
						break;
				}
			}

			// set draw location and source rectangle
			drawRectangle.X = x - drawRectangle.Width / 2;
			drawRectangle.Y = y - drawRectangle.Height / 2;
			SetSourceRectangleLocation(currentFrame);
			strip.GetData(0, sourceRectangle, textureData,0,textureData.Length);
		}

		/// <summary>
		/// Sets the source rectangle location to correspond with the given frame
		/// </summary>
		/// <param name="frameNumber">the frame number</param>
		private void SetSourceRectangleLocation(int frameNumber)
		{
			if (type == PickupType.RedOrb)
			{
				// calculate X and Y based on frame number
				sourceRectangle.X = (frameNumber % GameConstants.RedOrbFramesPerRow) * frameWidth;
				sourceRectangle.Y = (frameNumber / GameConstants.RedOrbFramesPerRow) * frameHeight;
			}
			else if (type == PickupType.BlueOrb)
			{
				// calculate X and Y based on frame number
				sourceRectangle.X = (frameNumber % GameConstants.BlueOrbFramesPerRow) * frameWidth;
				sourceRectangle.Y = (frameNumber / GameConstants.BlueOrbFramesPerRow) * frameHeight;
			}
			else if (type == PickupType.GreenOrb)
			{
				// calculate X and Y based on frame number
				sourceRectangle.X = (frameNumber % GameConstants.GreenOrbFramesPerRow) * frameWidth;
				sourceRectangle.Y = (frameNumber / GameConstants.GreenOrbFramesPerRow) * frameHeight;
			}
			else if (type == PickupType.GoldOrb)
			{
				// calculate X and Y based on frame number
				sourceRectangle.X = (frameNumber % GameConstants.GoldOrbFramesPerRow) * frameWidth;
				sourceRectangle.Y = (frameNumber / GameConstants.GoldOrbFramesPerRow) * frameHeight;
			}
		}
		#endregion
	}
}
