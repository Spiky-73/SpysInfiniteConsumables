﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SPIC.Infinities;

namespace SPIC.Systems;

// TODO remove recipes crafting a fully infinite item
public class InfiniteRecipe : ModSystem {

    public static readonly HashSet<int> CraftingStations = new();

    public static long HighestCost(int type) => _hightestCost.ContainsKey(type) ? _hightestCost[type] : 0;


    public override void Load() {
        On_Recipe.FindRecipes += HookRecipe_FindRecipes;
    }


    public override void PostAddRecipes() {
        CraftingStations.Clear();
        foreach (Recipe recipe in Main.recipe) {
            foreach (int t in recipe.requiredTile) {
                if (!CraftingStations.Contains(t)) CraftingStations.Add(t);
            }

            recipe.AddConsumeItemCallback(OnItemConsume);

            foreach (Item mat in recipe.requiredItem) {
                if (!_hightestCost.ContainsKey(mat.type) || _hightestCost[mat.type] < mat.stack) _hightestCost[mat.type] = mat.stack;
            }
        }
    }


    public static void OnItemConsume(Recipe recipe, int type, ref int amount) {
        if(CrossMod.MagicStorageIntegration.Enabled && CrossMod.MagicStorageIntegration.Version.CompareTo(new(0,5,7,9)) <= 0 && CrossMod.MagicStorageIntegration.InMagicStorage) return;
        if (Main.LocalPlayer.HasInfinite(type, amount, Material.Instance)) {
            amount = 0;
            return;
        }
        int group = recipe.acceptedGroups.FindIndex(g => RecipeGroup.recipeGroups[g].IconicItemId == type);
        if(group == -1) return;
        long total = 0;
        foreach (int groupItemType in RecipeGroup.recipeGroups[group].ValidItems) total += Main.LocalPlayer.GetInfinity(groupItemType, Material.Instance);
        if (total >= amount) amount = 0;
    }


    private static void HookRecipe_FindRecipes(On_Recipe.orig_FindRecipes orig, bool canDelayCheck) {
        if (canDelayCheck) {
            orig(canDelayCheck);
            return;
        }
        InfinityManager.ClearInfinities();
        orig(canDelayCheck);
    }
    
    
    private static readonly Dictionary<int, int> _hightestCost = new();
}
