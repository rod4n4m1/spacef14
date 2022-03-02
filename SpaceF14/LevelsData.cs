﻿using System;

using Microsoft.Xna.Framework;

namespace SpaceF14
{
	/// <summary>
	/// All constants used on each level to control game objects behavior
	/// </summary>
	public static class LevelsData
	{
		/* Column		Description
		 * 0			Level indicator
		 * 1			Level type
		 * 2			Max Number of Asteroids
		 * 3			Max Number of Meteors
		 * 4			Max Number of Yellow Bears
		 * 5			Max Number of Green Bears
		 * 6			Max Number of Pickups
		 * 7			Yellow Bear Points
		 * 8			Green Bear Points
		 * 8			Bear Projectile Points
		 * 10			Asteroid Points
		 * 11			Meteor Points
		 * 12			Bear Projectile Damage
		 * 13			Boss Projectile Damage
		 * 14			Yellow Bear Collision Damage
		 * 15			Green Bear Collision Damage
		 * 16			Asteroid Collision Damage
		 * 17			Meteor Collision Damage
		 * 18			Boss Collision Damage
		 * 19			Yellow Bear Initial Health
		 * 20			Green Bear Initial Health
		 * 21			Asteroid Initial Health
		 * 22			Meteor Initial Health
		 * 23			Boss Initial Health
		 * 24			Yellow Bear Min Speed (Factor of 0.05)
		 * 25			Yellow Bear Max Speed (Factor of 0.05)
		 * 26			Green Bear Min Speed (Factor of 0.05)
		 * 27			Green Bear Max Speed (Factor of 0.05)
		 * 28 			Asteroid Min Speed (Factor of 0.05)
		 * 29			Asteroid Max Speed (Factor of 0.05)
		 * 30			Meteor Min Speed (Factor of 0.05)
		 * 31			Meteor Max Speed (Factor of 0.05)
		 * 32			Boss Min Speed (Factor of 0.05)
		 * 33			Boss Max Speed (Factor of 0.05)
		 * 34			Bear Min Firing Delay (Factor of 500)
		 * 35			Bear Max Firing Delay (Factor of 500)
		 * 36			Boss Min Firing Delay (Factor of 500)
		 * 37			Boss Max Firing Delay (Factor of 500)
		 * 38			Bear Projectile Speed (Factor of 0.05)
		 * 39			Boss Projectile Speed (Factor of 0.05)
		 * 40			Level cap in points
		*/
		public static int[,] Levels =
		{
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 0, 2, 0, 1, 0, 2, 15, 25, 20, 5, 10, 5, 0, 10, 13, 15, 20, 0, 5, 10, 5, 30, 0, 2, 2, 3, 4, 2, 3, 3, 4, 0, 0, 5, 6, 0, 0, 4, 0, 700},
			{2, 0, 2, 0, 1, 0, 2, 15, 25, 20, 5, 10, 5, 0, 10, 13, 15, 20, 0, 5, 10, 5, 30, 0, 2, 2, 3, 4, 2, 3, 3, 4, 0, 0, 5, 6, 0, 0, 4, 0, 800},
			{3, 0, 2, 0, 1, 0, 2, 15, 25, 20, 5, 10, 5, 0, 10, 13, 15, 20, 0, 5, 10, 5, 30, 0, 2, 2, 3, 4, 2, 3, 3, 4, 0, 0, 5, 6, 0, 0, 4, 0, 900},
			{4, 0, 2, 0, 1, 0, 2, 15, 25, 20, 5, 10, 5, 0, 10, 13, 15, 20, 0, 5, 10, 5, 30, 0, 2, 2, 3, 4, 2, 3, 3, 4, 0, 0, 5, 6, 0, 0, 4, 0, 50},
			{5, 1, 0, 0, 2, 0, 3, 20, 30, 20, 0, 0, 0, 40, 10, 13, 0, 0, 60, 5, 10, 0, 0, 300, 2, 3, 3, 4, 0, 0, 0, 0, 2, 2, 5, 6, 5, 10, 4, 8, 0},
			{6, 0, 3, 1, 1, 0, 2, 20, 30, 20, 10, 15, 5, 0, 11, 14, 16, 21, 0, 7, 12, 6, 31, 0, 2, 3, 3, 4, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 4, 0, 1100},
			{7, 0, 3, 1, 1, 0, 2, 20, 30, 20, 10, 15, 5, 0, 11, 14, 16, 21, 0, 7, 12, 6, 31, 0, 2, 3, 3, 4, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 4, 0, 1200},
			{8, 0, 3, 1, 1, 0, 2, 20, 30, 20, 10, 15, 5, 0, 11, 14, 16, 21, 0, 7, 12, 6, 31, 0, 2, 3, 3, 4, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 4, 0, 1300},
			{9, 0, 3, 1, 1, 0, 2, 20, 30, 20, 10, 15, 5, 0, 11, 14, 16, 21, 0, 7, 12, 6, 31, 0, 2, 3, 3, 4, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 4, 0, 1400},
			{10, 1, 0, 0, 3, 0, 3, 25, 35, 30, 0, 0, 0, 45, 11, 14, 0, 0, 65, 7, 12, 0, 0, 350, 3, 4, 4, 5, 0, 0, 0, 0, 2, 2, 4, 5, 5, 10, 5, 8, 0},
			{11, 0, 4, 0, 2, 1, 3, 25, 35, 30, 15, 20, 10, 0, 12, 15, 17, 22, 0, 9, 14, 7, 32, 0, 3, 4, 4, 5, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 5, 0, 1500},
			{12, 0, 4, 0, 2, 1, 3, 25, 35, 30, 15, 20, 10, 0, 12, 15, 17, 22, 0, 9, 14, 7, 32, 0, 3, 4, 4, 5, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 5, 0, 1600},
			{13, 0, 4, 0, 2, 1, 3, 25, 35, 30, 15, 20, 10, 0, 12, 15, 17, 22, 0, 9, 14, 7, 32, 0, 3, 4, 4, 5, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 5, 0, 1700},
			{14, 0, 4, 0, 2, 1, 3, 25, 35, 30, 15, 20, 10, 0, 12, 15, 17, 22, 0, 9, 14, 7, 32, 0, 3, 4, 4, 5, 2, 3, 3, 4, 0, 0, 4, 5, 0, 0, 5, 0, 1800},
			{15, 1, 0, 0, 3, 0, 4, 30, 35, 30, 0, 0, 0, 50, 12, 15, 0, 0, 70, 9, 14, 0, 0, 400, 3, 4, 4, 5, 0, 0, 0, 0, 3, 3, 4, 5, 5, 9, 5, 9, 0},
			{16, 0, 5, 1, 2, 1, 3, 30, 35, 30, 20, 25, 10, 0, 13, 16, 18, 23, 0, 11, 16, 8, 33, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 4, 5, 0, 0, 5, 0, 1900},
			{17, 0, 5, 1, 2, 1, 3, 30, 35, 30, 20, 25, 10, 0, 13, 16, 18, 23, 0, 11, 16, 8, 33, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 4, 5, 0, 0, 5, 0, 2000},
			{18, 0, 5, 1, 2, 1, 3, 30, 35, 30, 20, 25, 10, 0, 13, 16, 18, 23, 0, 11, 16, 8, 33, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 4, 5, 0, 0, 5, 0, 2100},
			{19, 0, 5, 1, 2, 1, 3, 30, 35, 30, 20, 25, 10, 0, 13, 16, 18, 23, 0, 11, 16, 8, 33, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 4, 5, 0, 0, 5, 0, 2200},
			{20, 1, 0, 0, 4, 0, 4, 35, 40, 40, 0, 0, 0, 55, 13, 16, 0, 0, 75, 11, 16, 0, 0, 450, 3, 4, 4, 5, 0, 0, 0, 0, 3, 3, 3, 4, 4, 9, 5, 9, 0},
			{21, 0, 6, 1, 3, 1, 3, 35, 40, 40, 25, 30, 15, 0, 14, 17, 19, 24, 0, 13, 18, 9, 34, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 3, 4, 0, 0, 6, 0, 2300},
			{22, 0, 6, 1, 3, 1, 3, 35, 40, 40, 25, 30, 15, 0, 14, 17, 19, 24, 0, 13, 18, 9, 34, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 3, 4, 0, 0, 6, 0, 2400},
			{23, 0, 6, 1, 3, 1, 3, 35, 40, 40, 25, 30, 15, 0, 14, 17, 19, 24, 0, 13, 18, 9, 34, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 3, 4, 0, 0, 6, 0, 2500},
			{24, 0, 6, 1, 3, 1, 3, 35, 40, 40, 25, 30, 15, 0, 14, 17, 19, 24, 0, 13, 18, 9, 34, 0, 3, 4, 4, 5, 3, 4, 4, 5, 0, 0, 3, 4, 0, 0, 6, 0, 2600},
			{25, 1, 0, 0, 4, 0, 4, 40, 45, 40, 0, 0, 0, 60, 14, 17, 0, 0, 80, 13, 18, 0, 0, 500, 4, 5, 5, 6, 0, 0, 0, 0, 3, 3, 3, 4, 4, 8, 6, 10, 0},
			{26, 0, 7, 2, 3, 2, 4, 40, 45, 40, 30, 35, 15, 0, 15, 18, 20, 25, 0, 15, 20, 10, 35, 0, 4, 5, 5, 6, 4, 5, 5, 6, 0, 0, 2, 3, 0, 0, 6, 0, 2700},
			{27, 0, 7, 2, 3, 2, 4, 40, 45, 40, 30, 35, 15, 0, 15, 18, 20, 25, 0, 15, 20, 10, 35, 0, 4, 5, 5, 6, 4, 5, 5, 6, 0, 0, 2, 3, 0, 0, 6, 0, 2800},
			{28, 0, 7, 2, 3, 2, 4, 40, 45, 40, 30, 35, 15, 0, 15, 18, 20, 25, 0, 15, 20, 10, 35, 0, 4, 5, 5, 6, 4, 5, 5, 6, 0, 0, 2, 3, 0, 0, 6, 0, 2900},
			{29, 0, 7, 2, 3, 2, 4, 40, 45, 40, 30, 35, 15, 0, 15, 18, 20, 25, 0, 15, 20, 10, 35, 0, 4, 5, 5, 6, 4, 5, 5, 6, 0, 0, 2, 3, 0, 0, 6, 0, 3000},
			{30, 1, 0, 0, 5, 0, 5, 45, 50, 50, 0, 0, 0, 65, 15, 18, 0, 0, 85, 15, 20, 0, 0, 600, 4, 5, 5, 6, 0, 0, 0, 0, 4, 4, 2, 3, 3, 7, 6, 10, 0},
		};
	}
}