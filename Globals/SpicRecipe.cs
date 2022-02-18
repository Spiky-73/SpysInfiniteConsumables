﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SPIC.Globals {

	public class SpicRecipe : GlobalRecipe {

		public static List<int> CraftingStations;

		public override void SetStaticDefaults() {
			CraftingStations = new();
			foreach(Recipe r in Main.recipe) {
				foreach(int t in r.requiredTile) {
					if (!CraftingStations.Contains(t)) CraftingStations.Add(t);
				}
			}
		}

		public override void ConsumeItem(Recipe recipe, int type, ref int amount) {
			
		}
		
	}

}