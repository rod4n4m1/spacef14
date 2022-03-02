using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceF14
{
	/// <summary>
	/// A class for a projectile
	/// </summary>
	public class Projectile
	{
		#region Fields

		bool active = true;
		ProjectileType type;

		// drawing and animation support
		Texture2D sprite;
		Rectangle drawRectangle;
		Rectangle sourceRectangle;
		Color[] textureData;
		int frameNumber;
		int frameWidth;
		int frameHeight;

		// movement information
		float angle;
		Vector2 direction;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Projectile"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="location">Location.</param>
		public Projectile(ProjectileType type, Texture2D sprite, Point location) : 
		this(type, sprite, location.X, location.Y, 0, Vector2.Zero)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Projectile"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Projectile(ProjectileType type, Texture2D sprite, int x, int y) :
		this(type, sprite, x, y, 0, Vector2.Zero)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Projectile"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="location">Location.</param>
		/// <param name="frameNumber">Frame number.</param>
		public Projectile(ProjectileType type, Texture2D sprite, Point location,
						  int frameNumber) : this(type, sprite, location.X, location.Y, frameNumber, Vector2.Zero)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SpaceF14.Projectile"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="frameNumber">Frame number.</param>
		/// <param name="direction">Direction.</param>
		public Projectile(ProjectileType type, Texture2D sprite, int x, int y,
						  int frameNumber, Vector2 direction)
		{
			this.type = type;
			this.sprite = sprite;
            this.frameNumber = frameNumber;
			// tomcat projectiles
			if (type == ProjectileType.Laser || type == ProjectileType.Plasm)
			{
				// calculates angle based on current frame
				angle = (float)(frameNumber* Math.PI) / 90;
				// calculate frame width and height
				frameWidth = sprite.Width / GameConstants.MontageFramesPerRow;
				frameHeight = sprite.Height / GameConstants.MontageNumRows;
				// calculate direction
				// need to subtract 90 degrees as zero has a different reference
                this.direction.X = (float)(Math.Cos(angle - MathHelper.PiOver2) * frameWidth);
				this.direction.Y = (float)(Math.Sin(angle - MathHelper.PiOver2) * frameHeight);
				this.direction.Normalize();
				// set draw and source rectangles
				drawRectangle = new Rectangle(x-frameWidth/2, y-frameHeight/2, frameWidth, frameHeight);
				sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
				sourceRectangle.X = (frameNumber % GameConstants.MontageFramesPerRow) * frameWidth;
				sourceRectangle.Y = (frameNumber / GameConstants.MontageFramesPerRow) * frameHeight;
				// initialize texture color data
				textureData = new Color[frameWidth * frameHeight];
				sprite.GetData(0, sourceRectangle, textureData,0,textureData.Length);
			}
			// teddy or boss projectiles
			else
			{
				this.direction = direction;
				// set draw rectangles
				drawRectangle = new Rectangle(x, y, sprite.Width, sprite.Height);
				// initialize texture color data
				textureData = new Color[sprite.Width * sprite.Height];
				sprite.GetData(textureData);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets whether or not the projectile is active
		/// </summary>
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		/// <summary>
		/// Gets the projectile type
		/// </summary>
		public ProjectileType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the collision rectangle for the projectile
		/// </summary>
		public Rectangle CollisionRectangle
		{
			get { return drawRectangle; }
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
		/// Updates the projectile's location and makes inactive when it
		/// leaves the game window
		/// </summary>
		public void Update(GameTime gameTime)
		{
			// move projectile according to type
			// i dont know why, but angle here means 45 degrees ahead of tomcat
			switch (type)
			{
				case ProjectileType.Teddy:
					drawRectangle.Y += (int)(LevelsData.Levels[Game1.GetLevel(), 38] * GameConstants.BaseSpeed * gameTime.ElapsedGameTime.Milliseconds);
					break;
				case ProjectileType.Laser:
					drawRectangle.X += (int)(direction.X * GameConstants.LaserProjectileSpeed * gameTime.ElapsedGameTime.Milliseconds);
					drawRectangle.Y += (int)(direction.Y * GameConstants.LaserProjectileSpeed * gameTime.ElapsedGameTime.Milliseconds);
					break;
				case ProjectileType.Plasm:
					drawRectangle.X += (int)(direction.X * GameConstants.PlasmProjectileSpeed * gameTime.ElapsedGameTime.Milliseconds);
					drawRectangle.Y += (int)(direction.Y * GameConstants.PlasmProjectileSpeed * gameTime.ElapsedGameTime.Milliseconds);
					break;
				case ProjectileType.Boss1:
					drawRectangle.X += (int)(direction.X * LevelsData.Levels[Game1.GetLevel(), 39] * GameConstants.BaseSpeed * gameTime.ElapsedGameTime.Milliseconds);
					drawRectangle.Y += (int)(direction.Y * LevelsData.Levels[Game1.GetLevel(), 39] * GameConstants.BaseSpeed * gameTime.ElapsedGameTime.Milliseconds);
					break;
			}

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
		/// Draws the projectile
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			//spriteBatch.Draw(sprite, drawRectangle, Color.White);
			if (type == ProjectileType.Laser || type == ProjectileType.Plasm)
			{
				spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
			}
			else
			{
				spriteBatch.Draw(sprite, drawRectangle, Color.White);
			}
		}

		#endregion
	}
}