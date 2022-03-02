using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// An animated explosion object
	/// </summary>
	public class Sign
	{
		#region Fields

		// object properties
		SignType type;
		Color color;
		Vector2 location;
		Vector2 size;
		SpriteFont font;
		string message;

		// fields used to track animation
		int currentFrame;
		int lastFrame;
		int elapsedFrameTime = 0;

		// playing or not
		bool playing = false;
		bool lightOn = false;
		bool finished = false;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Sign"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="font">Font.</param>
		/// <param name="message">Message.</param>
		/// <param name="color">Color.</param>
		/// <param name="location">Location.</param>
		public Sign(SignType type, SpriteFont font, string message, Color color, Point location): this(type, font, message, color, location.X, location.Y)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Sign"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="font">Font.</param>
		/// <param name="message">Message.</param>
		/// <param name="color">Color.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Sign(SignType type, SpriteFont font, string message, Color color, int x, int y)
		{
			this.type = type;
			this.font = font;
			this.message = message;
			this.color = color;
			size = font.MeasureString(message);
			location.X = x - size.X / 2;
			location.Y = y - size.Y / 2;
			currentFrame = 0;
			lastFrame = GameConstants.SignNumFrames;
			playing = true;
			lightOn = true;
			elapsedFrameTime = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets whether or not the sign is finished
		/// </summary>
		public bool Finished
		{
			get { return finished; }
		}

		/// <summary>
		/// Gets the sign type
		/// </summary>
		public SignType Type
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
				elapsedFrameTime += gameTime.ElapsedGameTime.Milliseconds;
				if (elapsedFrameTime > GameConstants.SignTotalFrameMilliseconds)
				{
					// reset frame timer
					elapsedFrameTime = 0;
					// advance the animation
					if (currentFrame < lastFrame)
					{
						currentFrame++;
						MoveSign();
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
			if (playing && lightOn)
			{
				spriteBatch.DrawString(font, message, location, color);
			}
		}

		#endregion

		#region Private methods

		private void MoveSign()
		{
			switch (type)
			{
				// calculate X and Y based on sign type
				case (SignType.Emerging):
					location.Y -= 3;
					break;
				case (SignType.Blinking):
					lightOn = !lightOn;
					break;
				case (SignType.Colorful):
					location.Y -= 3;
					color = GameConstants.ColorfulSign[RandomNumberGenerator.Next(GameConstants.ColorfulSign.Length)];
					break;
			}
		}

		#endregion
	}
}
