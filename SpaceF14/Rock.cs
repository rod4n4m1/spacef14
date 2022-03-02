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
	public class Rock
	{
		#region Fields

		bool active = true;

		RockType type;

		// drawing support
		Texture2D sprite;
		Color[] textureData;
		Rectangle drawRectangle;

		// velocity information
		Vector2 velocity = new Vector2(0, 0);

		// health support
		int health;


		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a rock centered on the given x and y with the
		///  given velocity
		/// </summary>
		/// <param name="contentManager">the content manager for loading content</param>
		/// <param name="spriteName">the name of the sprite for the rock</param>
		/// <param name="x">the x location of the center of the rock</param>
		/// <param name="y">the y location of the center of the rock</param>
		/// <param name="velocity">the velocity of the rock</param>
		public Rock(RockType type, ContentManager contentManager, string spriteName, int x, int y,
			Vector2 velocity, int health)
		{
			this.type = type;
			LoadContent(contentManager, spriteName, x, y);
			this.velocity = velocity;
			this.health = health;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets whether or not the rock is active
		/// </summary>
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		/// <summary>
		/// Gets and sets health of the rock
		/// </summary>
		public int Health
		{
			get { return health; }
		}

		/// <summary>
		/// Gets type of the rock
		/// </summary>
		public RockType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the location of the rock
		/// </summary>
		public Point Location
		{
			get { return drawRectangle.Center; }
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
		/// Sets the x location of the center of the rock
		/// </summary>
		public int X
		{
			set { drawRectangle.X = value - drawRectangle.Width / 2; }
		}

		/// <summary>
		/// Sets the y location of the center of the rock
		/// </summary>
		public int Y
		{
			set { drawRectangle.Y = value - drawRectangle.Height / 2; }
		}

		/// <summary>
		/// Gets the collision rectangle for the rock
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
		}

		/// <summary>
		/// Gets and sets the velocity of the rock
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
		/// Updates the rock's location.
		/// </summary>
		/// <param name="gameTime">game time</param>
		public void Update(GameTime gameTime)
		{
			// move the rock
			drawRectangle.X += (int)(velocity.X * gameTime.ElapsedGameTime.Milliseconds);
			drawRectangle.Y += (int)(velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

			// check for outside game window
			if (drawRectangle.Y > GameConstants.DisplayHeight ||
				drawRectangle.Y + sprite.Height < 0 ||
			    drawRectangle.X > GameConstants.DisplayWidth ||
				drawRectangle.X + sprite.Width < 0)
			{
				active = false;
			}
		}

		/// <summary>
		/// Draws the rock
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
		/// Loads the content for the rock
		/// </summary>
		/// <param name="contentManager">the content manager to use</param>
		/// <param name="spriteName">the name of the sprite for the rock</param>
		/// <param name="x">the x location of the center of the rock</param>
		/// <param name="y">the y location of the center of the rock</param>
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

		#endregion
	}
}
