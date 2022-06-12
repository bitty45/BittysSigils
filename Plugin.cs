using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using InscryptionAPI.Triggers;
using BittysSigils.Triggers;
using Pixelplacement;
using UnityEngine;
using Object = UnityEngine.Object;
using APIPlugin;

/// 
/// To be added:
///		Swap Aura
///			when another card is played, its stats get swapped
///		IOnTarget Trigger?
///		IOnOtherTarget Trigger?
///		Target creature takes 2 damage, gains 1 power
///		Divine Favor (set all stats of all cards on the board to 1/1)
///			(makes you unable to play any more cards that turn? (requires patch))
///		Table Flip?
///			switch the places of the player's and the opponent's cards on the board
///			opponent's queue becomes your hand
///			first 4 cards of your hand becomes opponent's queue
///		Gemify
///			patch the playable card to have cost modifications?
///		
///	Test out:
///		Champion (should activate when taking damage from anything other than the combat phase)
///		Targeted StatSwap
///	
/// New sigils:
///		
///	New Triggers: 
///		
/// need art for:
///		Targeted StatSwap
///		
/// Credit:
///		//Dragon for gemify act 2 art
/// 
///	45 total sigils

namespace BittysSigils
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Plugin.Log = base.Logger;
			Harmony harmony = new Harmony(PluginGuid);
			Add_Ability_AllStatsSwap();
			Add_Ability_OpposingCopy();
			Add_Ability_ActivatedBuffPlayerPower();
			Add_Ability_ActivatedHealthRoll();
			Add_Ability_ActivatedSigilRoll();

			Add_Ability_CorpseSpawner();
			Add_Ability_Bleach();
			Add_Ability_OnlyTransfer();
			Add_Ability_NoTransfer();
			Add_Ability_NoStones();

			Add_Ability_Champion();
			Add_Ability_CantAttack();
			Add_Ability_Mushrooms();
			Add_Ability_StrafePull();
			Add_Ability_StrafeSticky();

			Add_Ability_CounterAttack();
			Add_Ability_MirrorCounter();
			Add_Ability_StrafeBoard();
			Add_Ability_StrafePlayerBoard();
			Add_Ability_StrafeOppBoard();

			Add_Ability_DeathBell();
			Add_Ability_Clockwise();
			Add_Ability_SwapImmune();
			Add_Ability_Tornado();
			Add_Ability_OpponentCopyOnKill();

			Add_Ability_SoulLinked();
			Add_Ability_VibesharkDraw();
			Add_Ability_VibesharkBarrage();
			Add_Ability_StrafeJump();
			Add_Ability_StrafeSuper();

			Add_Ability_NoCampfire();
			Add_Ability_NoCampfireHealth();
			Add_Ability_NoCampfireAttack();
			Add_Ability_MoxDependant();
			Add_Ability_MoxPhobic();

			Add_Ability_CreateEggs();
			Add_Ability_FirCaller();
			Add_Ability_NoModifications();
			Add_Ability_Outcast();
			Add_Ability_HallowedRepeater();

			Add_Ability_Fleeting();
			Add_Ability_FleetingDraw();
			Add_Ability_SwapStatsTurnEnd();
			Add_Ability_SquirrelDeck();
			Add_Ability_Replenish();

			Add_Ability_TargetedStatSwap();

			//Add_Ability_Gemify();

			Add_Ability_SwapStats();

			Patches.ReplaceSwapper();
			Add_Card_TestCard();
			Add_Card_Corpse();
			harmony.PatchAll(typeof(Patches));
			base.Logger.LogInfo("Bitty45's Sigils loaded!");
		}
		internal const string PluginGuid = "bitty45.inscryption.sigils";

		internal const string PluginName = "Bitty45's Sigils";

		internal const string PluginVersion = "1.4.0";

		internal const string CardPrefix = "bitty";

		internal static ManualLogSource Log;

		public void Add_Ability_AllStatsSwap()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Heart Swap",
				"When a card bearing this sigil is played, all cards on the board swap their power and health.",
				typeof(Sigils.GiveAllStatsSwap),
				"ability_allStatsSwap.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_allStatsSwap_a2.png"))
			;
			abilityInfo.powerLevel = 4;

			// Pass the ability to the API.
			Sigils.GiveAllStatsSwap.ability = abilityInfo.ability;
		}
		public void Add_Ability_OpposingCopy()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Mutual Hire",
				"When a card bearing this sigil is played, a copy of it is created in the opposing space.",
				typeof(Sigils.GiveOpposingCopy),
				"ability_opposingCopy.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_opposingCopy_a2.png"))
			;
			abilityInfo.powerLevel = 2;

			// Pass the ability to the API.
			Sigils.GiveOpposingCopy.ability = abilityInfo.ability;
		}
		public void Add_Ability_ActivatedBuffPlayerPower()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Charged Barrage",
				"Pay 4 life to increase the power of all cards on your side of the board by 1.",
				typeof(Sigils.GiveActivatedBuffPlayerPower),
				"ability_activatedBuffPlayerPower.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			;
			abilityInfo.powerLevel = 5;
			abilityInfo.activated = true;
			abilityInfo.pixelIcon = TextureHelper.ConvertTexture(TextureHelper.GetImageAsTexture("ability_activatedBuffPlayerPower_a2.png"));

			// Pass the ability to the API.
			Sigils.GiveActivatedBuffPlayerPower.ability = abilityInfo.ability;
		}
		public void Add_Ability_ActivatedHealthRoll()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Health Roll",
				"Pay 1 Energy to set the health of a card bearing this sigil randomly between 1 and 3.",
				typeof(Sigils.GiveActivatedHealthRoll),
				"ability_activatedHealthRoll.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			;
			abilityInfo.powerLevel = 1;
			abilityInfo.activated = true;
			abilityInfo.pixelIcon = TextureHelper.ConvertTexture(TextureHelper.GetImageAsTexture("ability_activatedHealthRoll_a2.png"));

			// Pass the ability to the API.
			Sigils.GiveActivatedHealthRoll.ability = abilityInfo.ability;
		}
		public void Add_Ability_ActivatedSigilRoll()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Sigil Roll",
				"Pay 4 Life to add a random sigil to a card bearing this sigil.",
				typeof(Sigils.GiveActivatedSigilRoll),
				"ability_activatedSigilRoll.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			;
			abilityInfo.powerLevel = 5;
			abilityInfo.activated = true;
			abilityInfo.pixelIcon = TextureHelper.ConvertTexture(TextureHelper.GetImageAsTexture("ability_activatedSigilRoll_a2.png"));

			// Pass the ability to the API.
			Sigils.GiveActivatedSigilRoll.ability = abilityInfo.ability;
		}
		public void Add_Ability_OpponentCopyOnKill()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Wildlife Camera",
				"When a card bearing this sigil kills another card, a copy of the killed card is created in your hand.",
				typeof(Sigils.GiveOpponentCopyOnKill),
				"ability_opponentCopyOnKill.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_opponentCopyOnKill_a2.png"))

			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveOpponentCopyOnKill.ability = abilityInfo.ability;
		}
		public void Add_Ability_CorpseSpawner()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Dusty Quill",
				"Whenever a creature dies while a card bearing this sigil is on the board, a corpse is raised in it's place. Corpses are defined as: 0/1.",
				typeof(Sigils.GiveCorpseSpawner),
				"ability_corpseSpawner.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_corpseSpawner_a2.png"))

			;
			abilityInfo.powerLevel = 4;

			// Pass the ability to the API.
			Sigils.GiveCorpseSpawner.ability = abilityInfo.ability;
		}
		public void Add_Ability_Bleach()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Bleached Brush",
				"When a card bearing this sigil is played, the opposing card will lose all its sigils.",
				typeof(Sigils.GiveBleach),
				"ability_bleach.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_bleach_a2.png"))

			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveBleach.ability = abilityInfo.ability;
		}
		public void Add_Ability_OnlyTransfer()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Strong Soul",
				"A card bearing this sigil may not recieve sigils from another card.",
				typeof(Sigils.GiveOnlyTransfer),
				"ability_onlyTransfer.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_onlyTransfer_a2.png"))
			;
			abilityInfo.powerLevel = -1;

			// Pass the ability to the API.
			Sigils.GiveOnlyTransfer.ability = abilityInfo.ability;
		}
		public void Add_Ability_NoTransfer()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Weak Soul",
				"A card bearing this sigil may not have its sigils transferred to another card.",
				typeof(Sigils.GiveNoTransfer),
				"ability_noTransfer.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_noTransfer_a2.png"))
			;
			abilityInfo.powerLevel = -1;

			// Pass the ability to the API.
			Sigils.GiveNoTransfer.ability = abilityInfo.ability;
		}
		public void Add_Ability_NoStones()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Nonexistent Soul",
				"A card bearing this sigil may not have sigils transferred to, or from this card.",
				typeof(Sigils.GiveNoStones),
				"ability_noStones.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_noStones_a2.png"))

			;
			abilityInfo.powerLevel = -2;

			// Pass the ability to the API.
			Sigils.GiveNoStones.ability = abilityInfo.ability;
		}
		public void Add_Ability_NoCampfire()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Pyrophobia",
				"A card bearing this sigil may not be buffed at campsites.",
				typeof(Sigils.GiveNoCampfire),
				"ability_noCampfire.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_noCampfire_a2.png"))

			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			Sigils.GiveNoCampfire.ability = abilityInfo.ability;
		}
		public void Add_Ability_NoCampfireHealth()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Pyrophobia (Health)",
				"A card bearing this sigil may not recieve health buffs from campsites.",
				typeof(Sigils.GiveNoCampfireHealth),
				"ability_noCampfireHealth.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_noCampfireHealth_a2.png"))

			;
			abilityInfo.powerLevel = -1;

			// Pass the ability to the API.
			Sigils.GiveNoCampfireHealth.ability = abilityInfo.ability;
		}
		public void Add_Ability_NoCampfireAttack()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Pyrophobia (Attack)",
				"A card bearing this sigil may not recieve power buffs from campsites.",
				typeof(Sigils.GiveNoCampfireAttack),
				"ability_noCampfireAttack.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_noCampfireAttack_a2.png"))

			;
			abilityInfo.powerLevel = -2;

			// Pass the ability to the API.
			Sigils.GiveNoCampfireAttack.ability = abilityInfo.ability;
		}
		public void Add_Ability_Champion()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Champion",
				"A card bearing this sigil will not take damage from other cards except during the combat phase.",
				typeof(Sigils.GiveChampion),
				"ability_champion.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_champion_a2.png"))

			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveChampion.ability = abilityInfo.ability;
		}
		private void Add_Ability_CantAttack()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Repulsion",
				"A card bearing this sigil may not attack.",
				typeof(Sigils.GiveCantAttack),
				"ability_repulsion.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_repulsion_a2.png"))
			;
			abilityInfo.powerLevel = -5;

			// Pass the ability to the API.
			Sigils.GiveCantAttack.ability = abilityInfo.ability;
		}
		private void Add_Ability_Mushrooms()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Mysterious Mushrooms",
				"At the end of the turn, if there is a card on either side of a card bearing this sigil, they are fused together.",
				typeof(Sigils.GiveMushrooms),
				"ability_mushrooms.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_mushrooms_a2.png"))
			;
			abilityInfo.powerLevel = 3;


			// Pass the ability to the API.
			Sigils.GiveMushrooms.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeSticky()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Sticky",
				"At the end of the owner's turn, a card bearing this sigil will move the opposing card and itself in the direction inscribed in the sigil.",
				typeof(Sigils.GiveStrafeSticky),
				"ability_sticky.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_sticky_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			Sigils.GiveStrafeSticky.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafePull()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Hauler",
				"At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. Adjacent friendly creatures will be pulled in the same direction.",
				typeof(Sigils.GiveStrafePull),
				"ability_pull.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_pull_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			Sigils.GiveStrafePull.ability = abilityInfo.ability;
		}
		private void Add_Ability_CounterAttack()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Counter Attack",
				"Once a card bearing this sigil is struck, the striker is then dealt damage equal to this card's attack.",
				typeof(Sigils.GiveCounterAttack),
				"ability_counterAttack.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_counterAttack_a2.png"))
			;
			abilityInfo.powerLevel = 4;

			// Pass the ability to the API.
			Sigils.GiveCounterAttack.ability = abilityInfo.ability;
		}
		private void Add_Ability_MirrorCounter()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Mirror Counter",
				"Once a card bearing this sigil is struck, the striker is then dealt damage equal to the striker's attack.",
				typeof(Sigils.GiveMirrorCounter),
				"ability_mirrorCounter.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_mirrorCounter_a2.png"))
			;
			abilityInfo.powerLevel = 5;

			// Pass the ability to the API.
			Sigils.GiveMirrorCounter.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeBoard()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Board Shifter",
				"At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. All cards will shift in the same direction, looping to the other edge of the board.",
				typeof(Sigils.GiveStrafeBoard),
				"ability_boardShifter.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_boardShifter_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			Sigils.GiveStrafeBoard.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafePlayerBoard()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Board Shifter (Player)",
				"At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. Friendly cards will shift in the same direction, looping to the other edge of the board.",
				typeof(Sigils.GiveStrafePlayerBoard),
				"ability_boardShifter(Player).png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_boardShifter(Player)_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			Sigils.GiveStrafePlayerBoard.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeOppBoard()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Board Shifter (Opponent)",
				"At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. The opponent's cards will shift in the same direction, looping to the other edge of the board.",
				typeof(Sigils.GiveStrafeOppBoard),
				"ability_boardShifter(Opponent).png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_boardShifter(Opponent)_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			Sigils.GiveStrafeOppBoard.ability = abilityInfo.ability;
		}
		private void Add_Ability_DeathBell()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Deathbell",
				"At the start of the battle phase, a card bearing this sigil perishes.",
				typeof(Sigils.GiveDeathBell),
				"ability_deathbell.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_deathbell_a2.png"))
			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			Sigils.GiveDeathBell.ability = abilityInfo.ability;
		}
		private void Add_Ability_Clockwise()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Clockwise",
				"When a card bearing this sigil is played, move all cards on the board clockwise.",
				typeof(Sigils.GiveClockwise),
				"ability_clockwise.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_clockwise_a2.png"))
			;
			abilityInfo.powerLevel = 5;

			// Pass the ability to the API.
			Sigils.GiveClockwise.ability = abilityInfo.ability;
		}
		private void Add_Ability_SwapImmune()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Stubborn",
				"A card bearing this sigil may not have its stats swapped.",
				typeof(Sigils.GiveStatsSwapImmune),
				"ability_stubborn.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_stubborn_a2.png"))
			;
			abilityInfo.powerLevel = 2;

			// Pass the ability to the API.
			Sigils.GiveStatsSwapImmune.ability = abilityInfo.ability;
		}
		private void Add_Ability_Tornado()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Twister",
				"At the end of each turn a card bearing this sigil is on the board, move all cards on the board clockwise.",
				typeof(Sigils.GiveTornado),
				"ability_tornado.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_tornado_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveTornado.ability = abilityInfo.ability;
		}
		private void Add_Ability_Fleeting()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Fleeting",
				"A card bearing this sigil will be discarded at the end of the turn.",
				typeof(Sigils.GiveFleeting),
				"ability_fleeting.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_fleeting_a2.png"))
			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			Sigils.GiveFleeting.ability = abilityInfo.ability;
		}
		private void Add_Ability_SwapStats()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Swapper",
				"After a card bearing this sigil is dealt damage, swap its Power and Health.",
				typeof(Sigils.GiveSwapStats),
				"ability_swapstats.png"
			).AddMetaCategories(
			AbilityMetaCategory.Part1Rulebook,
			AbilityMetaCategory.Part3BuildACard,
			AbilityMetaCategory.BountyHunter,
			AbilityMetaCategory.GrimoraRulebook,
			AbilityMetaCategory.MagnificusRulebook)
			.SetDefaultPart3Ability()
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_swapstats_a2.png"))

			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			Sigils.GiveSwapStats.ability = abilityInfo.ability;
		}
		private void Add_Ability_SoulLinked()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Soul Link",
				"When a card bearing this sigil perishes, all other allied cards bearing this sigil perish as well.",
				typeof(Sigils.GiveSoulLinked),
				"ability_soullinked.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_soullinked_a2.png"))
			;
			abilityInfo.powerLevel = -2;

			// Pass the ability to the API.
			Sigils.GiveSoulLinked.ability = abilityInfo.ability;
		}
		private void Add_Ability_VibesharkDraw()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Hollow Draw",
				"When a card bearing this sigil is played, discard the oldest card in your hand, draw a card.",
				typeof(Sigils.GiveVibesharkDraw),
				"ability_hollowdraw.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_hollowdraw_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveVibesharkDraw.ability = abilityInfo.ability;
		}
		private void Add_Ability_VibesharkBarrage()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Hollow Barrage",
				"When a card bearing this sigil is played, all cards take 1 damage.",
				typeof(Sigils.GiveVibesharkBarrage),
				"ability_hollowbarrage.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_hollowbarrage_a2.png"))
			;
			abilityInfo.powerLevel = 4;

			// Pass the ability to the API.
			Sigils.GiveVibesharkBarrage.ability = abilityInfo.ability;
		}
		private void Add_Ability_Gemify()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Gemify",
				"A card bearing this sigil gains buffs based on moxes in play on the owner's side. Orange is 1 Power. Green is 2 Health. Blue is lower Cost.",
				typeof(Sigils.GiveGemify),
				"ability_gemify.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_gemify_a2.png"))
			;
			abilityInfo.powerLevel = 4;
			abilityInfo.colorOverride = Color.HSVToRGB(88, 94, 90);

			// Pass the ability to the API.
			Sigils.GiveGemify.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeJump()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Jumper",
				"At the end of the owner's turn, a card bearing this sigil will move itself to the first empty space in the direction inscribed in the sigil.",
				typeof(Sigils.GiveStrafeJump),
				"ability_jumper.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_jumper_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			Sigils.GiveStrafeJump.ability = abilityInfo.ability;
		}
		private void Add_Ability_StrafeSuper()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Super Sprinter",
				"At the end of the owner's turn, a card bearing this sigil will move itself as far as possible in the direction inscribed in the sigil.",
				typeof(Sigils.GiveStrafeSuper),
				"ability_superstrafe.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_superstrafe_a2.png"))
			;
			abilityInfo.powerLevel = 0;

			// Pass the ability to the API.
			Sigils.GiveStrafeSuper.ability = abilityInfo.ability;
		}
		private void Add_Ability_MoxDependant()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Mox Dependant",
				"If a card bearing this sigil's owner controls no Mox cards, a card bearing this sigil perishes.",
				typeof(Sigils.GiveMoxDependant),
				"ability_moxdependant.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_moxdependant_a2.png"))
			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			Sigils.GiveMoxDependant.ability = abilityInfo.ability;
		}
		private void Add_Ability_MoxPhobic()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Mox Phobic",
				"If a card bearing this sigil's owner controls a Mox card, a card bearing this sigil perishes.",
				typeof(Sigils.GiveMoxPhobic),
				"ability_moxphobic.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_moxphobic_a2.png"))
			;
			abilityInfo.powerLevel = -3;

			// Pass the ability to the API.
			Sigils.GiveMoxPhobic.ability = abilityInfo.ability;
		}
		private void Add_Ability_NoModifications()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Foolhardy",
				"A card bearing this sigil may not recieve modifications from any source.",
				typeof(Sigils.GiveNoModifications),
				"ability_nomodifications.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_nomodifications_a2.png"))
			;
			abilityInfo.powerLevel = -5;

			// Pass the ability to the API.
			Sigils.GiveNoModifications.ability = abilityInfo.ability;
		}
		private void Add_Ability_CreateEggs()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Eggist",
				"When a card bearing this sigil is played, an Egg is created on each empty adjacent space.",
				typeof(Sigils.GiveCreateEggs),
				"ability_eggist.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_eggist_a2.png"))
			;
			abilityInfo.powerLevel = 4;

			// Pass the ability to the API.
			Sigils.GiveCreateEggs.ability = abilityInfo.ability;
		}
		private void Add_Ability_FirCaller()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Fir Caller",
				"When a card bearing this sigil is played, a Fir is created in each of the player's spaces.",
				typeof(Sigils.GiveCreateFirsOnPlayer),
				"ability_fircaller.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_fircaller_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveCreateFirsOnPlayer.ability = abilityInfo.ability;
		}
		public void Add_Ability_SwapStatsTurnEnd()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Flipper",
				"At the end of the owner's turn, a card bearing this sigil swaps its stats.",
				typeof(Sigils.GiveSwapStatsTurnEnd),
				"ability_statSwapTurnEnd.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_statSwapTurnEnd_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveSwapStatsTurnEnd.ability = abilityInfo.ability;
		}
		private void Add_Ability_FleetingDraw()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Fleeting Draw",
				"When a card bearing this sigil is played, draw 1 card with Fleeting.",
				typeof(Sigils.GiveFleetingDraw),
				"ability_fleetingdraw.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_fleetingdraw_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveFleetingDraw.ability = abilityInfo.ability;
		}
		private void Add_Ability_Outcast()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Outcast",
				"At the end of the turn, a card bearing this sigil is shuffled into the deck.",
				typeof(Sigils.GiveOutcast),
				"ability_outcast.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_outcast_a2.png"))
			;
			abilityInfo.powerLevel = -2;

			// Pass the ability to the API.
			Sigils.GiveOutcast.ability = abilityInfo.ability;
		}
		private void Add_Ability_HallowedRepeater()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Hollow Repeater",
				"When a card bearing this sigil perishes, all On Play sigils on that card are activated.",
				typeof(Sigils.GiveHallowedRepeater),
				"ability_hollowrepeater.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_hollowrepeater_a2.png"))
			;
			abilityInfo.powerLevel = 5;

			// Pass the ability to the API.
			Sigils.GiveHallowedRepeater.ability = abilityInfo.ability;
		}
		private void Add_Ability_SquirrelDeck()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Explosive Squirrel Reproduction",
				"When a card bearing this sigil is played, fill your side deck with squirrels.",
				typeof(Sigils.GiveSquirrelDeck),
				"ability_squirreldeck.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_squirreldeck_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveSquirrelDeck.ability = abilityInfo.ability;
		}
		private void Add_Ability_Replenish()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Replenish",
				"When a card bearing this sigil is drawn, draw a card.",
				typeof(Sigils.GiveReplenish),
				"ability_replenish.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_replenish_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveReplenish.ability = abilityInfo.ability;
		}
		private void Add_Ability_TargetedStatSwap()
		{
			AbilityInfo abilityInfo = AbilityManager.New(
				PluginGuid,
				"Target Swap",
				"When a card bearing this sigil is played, Target a Card: the card's stats are swapped.",
				typeof(Sigils.GiveTargetedStatSwap),
				"ability_test.png"
			).AddMetaCategories(AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook)
			.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("ability_test_a2.png"))
			;
			abilityInfo.powerLevel = 3;

			// Pass the ability to the API.
			Sigils.GiveTargetedStatSwap.ability = abilityInfo.ability;
		}

		private void Add_Card_TestCard()
		{
			CardInfo newCard = CardManager.New("bitty", "TestCard", "TestCard",
				1, 3,
				"Hmm.")
				.AddAbilities(
				Ability.Tutor,
				Sigils.GiveTargetedStatSwap.ability)
				.SetPortraitAndEmission("portrait_test.png", "portrait_test.png");

			CardManager.Add("bitty", newCard);
		}
		private void Add_Card_Corpse()
		{
			CardInfo newCard = CardManager.New("bitty", "Corpse", "Corpse",
				0, 1,
				"Hmm.")

				.SetPortraitAndEmission("portrait_test.png", "portrait_test.png")
				.AddAppearances(UndeadAppearance);
			newCard.defaultEvolutionName = "Zombie";
			CardManager.Add("bitty", newCard);
		}
		public class UndeadAppearanceBehaviour : CardAppearanceBehaviour
		{
			public override void ApplyAppearance()
			{
				Texture2D UndeadBG = TextureHelper.GetImageAsTexture("card_undead.png");
				UndeadBG.filterMode = FilterMode.Point;
				base.Card.RenderInfo.baseTextureOverride = UndeadBG;
				base.Card.renderInfo.forceEmissivePortrait = true;
				base.Card.StatsLayer.SetEmissionColor(GameColors.Instance.brightGold);
			}

		}
		public readonly static CardAppearanceBehaviour.Appearance UndeadAppearance = CardAppearanceBehaviourManager.Add(PluginGuid, "UndeadAppearance", typeof(UndeadAppearanceBehaviour)).Id;
	}
	public class Sigils
	{
		public abstract class SigilEffectsBase : AbilityBehaviour
		{
			public IEnumerator SwapStats(PlayableCard card)
			{
				int baseAttack = card.Info.Attack;
				int baseHealth = card.Info.Health;
				CardModificationInfo cardModificationInfo = card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "zeroout");
				if (cardModificationInfo == null)
				{
					CardModificationInfo zeroMod = new CardModificationInfo();
					zeroMod.nonCopyable = true;
					zeroMod.singletonId = "zeroout";
					zeroMod.attackAdjustment = -baseAttack;
					zeroMod.healthAdjustment = -baseHealth;
					card.AddTemporaryMod(zeroMod);
					zeroMod = new CardModificationInfo();
					zeroMod.nonCopyable = true;
					zeroMod.singletonId = "statswap";
					zeroMod.attackAdjustment = baseAttack;
					zeroMod.healthAdjustment = baseHealth;
					card.AddTemporaryMod(zeroMod);
				}

				int attack = card.Attack;
				int health = card.Health;
				CardModificationInfo swapMod = card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "statswap");
				if (swapMod != null)
				{
					card.HealDamage(card.Status.damageTaken);
					swapMod.attackAdjustment = health;
					swapMod.healthAdjustment = attack;

					card.OnStatsChanged();
					card.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.25f);
				}
				//swap trigger
				yield return card.TriggerHandler.Trigger((IOnStatSwap x) => x.RespondsToStatSwap(card), (IOnStatSwap x) => x.OnStatSwap(card));
				yield return CustomTriggerFinder.TriggerAll<IOnOtherStatSwap>(false, (IOnOtherStatSwap x) => x.RespondsToOtherStatSwap(card), (IOnOtherStatSwap x) => x.OnOtherStatSwap(card));

				if (card.Health <= 0)
				{
					yield return card.Die(false, null, true);
				}
			}
			public IEnumerator DiscardCard(PlayableCard card)
            {
				yield return card.TriggerHandler.Trigger((IOnDiscard x) => x.RespondsToThisDiscard(card), (IOnDiscard x) => x.OnThisDiscard(card));
				yield return CustomTriggerFinder.TriggerAll<IOnOtherDiscard>(false, (IOnOtherDiscard x) => x.RespondsToOtherDiscard(card), (IOnOtherDiscard x) => x.OnOtherDiscard(card));

				if (Singleton<ViewManager>.Instance.CurrentView != View.Hand)
				{
					yield return new WaitForSeconds(0.2f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Hand, false, false);
					yield return new WaitForSeconds(0.2f);
				}
				card.SetInteractionEnabled(false);
				card.Anim.PlayDeathAnimation(false);
				AudioController.Instance.PlaySound3D("wizard_projectileimpact", MixerGroup.TableObjectsSFX, base.Card.transform.position, 1f, 0f);
				Object.Destroy(card.gameObject, 1f);
				Singleton<PlayerHand>.Instance.RemoveCardFromHand(card);
				yield return new WaitForSeconds(0.4f);
			}
			public IEnumerator MoveCardIntoDeck(PlayableCard card, bool toSideDeck = false)
			{
				bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;

				if (card.OnBoard && Singleton<ViewManager>.Instance.CurrentView != View.Board)
				{
					yield return new WaitForSeconds(0.2f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					yield return new WaitForSeconds(0.2f);
				}
				card.UnassignFromSlot();

				if (Singleton<PlayerHand>.Instance.CardsInHand.Contains(card) && Singleton<ViewManager>.Instance.CurrentView != View.Hand)
				{
					yield return new WaitForSeconds(0.2f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Hand, false, false);
					yield return new WaitForSeconds(0.2f); 
				}
				if (Singleton<PlayerHand>.Instance.CardsInHand.Contains(card))
					Singleton<PlayerHand>.Instance.RemoveCardFromHand(card);

				if (is3D && toSideDeck)
				{
					Singleton<CardDrawPiles3D>.Instance.sidePile.MoveCardToPile(card, true);
					Object.Destroy(card.gameObject, 1f);
				}
				else if (is3D)
				{
					Singleton<CardDrawPiles3D>.Instance.pile.MoveCardToPile(card, true);
					Object.Destroy(card.gameObject, 1f);
				}
				CreateCardInDeck(card.Info, toSideDeck);
            }
			public void CreateCardInDeck(CardInfo info, bool toSideDeck = false)
			{
				bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;
				if (is3D && toSideDeck)
				{
					Singleton<CardDrawPiles3D>.Instance.sidePile.CreateCards(1);
					Singleton<CardDrawPiles3D>.Instance.SideDeck.AddCard(info);
				}
				else
				{
					if (is3D)
					{
						Singleton<CardDrawPiles3D>.Instance.pile.CreateCards(1);
					}
					Singleton<CardDrawPiles>.Instance.Deck.AddCard(info);
				}
			}
			public IEnumerator DrawCard(bool mods = false)
			{
				bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;
				if (Singleton<CardDrawPiles>.Instance.Deck.cards.Count > 0)
				{
					yield return new WaitForSeconds(0.4f);
					if (Singleton<ViewManager>.Instance.CurrentView != View.Default)
					{
						yield return new WaitForSeconds(0.2f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
						yield return new WaitForSeconds(0.2f);
					}
                    if (is3D)
					{
						CardDrawPiles3D.Instance.Pile.Draw();
						CardInfo info = Singleton<CardDrawPiles3D>.Instance.Deck.Draw();
						yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(info, new List<CardModificationInfo>(), Singleton<CardDrawPiles3D>.Instance.drawFromPilesSpawnOffset, 0.15f, null);
                    }
                    else
                    {
						yield return Singleton<CardDrawPiles>.Instance.DrawCardFromDeck(null, null);
					}

					if (mods)
					{
						PlayableCard card = Singleton<PlayerHand>.Instance.CardsInHand[Singleton<PlayerHand>.Instance.CardsInHand.Count - 1];
						yield return card.FlipInHand(new Action(this.AddMod));
						yield return new WaitForSeconds(0.4f);
					}
				}
				yield break;
			}
			private void AddMod()
			{
				PlayableCard card = Singleton<PlayerHand>.Instance.CardsInHand[Singleton<PlayerHand>.Instance.CardsInHand.Count - 1];
				card.Status.hiddenAbilities.Add(this.Ability);
				CardModificationInfo cardModificationInfo = new CardModificationInfo(this.DrawAbility);
				CardModificationInfo cardModificationInfo2 = card.TemporaryMods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
				if (cardModificationInfo2 == null)
				{
					cardModificationInfo2 = card.Info.Mods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
				}
				if (cardModificationInfo2 != null)
				{
					cardModificationInfo.fromTotem = cardModificationInfo2.fromTotem;
					cardModificationInfo.fromCardMerge = cardModificationInfo2.fromCardMerge;
				}
				card.AddTemporaryMod(cardModificationInfo);
				if (DrawMods != null)
                {
					card.AddTemporaryMod(DrawMods);
                }
			}
			protected virtual Ability DrawAbility { get; }
			protected virtual CardModificationInfo DrawMods 
			{
				get
				{
					return null;
				}
			}
		}
		public abstract class TargetSlotBase : SigilEffectsBase
        {
			public IEnumerator TargetSlot(List<CardSlot> allSlots, List<CardSlot> validSlots)
            {
				BoardManager instance = Singleton<BoardManager>.Instance;
				yield return instance.ChooseTarget(
					allSlots, 
					validSlots, 
					delegate (CardSlot slot)
					{
						CustomCoroutine.Instance.StartCoroutine(TargetSelectedCallback(slot));
					}, 
					delegate (CardSlot slot)
					{
						CustomCoroutine.Instance.StartCoroutine(InvalidTargetCallback(slot));
					},
					delegate (CardSlot slot)
					{
						CustomCoroutine.Instance.StartCoroutine(SlotCursorCallback(slot));
					}, () => CancelCondition(), CursorType.Target);
			}
			public virtual IEnumerator TargetSelectedCallback(CardSlot slot)
            {
				yield break;
			}
			public virtual IEnumerator InvalidTargetCallback(CardSlot slot)
			{
				yield break;
			}
			public virtual IEnumerator SlotCursorCallback(CardSlot slot)
			{
				yield break;
			}
			public virtual bool CancelCondition()
			{
				return false;
			}
		}
		public class GiveAllStatsSwap : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveAllStatsSwap.ability;
				}
			}

			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					if (slot.Card != null)
					{
						yield return SwapStats(slot.Card);
					}
				}
				yield return base.LearnAbility(0.2f);
				yield break;
			}

			public static Ability ability;
		}
		public class GiveOpposingCopy : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveOpposingCopy.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				yield return new WaitForSeconds(0.1f);
				CardSlot opposingSlot = base.Card.Slot.opposingSlot;
				if (opposingSlot.Card == null)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(base.Card.Info, opposingSlot, 0.15f, true);
					yield return new WaitForSeconds(0.2f);
					yield return base.LearnAbility(0f);
				}
				else
				{
					base.Card.Anim.StrongNegationEffect();
				}
				yield break;
			}

			public static Ability ability;
		}
		public class GiveActivatedBuffPlayerPower : ActivatedAbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveActivatedBuffPlayerPower.ability;
				}
			}
			public override bool CanActivate()
			{
				return Singleton<LifeManager>.Instance.Balance >= 0;
			}
			public override IEnumerator Activate()
			{
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.2f);
				yield return Singleton<LifeManager>.Instance.ShowDamageSequence(4, 4, true);
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				yield return new WaitForSeconds(0.1f);
				AudioController.Instance.PlaySound3D("beastcage_end", MixerGroup.TableObjectsSFX, base.Card.transform.position, 1f, 0f);
				
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.PlayerSlotsCopy)
				{
					if (slot.Card != null)
					{
						slot.Card.Anim.StrongNegationEffect();
						slot.Card.AddTemporaryMod(new CardModificationInfo(1, 0));
						yield return new WaitForSeconds(0.20f);
					}
				}
				yield return base.LearnAbility(0f);
				yield break;
			}

			public static Ability ability;
		}
		public class GiveActivatedHealthRoll : ActivatedAbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveActivatedHealthRoll.ability;
				}
			}
			public override int EnergyCost
			{
				get
				{
					return 1;
				}
			}
			public override IEnumerator Activate()
			{
				CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_randomHealth");
				if (cardModificationInfo != null || base.Card.Health <= 1)
				{
					yield return HealthRoll(0, cardModificationInfo);
				}
				else
				{
					yield return HealthRoll(base.Card.Health - 1, cardModificationInfo);
				}
				yield break;
			}
			public IEnumerator HealthRoll(int healthReduction, CardModificationInfo cardModificationInfo)
			{
				yield return base.PreSuccessfulTriggerSequence();
				if (healthReduction > 0)
				{
					base.Card.AddTemporaryMod(new CardModificationInfo(0, -healthReduction));
				}

				if (cardModificationInfo == null)
				{
					cardModificationInfo = new CardModificationInfo();
					cardModificationInfo.singletonId = "bitty_randomHealth";
					base.Card.AddTemporaryMod(cardModificationInfo);
				}
				int healthAdjustment = cardModificationInfo.healthAdjustment;
				while (cardModificationInfo.healthAdjustment == healthAdjustment)
				{
					cardModificationInfo.healthAdjustment = UnityEngine.Random.Range(0, 3);
				}
				AudioController.Instance.PlaySound3D("bones_scales_scuttle", MixerGroup.TableObjectsSFX, base.Card.transform.position, 1f, 0f);
				base.Card.Anim.StrongNegationEffect();
				base.Card.OnStatsChanged();
				yield return new WaitForSeconds(0.25f);
				yield return base.LearnAbility(0f);
			}
			public static Ability ability;
		}
		public class GiveActivatedSigilRoll : ActivatedAbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveActivatedSigilRoll.ability;
				}
			}
			public override bool CanActivate()
			{
				return Singleton<LifeManager>.Instance.Balance >= 0;
			}
			public override IEnumerator Activate()
			{
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.2f);
				yield return Singleton<LifeManager>.Instance.ShowDamageSequence(4, 4, true);
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				yield return new WaitForSeconds(0.1f);

				int currentRandomSeed = GetRandomSeed();
				CardModificationInfo mod = new CardModificationInfo(
					AbilitiesUtil.GetRandomLearnedAbility(currentRandomSeed++, false, 0, 5, AbilityMetaCategory.Part1Modular)
					);
				mod.fromCardMerge = true;
				base.Card.AddTemporaryMod(mod);
				if (!CardDisplayer3D.EmissionEnabledForCard(base.Card.renderInfo, base.Card))
				{
					base.Card.RenderInfo.forceEmissivePortrait = true;
					base.Card.StatsLayer.SetEmissionColor(GameColors.Instance.brightLimeGreen);
				}

				AudioController.Instance.PlaySound3D("bones_scales_scuttle", MixerGroup.TableObjectsSFX, base.Card.transform.position, 1f, 0f);
				base.Card.Anim.StrongNegationEffect();
				base.Card.OnStatsChanged();
				yield return new WaitForSeconds(0.25f);
				yield return base.LearnAbility(0f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveOpponentCopyOnKill : DrawCreatedCard
		{
			public override Ability Ability
			{
				get
				{
					return GiveOpponentCopyOnKill.ability;
				}
			}
			public override CardInfo CardToDraw
			{
				get
				{
					return opponentCard;
				}
			}
			public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				return killer == base.Card && !card.HasTrait(Trait.Giant);
			}
			public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				yield return base.PreSuccessfulTriggerSequence();
				AudioController.Instance.PlaySound3D("camera_flash_shorter", MixerGroup.TableObjectsSFX, base.Card.transform.position, 1f, 0f);
				opponentCard = card.Info;
				yield return base.CreateDrawnCard();
				yield return base.LearnAbility(0f);
				yield break;
			}

			public CardInfo opponentCard;
			public static Ability ability;
		}
		public class GiveCorpseSpawner : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveCorpseSpawner.ability;
				}
			}
			public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				return  base.Card.OnBoard && card.Info.name != "bitty_Corpse" && !card.HasTrait(Trait.Terrain) && fromCombat && card != base.Card && !card.InOpponentQueue;
			}
			public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
			{
				yield return base.PreSuccessfulTriggerSequence();
				var corpse = CardLoader.GetCardByName("bitty_Corpse");

				//below corpse is a clone
				corpse = AdjustedCost(corpse, card.Info);
				corpse.portraitTex = card.Info.portraitTex;

				CardModificationInfo mod = new CardModificationInfo();
				mod.abilities.AddRange(card.Info.Abilities);
				corpse.mods.Add(mod);

				mod = new CardModificationInfo();
				mod.abilities.AddRange(card.Info.ModAbilities);
				mod.fromCardMerge = true;
				corpse.mods.Add(mod);

				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(corpse, deathSlot, 0.15f, true);
				yield return base.LearnAbility(0f);
				yield break;
			}
			private CardInfo AdjustedCost(CardInfo info, CardInfo deadCard)
			{
				CardInfo cloneInfo = CardLoader.Clone(info);
				cloneInfo.mods.Add(new CardModificationInfo()
				{
					bloodCostAdjustment = deadCard.BloodCost,
					bonesCostAdjustment = deadCard.BonesCost,
					energyCostAdjustment = deadCard.EnergyCost,
					addGemCost = deadCard.gemsCost
				});
				return cloneInfo;
			}

			public static Ability ability;
		}
		public class GiveBleach : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveBleach.ability;
				}
			}

            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);

				PlayableCard card = base.Card.OpposingCard();
				if (card != null && !CardHasNoAbilities(card))
				{
					this.SpawnSplatter(Card);
					if (card.FaceDown)
					{
						card.SetFaceDown(false, true);
					}
					card.Anim.PlayTransformAnimation();
					CustomCoroutine.WaitThenExecute(0.15f, delegate
					{
						this.RemoveCardAbilities(card);
					}, false);
					AudioController.Instance.PlaySound2D("magnificus_brush_splatter_bleach", MixerGroup.None, 0.5f, 0f, null, null, null, null, false);
				}
				yield return new WaitForSeconds(0.7f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return base.LearnAbility(0f);
				yield break;
            }
			private void RemoveCardAbilities(PlayableCard card)
			{
				CardModificationInfo cardModificationInfo = new CardModificationInfo();
				cardModificationInfo.negateAbilities = new List<Ability>();
				foreach (CardModificationInfo cardModificationInfo2 in card.TemporaryMods)
				{
					cardModificationInfo.negateAbilities.AddRange(cardModificationInfo2.abilities);
				}
				cardModificationInfo.negateAbilities.AddRange(card.Info.Abilities);
				card.AddTemporaryMod(cardModificationInfo);
			}
			private bool CardHasNoAbilities(PlayableCard card)
			{
				return !card.TemporaryMods.Exists((CardModificationInfo t) => t.abilities.Count > 0) && card.Info.Abilities.Count <= 0;
			}
			private void SpawnSplatter(PlayableCard card)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Items/BleachSplatter"));
				gameObject.transform.position = card.transform.position + new Vector3(0f, 0.1f, -0.25f);
				Object.Destroy(gameObject, 5f);
			}

			public static Ability ability;
		}
		public class GiveOnlyTransfer : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveOnlyTransfer.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveNoTransfer : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoTransfer.ability;
				}
			}
			
			public static Ability ability;
		}
		public class GiveNoStones : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoStones.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveNoCampfire : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoCampfire.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveNoCampfireAttack : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoCampfireAttack.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveNoCampfireHealth : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoCampfireHealth.ability;
				}
			}

			public static Ability ability;
		}
		public class GiveChampion : AbilityBehaviour, IOnBellRung
		{
			public override Ability Ability
			{
				get
				{
					return GiveChampion.ability;
				}
			}

            public bool RespondsToBellRung(bool playerCombatPhase)
            {
				return playerCombatPhase == Card.OpponentCard;
            }
            public IEnumerator OnBellRung(bool playerCombatPhase)
            {
				CardModificationInfo mods = new CardModificationInfo();
				mods.singletonId = "bitty_champion";
				Card.AddTemporaryMod(mods);
				Plugin.Log.LogInfo("Adding Champion Tag for " + Card.name + " in slot " + Card.slot);
				yield break;
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
                return true;
            }
			public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				CardModificationInfo mods = Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_champion");
				Card.RemoveTemporaryMod(mods);
				Plugin.Log.LogInfo("Removing Champion Tag for " + Card.name + " in slot " + Card.slot);
				yield break;
			}

			public static Ability ability;
		}
		public class GiveCantAttack : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveCantAttack.ability;
				}
			}
			public static Ability ability;
		}
		public class GiveMushrooms : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveMushrooms.ability;
				}
			}
			public override int Priority
			{
				get
				{
					return 0;
				}
			}
			public override bool RespondsToTurnEnd(bool playerTurnEnd)
			{
				return true;
			}
			public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				Plugin.Log.LogInfo("Mushrooms Activation");
				yield return base.PreSuccessfulTriggerSequence();
				CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.slot, true);
				CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.slot, false);
				bool toLeftValid = toLeft != null && toLeft.Card != null;
				bool toRightValid = toRight != null && toRight.Card != null;
				if (toLeftValid)
				{
					yield return new WaitForSeconds(0.5f);

					Singleton<ViewManager>.Instance.SwitchToView(View.Board);
					yield return new WaitForSeconds(0.75f);
					yield return this.MoveFromSlotToCenterSlot(toLeft);
				}
				if (toRightValid)
				{
					yield return new WaitForSeconds(0.5f);

					Singleton<ViewManager>.Instance.SwitchToView(View.Board);
					yield return new WaitForSeconds(0.75f);
					yield return this.MoveFromSlotToCenterSlot(toRight);

				}
				yield return new WaitForSeconds(0.5f);
				yield return base.LearnAbility(0f);
				yield break;
			}
			private CardModificationInfo CreateMergeMod(PlayableCard existingCard, PlayableCard cardToMerge)
			{
				CardModificationInfo mods = new CardModificationInfo();
				int num2 = cardToMerge.Info.Abilities.Count;
				for (int i = 0; i < num2; i++)
				{
					mods.abilities.Add(cardToMerge.Info.Abilities[i]);
				}
				return GetDuplicateMod(cardToMerge.Info.Attack, cardToMerge.Info.Health, mods);
			}
			public static CardModificationInfo GetDuplicateMod(int attackBuff, int healthBuff, CardModificationInfo mods)
			{
				return new CardModificationInfo
				{
					fromDuplicateMerge = true,
					attackAdjustment = attackBuff,
					healthAdjustment = healthBuff,
					abilities = mods.abilities,
					DecalIds =
				{
					AlternatingBloodDecal.GetBloodDecalId(),
					"decal_fungus",
					"decal_stitches"
				}
				};
			}
			private IEnumerator MoveFromSlotToCenterSlot(CardSlot originSlot)
			{
				if (originSlot.Card != null)
				{
					CardSlot centerSlot = Singleton<BoardManager>.Instance.GetSlots(originSlot.IsPlayerSlot)[base.Card.Slot.Index];
					if (centerSlot.Card != null)
					{
						Sprite portraitSprite = TextureHelper.GetImageAsSprite("portrait_carnage.png", TextureHelper.SpriteType.CardPortrait);
						if (is3D) centerSlot.Card.RenderInfo.portraitOverride = portraitSprite;

						Tween.Position(centerSlot.Card.transform, centerSlot.transform.position + Vector3.up * 0.2f, 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
						Tween.Position(originSlot.Card.transform, centerSlot.transform.position + Vector3.up * 0.05f, 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
						yield return new WaitForSeconds(0.1f);
						centerSlot.Card.AddTemporaryMod(this.CreateMergeMod(centerSlot.Card, originSlot.Card));
						if (originSlot.Card.HasTrait(Trait.Ant) && !centerSlot.Card.HasTrait(Trait.Ant))
						{
							centerSlot.Card.Info.AddTraits(Trait.Ant);
						}
						Object.Destroy(originSlot.Card.gameObject);
						originSlot.Card = null;
						if (is3D) ApplyAppearance(centerSlot);
						centerSlot.Card.OnStatsChanged();
						if (is3D) Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.3f);
						AudioController.Instance.PlaySound3D("mycologist_carnage", MixerGroup.TableObjectsSFX, centerSlot.transform.position, 1f, 0f, null, null, null, null, false);
						centerSlot.Card.Anim.StrongNegationEffect();
						yield return new WaitForSeconds(0.25f);
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(centerSlot.Card, centerSlot, 0.1f, null, true);
						yield return new WaitForSeconds(0.25f);
					}
					else
					{
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(originSlot.Card, centerSlot, 0.1f, null, true);
					}
				}
				yield break;
			}
			public void ApplyAppearance(CardSlot slot)
			{
				if (slot.Card is PlayableCard)
				{
					slot.Card.Info.TempDecals.Clear();
					slot.Card.Info.TempDecals.Add(ResourceBank.Get<Texture>("Art/Cards/Decals/" + AlternatingBloodDecal.GetBloodDecalId()));
					slot.Card.Info.TempDecals.Add(ResourceBank.Get<Texture>("Art/Cards/Decals/" + "decal_fungus"));
					slot.Card.Info.TempDecals.Add(ResourceBank.Get<Texture>("Art/Cards/Decals/" + "decal_stitches"));
				}
			}
			public static bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;

			public static Ability ability;
		}
		public class GiveStrafeSticky : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeSticky.ability;
				}
			}
			public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool oppExists = base.Card.HasOpposingCard();
				bool oppLeftValid = toLeft != null && toLeft.opposingSlot.Card == null;
				bool oppRightValid = toRight != null && toRight.opposingSlot.Card == null;
				bool leftValid = toLeft != null && toLeft.Card == null;
				bool rightValid = toRight != null && toRight.Card == null;

				if (movingLeft && (!leftValid || (oppExists && !oppLeftValid)))
				{
					movingLeft = false;
				}
				if (!movingLeft && (!rightValid || (oppExists && !oppRightValid)))
				{
					movingLeft = true;
				}
				Plugin.Log.LogInfo(movingLeft);

				CardSlot destination = this.movingLeft ? toLeft : toRight;
				bool destinationValid = this.movingLeft ? leftValid : rightValid;
				if (oppExists)
				{
					destinationValid = this.movingLeft ? leftValid && oppLeftValid : rightValid && oppRightValid;
				}

				Plugin.Log.LogInfo(destination);
				Plugin.Log.LogInfo(destinationValid);
				if (oppExists)
				{
					yield return OppMoveToSlot(destination, destinationValid);
				}
				yield return this.MoveToSlot(destination, destinationValid);

				if (destination != null && destinationValid)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return base.LearnAbility(0f);
				}
				yield break;
			}
			protected IEnumerator OppMoveToSlot(CardSlot destination, bool destinationValid)
			{
				PlayableCard oppCard = base.Card.OpposingCard();
				if (destination != null && destinationValid)
				{
					destination = destination.opposingSlot;
					CardSlot oldSlot = oppCard.Slot;
					yield return Singleton<BoardManager>.Instance.AssignCardToSlot(oppCard, destination, 0.01f, null, true);
					yield return this.PostSuccessfulMoveSequence(oldSlot);
					yield return new WaitForSeconds(0f);
				}
				else
				{
					oppCard.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.15f);
				}
				yield break;
			}

			public static Ability ability;
		}
		public class GiveStrafePull : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafePull.ability;
				}
			}

			protected IEnumerator MovePulled(CardSlot destination, CardSlot pulledSlot)
			{
				if (pulledSlot != null)
				{
					PlayableCard pulled = pulledSlot.Card;
					if (pulled != null)
					{
						CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(pulled.Slot, true);
						CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(pulled.Slot, false);
						bool flag = toLeft != null && toLeft.Card == null;
						bool flag2 = toRight != null && toRight.Card == null;
						if (this.movingLeft && !flag)
						{
							this.movingLeft = false;
						}
						if (!this.movingLeft && !flag2)
						{
							this.movingLeft = true;
						}
						bool destinationValid = destination.Card == null;


						pulled.RenderInfo.flippedPortrait = (this.movingLeft && pulled.Info.flipPortraitForStrafe);
						pulled.RenderCard();
						if (destination != null && destinationValid)
						{
							CardSlot oldSlot = pulled.Slot;
							yield return Singleton<BoardManager>.Instance.AssignCardToSlot(pulled, destination, 0.1f, null, true);
							yield return new WaitForSeconds(0.25f);
							oldSlot = null;
						}
						else
						{
							pulled.Anim.StrongNegationEffect();
							yield return new WaitForSeconds(0.15f);
						}
					}
				}
				yield break;
			}

			public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
			{
				CardSlot pulledSlot = this.movingLeft ? Singleton<BoardManager>.Instance.GetAdjacent(oldSlot, false) : Singleton<BoardManager>.Instance.GetAdjacent(oldSlot, true);
				yield return this.MovePulled(oldSlot, pulledSlot);
				yield break;
			}

			public static Ability ability;
		}
		public class GiveCounterAttack : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveCounterAttack.ability;
				}
			}
            public override bool RespondsToTakeDamage(PlayableCard source)
            {
                return source != null && source.Health > 0;
            }
            public override IEnumerator OnTakeDamage(PlayableCard source)
			{
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.55f);
				yield return source.TakeDamage(base.Card.Attack, base.Card);
				yield return base.LearnAbility(0.4f);
				yield break;
            }
            public static Ability ability;
		}
		public class GiveMirrorCounter : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveMirrorCounter.ability;
				}
			}
			public override bool RespondsToTakeDamage(PlayableCard source)
			{
				return source != null && source.Health > 0;
			}
			public override IEnumerator OnTakeDamage(PlayableCard source)
			{
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.55f);
				yield return source.TakeDamage(source.Attack, base.Card);
				yield return base.LearnAbility(0.4f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveStrafeBoard : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeBoard.ability;
				}
			}
            public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool slotsContainsBaseCard = GetCardSlots().Contains(base.Card.slot); 
				List<CardSlot> baseSideSlots = base.Card.OpponentCard ? Singleton<BoardManager>.Instance.OpponentSlotsCopy : Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				bool leftValid = toLeft != null && (toLeft.Card == null || slotsContainsBaseCard);
				bool rightValid = toRight != null && (toRight.Card == null || slotsContainsBaseCard);
				if(BaseCardWarps() && toLeft == null && baseSideSlots.Last().Card == null)
                {
					leftValid = true;
                }
				if (BaseCardWarps() && toRight == null && baseSideSlots.First().Card == null)
				{
					rightValid = true;
				}
				
				if (this.movingLeft && !leftValid)
				{
					this.movingLeft = false;
				}
				if (!this.movingLeft && !rightValid)
				{
					this.movingLeft = true;
				}

				CardSlot destination = this.movingLeft ? toLeft : toRight;
				if (BaseCardWarps() && destination == null && movingLeft)
				{
					destination = baseSideSlots.Last();
				}
				if (BaseCardWarps() && destination == null && !movingLeft)
				{
					destination = baseSideSlots.First();
				}

				bool destinationValid = this.movingLeft ? leftValid : rightValid;

				yield return ShiftAllCards(destinationValid, slotsContainsBaseCard);
				yield return this.MoveToSlot(destination, destinationValid);
				if (destination != null && destinationValid)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return base.LearnAbility(0f);
				}
				yield break;
			}
			public IEnumerator ShiftAllCards(bool destinationValid, bool slotsContainsBaseCard)
            {
				if (destinationValid)
				{
					List<CardSlot> affectedSlots = GetCardSlots();
                    if (slotsContainsBaseCard)
					{
						affectedSlots.Remove(base.Card.slot);
					}

					if (affectedSlots.Exists((CardSlot x) => x.Card != null))
					{
						Dictionary<PlayableCard, CardSlot> dictionary = new Dictionary<PlayableCard, CardSlot>();
						List<CardSlot> slots = Singleton<BoardManager>.Instance.GetSlots(true);
						List<CardSlot> slots2 = Singleton<BoardManager>.Instance.GetSlots(false);
						int slotAdjustment = movingLeft ? -1 : 1;
                        if (slots.Contains(affectedSlots[0]))
						{
							for (int i = 0; i < slots.Count; i++)
							{
								if (slots[i].Card != null)
								{
									CardSlot value;
									if (i == 0 && movingLeft)
									{
										value = slots[slots.Count - 1];
									}
									else if(i == slots.Count - 1 && !movingLeft)
                                    {
										value = slots[0];
									}
									else
									{
										value = slots[i + slotAdjustment];
									}
									dictionary.Add(slots[i].Card, value);
								}
							}
						}
						if (slots2.Contains(affectedSlots[affectedSlots.Count - 1]))
						{
							for (int i = 0; i < slots2.Count; i++)
							{
								if (slots2[i].Card != null)
								{
									CardSlot value;
									if (i == 0 && movingLeft)
									{
										value = slots2[slots2.Count - 1];
									}
									else if (i == slots2.Count - 1 && !movingLeft)
									{
										value = slots2[0];
									}
									else
									{
										value = slots2[i + slotAdjustment];
									}
									dictionary.Add(slots2[i].Card, value);
								}
							}
						}
						foreach (CardSlot cardSlot in affectedSlots)
						{
							if (cardSlot.Card != null)
							{
								cardSlot.Card.Slot = null;
								cardSlot.Card = null;
							}
						}
						foreach (KeyValuePair<PlayableCard, CardSlot> assignment in dictionary)
						{
							PlayableCard card = assignment.Key;
							assignment.Key.SetIsOpponentCard(!assignment.Value.IsPlayerSlot);
							yield return Singleton<BoardManager>.Instance.AssignCardToSlot(card, assignment.Value, 0.1f, null, true);
							if (card.FaceDown)
							{
								bool flag = assignment.Value.Index == 0 && !assignment.Value.IsPlayerSlot;
								bool flag2 = assignment.Value.Index == Singleton<BoardManager>.Instance.GetSlots(false).Count - 1 && assignment.Value.IsPlayerSlot;
								if (flag || flag2)
								{
									card.SetFaceDown(false, false);
									card.UpdateFaceUpOnBoardEffects();
								}
							}
						}
					}
				}
				yield break;
			}
			public virtual List<CardSlot> GetCardSlots()
            {
				return Singleton<BoardManager>.Instance.AllSlotsCopy;
			}
			public virtual bool BaseCardWarps()
			{
				return false;
			}

			public static Ability ability;
		}
		public class GiveStrafePlayerBoard : GiveStrafeBoard
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafePlayerBoard.ability;
				}
			}
			public override List<CardSlot> GetCardSlots()
			{
				return Singleton<BoardManager>.Instance.PlayerSlotsCopy;
			}

			public static new Ability ability;
		}
		public class GiveStrafeOppBoard : GiveStrafeBoard
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeOppBoard.ability;
				}
			}
			public override List<CardSlot> GetCardSlots()
			{
				return Singleton<BoardManager>.Instance.OpponentSlotsCopy;
			}

			public static new Ability ability;
		}
		public class GiveDeathBell : AbilityBehaviour, IOnBellRung
		{
			public override Ability Ability
			{
				get
				{
					return GiveDeathBell.ability;
				}
			}
            public bool RespondsToBellRung(bool playerCombatPhase)
            {
				return base.Card.OnBoard && playerCombatPhase != base.Card.OpponentCard;
            }
            public IEnumerator OnBellRung(bool playerCombatPhase)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0f);
				yield break;
			}

            public static Ability ability;
		}
		public class GiveClockwise : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveClockwise.ability;
				}
			}
            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
			{
				bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;

				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.1f);
				if (is3D) Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				AudioController.Instance.PlaySound2D("consumable_pocketwatch_use", MixerGroup.TableObjectsSFX, 0.8f, 0f, null, null, null, null, false);
				yield return new WaitForSeconds(0.75f);
				yield return Singleton<BoardManager>.Instance.MoveAllCardsClockwise();
				yield return new WaitForSeconds(1f);
				if (is3D) Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				if (is3D) Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return base.LearnAbility(0f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveStatsSwapImmune : AbilityBehaviour, IOnStatSwap
		{
			public override Ability Ability
			{
				get
				{
					return GiveStatsSwapImmune.ability;
				}
			}

            public bool RespondsToStatSwap(PlayableCard card)
            {
                return base.Card.OnBoard;
            }

            public IEnumerator OnStatSwap(PlayableCard card)
            {
				CardModificationInfo cardModificationInfo = card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "statswap");
				if (cardModificationInfo != null)
				{
					int attack = card.Attack;
					int health = card.Health;
					cardModificationInfo.attackAdjustment = health;
					cardModificationInfo.healthAdjustment = attack;

					card.OnStatsChanged();
					card.Anim.StrongNegationEffect();
				}
				yield break;
            }

            public static Ability ability;
		}
		public class GiveTornado : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveTornado.ability;
				}
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
                return playerTurnEnd;
            }
            public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				bool is3D = Singleton<BoardManager>.Instance is BoardManager3D;

				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.1f);
				if(is3D) Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				AudioController.Instance.PlaySound2D("wardrobe_door_open", MixerGroup.TableObjectsSFX, 0.8f, 0f, null, null, null, null, false);
				yield return new WaitForSeconds(0.75f);
				yield return Singleton<BoardManager>.Instance.MoveAllCardsClockwise();
				yield return new WaitForSeconds(1f);
				if (is3D) Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				if (is3D) Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return base.LearnAbility(0f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveFleeting : SigilEffectsBase, IOnTurnEndInHand
		{
			public override Ability Ability
			{
				get
				{
					return GiveFleeting.ability;
				}
			}
			public override int Priority => 99;
            public bool RespondsToTurnEndInHand(bool playerTurn)
            {
                return playerTurn;
            }
            public IEnumerator OnTurnEndInHand(bool playerTurn)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0f);
				yield return DiscardCard(base.Card);
				yield break;
			}
            public static Ability ability;
		}
		public class GiveSwapStats : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveSwapStats.ability;
				}
			}
			public override bool RespondsToTakeDamage(PlayableCard source)
			{
				return base.Card.Health > 0;
			}
			public override IEnumerator OnTakeDamage(PlayableCard source)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.5f);
				this.swapped = !this.swapped;
				if (this.swapped && base.Card.Info.alternatePortrait != null && Singleton<BoardManager>.Instance is BoardManager3D)
				{
					base.Card.SwitchToAlternatePortrait();
				}
				else
				{
					base.Card.SwitchToDefaultPortrait();
				}
				yield return SwapStats(base.Card);
				yield return base.LearnAbility(0.25f);
				yield break;
			}

			private bool swapped;

			public static Ability ability;
		}
		public class GiveSoulLinked : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveSoulLinked.ability;
				}
			}
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return (killer != null && !killer.HasAbility(GiveSoulLinked.ability)) || killer == null;
            }
            public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
            {
				yield return base.PreSuccessfulTriggerSequence();
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					if (slot.Card != null && slot.Card.OpponentCard == base.Card.OpponentCard && slot.Card.HasAbility(GiveSoulLinked.ability))
					{
						yield return new WaitForSeconds(0.5f);
						yield return slot.Card.Die(false, base.Card);
					}
				}
				yield return base.LearnAbility(0.25f);
				yield break;
			}

			public static Ability ability;
		}
		public class GiveVibesharkDraw : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveVibesharkDraw.ability;
				}
			}
            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return DiscardDraw();
				yield return base.LearnAbility(0.25f);
				yield break;
			}/*
            public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
            {
                return true;
			}
			public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return DiscardDraw();
				yield return base.LearnAbility(0.25f);
				yield break;
			}*/
			public IEnumerator DiscardDraw()
            {
				if(Singleton<PlayerHand>.Instance.CardsInHand.Count > 0)
				{
					yield return DiscardCard(Singleton<PlayerHand>.Instance.CardsInHand[0]);
				}
				yield return DrawCard();
				yield break;
            }

			public static Ability ability;
		}
		public class GiveVibesharkBarrage : AbilityBehaviour
		{
			public override Ability Ability
			{
				get
				{
					return GiveVibesharkBarrage.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return Barrage();
				yield return base.LearnAbility(0.25f);
				yield break;
			}/*
			public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
			{
				return true;
			}
			public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return Barrage();
				yield return base.LearnAbility(0.25f);
				yield break;
			}*/
			public IEnumerator Barrage()
			{
				yield return new WaitForSeconds(0.4f);
				if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
				{
					yield return new WaitForSeconds(0.2f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					yield return new WaitForSeconds(0.2f);
				}
				foreach (CardSlot slot in Singleton<BoardManager>.Instance.AllSlotsCopy)
				{
					if (slot.Card != null)
					{
						yield return slot.Card.TakeDamage(1, base.Card);
						yield return new WaitForSeconds(0.2f);
					}
				}
				yield break;
			}

			public static Ability ability;
		}
		public class GiveMoxDependant : AbilityBehaviour, IOnGemLoss, IOnBellRung
		{
			public override Ability Ability
			{
				get
				{
					return GiveMoxDependant.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return base.Card != null && !base.Card.Dead && !this.HasGems(true);
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public bool RespondsToBellRung(bool playerCombatPhase)
			{
				return base.Card != null && base.Card.OnBoard && playerCombatPhase != base.Card.OpponentCard && !base.Card.Dead && !this.HasGems(false);
			}

			public IEnumerator OnBellRung(bool playerCombatPhase)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
			{
				return base.Card != null && base.Card.OpponentCard != playerUpkeep && !base.Card.Dead && !this.HasGems(false);
			}
			public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield break;
			}
			public bool RespondsToGemLoss()
			{
				return base.Card != null && !base.Card.Dead && base.Card.OnBoard && !this.HasGems(false);
			}
			public IEnumerator OnGemLoss()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}

			public bool HasGems(bool onResolve)
			{
				if (base.Card.OpponentCard)
				{
					if (onResolve)
					{
						if (Singleton<TurnManager>.Instance.Opponent.Queue.Exists((PlayableCard x) => x != null && x.Info.HasTrait(Trait.Gem) && x.Slot.Card == null))
						{
							return true;
						}
					}
					return Singleton<BoardManager>.Instance.OpponentSlotsCopy.Exists((CardSlot x) => x.Card != null && x.Card.Info.HasTrait(Trait.Gem));
				}
				return Singleton<ResourcesManager>.Instance.HasGem(GemType.Orange)
					|| Singleton<ResourcesManager>.Instance.HasGem(GemType.Green)
					|| Singleton<ResourcesManager>.Instance.HasGem(GemType.Blue);
			}

			public static Ability ability;
		}
		public class GiveMoxPhobic : AbilityBehaviour, IOnGemGain, IOnBellRung
		{
			public override Ability Ability
			{
				get
				{
					return GiveMoxPhobic.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return base.Card != null && !base.Card.Dead && this.HasGems(true);
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public bool RespondsToBellRung(bool playerCombatPhase)
			{
				return base.Card != null && base.Card.OnBoard && playerCombatPhase != base.Card.OpponentCard && !base.Card.Dead && this.HasGems(false);
			}

			public IEnumerator OnBellRung(bool playerCombatPhase)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public override bool RespondsToUpkeep(bool playerUpkeep)
			{
				return base.Card != null && base.Card.OpponentCard != playerUpkeep && !base.Card.Dead && this.HasGems(false);
			}
			public override IEnumerator OnUpkeep(bool playerUpkeep)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield break;
			}
			public bool RespondsToGemGain()
			{
				return base.Card != null && !base.Card.Dead && base.Card.OnBoard;
			}
			public IEnumerator OnGemGain()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}

			public bool HasGems(bool onResolve)
			{
				if (base.Card.OpponentCard)
				{
					if (onResolve)
					{
						if (Singleton<TurnManager>.Instance.Opponent.Queue.Exists((PlayableCard x) => x != null && x.Info.HasTrait(Trait.Gem) && x.Slot.Card == null))
						{
							return true;
						}
					}
					return Singleton<BoardManager>.Instance.OpponentSlotsCopy.Exists((CardSlot x) => x.Card != null && x.Card.Info.HasTrait(Trait.Gem));
				}
				return Singleton<ResourcesManager>.Instance.HasGem(GemType.Orange)
					|| Singleton<ResourcesManager>.Instance.HasGem(GemType.Green)
					|| Singleton<ResourcesManager>.Instance.HasGem(GemType.Blue);
			}

			public static Ability ability;
		}
		public class GiveNoModifications : AbilityBehaviour, IOnBellRung
		{
			public override Ability Ability
			{
				get
				{
					return GiveNoModifications.ability;
				}
			}
			private void Start()
			{
				CardModificationInfo cardModificationInfo = new CardModificationInfo();
				cardModificationInfo.nonCopyable = true;
				cardModificationInfo.singletonId = "bitty_noAbilities";
				foreach(AbilityInfo abilityInfo in AbilityManager.AllAbilityInfos)
                {
					if(abilityInfo.ability != GiveNoModifications.ability)
					{
						cardModificationInfo.negateAbilities.Add(abilityInfo.ability);
					}
                }
				base.Card.AddTemporaryMod(cardModificationInfo);
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
                return base.Card.TemporaryMods.Count > 0;
            }
            public override IEnumerator OnTurnEnd(bool playerTurnEnd)
            {
				yield return ClearMods();
				yield break;
			}
			public bool RespondsToBellRung(bool playerCombatPhase)
			{
				return base.Card.TemporaryMods.Count > 0;
			}
			public IEnumerator OnBellRung(bool playerCombatPhase)
			{
				yield return ClearMods();
				yield break;
			}

			public IEnumerator ClearMods()
			{
				base.Card.TemporaryMods.RemoveAll((CardModificationInfo x) => x.singletonId != "bitty_noAbilities");
				base.Card.Anim.StrongNegationEffect();
				base.Card.OnStatsChanged();
				if(base.Card.Health <= 0)
                {
					yield return base.Card.Die(false);
                }
				yield break;
			}

            public static Ability ability;
		}
		public class GiveStrafeJump : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeJump.ability;
				}
			}
			public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool leftHasSpace = this.SlotHasSpace(base.Card.Slot, true);
				bool rightHasSpace = this.SlotHasSpace(base.Card.Slot, false);
				if (this.movingLeft && !leftHasSpace)
				{
					this.movingLeft = false;
				}
				if (!this.movingLeft && !rightHasSpace)
				{
					this.movingLeft = true;
				}
				CardSlot destination = this.movingLeft ? toLeft : toRight;
				bool destinationValid = this.movingLeft ? leftHasSpace : rightHasSpace;
				if (destination != null && destination.Card != null)
				{
					this.didMove = false;
					yield return this.RecursiveMove(destination, this.movingLeft);
				}
				else
                {
					yield return MoveToSlot(destination, destinationValid);
					this.didMove = true;
				}

				if (this.didMove)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return base.LearnAbility(0f);
				}
				else
				{
					base.Card.Anim.StrongNegationEffect();
				}
				yield break;
			}
			public virtual IEnumerator RecursiveMove(CardSlot slot, bool toLeft)
			{
				CardSlot adjacent = Singleton<BoardManager>.Instance.GetAdjacent(slot, toLeft);
				if (adjacent != null && adjacent.Card == null)
				{
					yield return MoveToSlot(adjacent, true);
					this.didMove = true;
				}
				else if (adjacent != null)
				{
					yield return this.RecursiveMove(adjacent, toLeft);
				}
				yield break;
			}

			private bool SlotHasSpace(CardSlot slot, bool toLeft)
			{
				CardSlot adjacent = Singleton<BoardManager>.Instance.GetAdjacent(slot, toLeft);
				return (adjacent != null) && (adjacent.Card == null || this.SlotHasSpace(adjacent, toLeft));
			}

			public bool didMove;

			public static Ability ability;
		}
		public class GiveStrafeSuper : Strafe
		{
			public override Ability Ability
			{
				get
				{
					return GiveStrafeSuper.ability;
				}
			}
			public override IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
			{
				bool leftHasSpace = (toLeft != null) && (toLeft.Card == null);
				bool rightHasSpace = (toRight != null) && (toRight.Card == null);
				if (this.movingLeft && !leftHasSpace)
				{
					this.movingLeft = false;
				}
				if (!this.movingLeft && !rightHasSpace)
				{
					this.movingLeft = true;
				}
				CardSlot destination = this.movingLeft ? EndSlot(base.Card.slot, true) : EndSlot(base.Card.slot, false);
				Plugin.Log.LogInfo("Destination: " + destination);
				bool destinationValid = this.movingLeft ? leftHasSpace && destination != base.Card.slot : rightHasSpace && destination != base.Card.slot;

				yield return base.MoveToSlot(destination, destinationValid);

				if (destination != null && destination != base.Card.slot && destinationValid)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return base.LearnAbility(0f);
				}
				yield break;
			}
			private CardSlot EndSlot(CardSlot slot, bool toLeft)
			{
				CardSlot adjacent = Singleton<BoardManager>.Instance.GetAdjacent(slot, toLeft);
				if ((adjacent != null) && (adjacent.Card == null))
				{
					return EndSlot(adjacent, toLeft);
                }
                else
                {
					return slot;
                }
			}

			public static Ability ability;
		}
		public class GiveCreateEggs : CreateCardsAdjacent
		{
			public override Ability Ability
			{
				get
				{
					return Sigils.GiveCreateEggs.ability;
				}
			}
			public override string SpawnedCardId
			{
				get
				{
					bool ravenEgg = SeededRandom.Value(base.GetRandomSeed()) < 0.1f && ProgressionData.LearnedAbility(this.Ability);
					string cardId = ravenEgg ? "RavenEgg" : "BrokenEgg";
					return cardId;
				}
			}
			public override string CannotSpawnDialogue
			{
				get
				{
					return "Blocked on both sides. No Eggs for you.";
				}
			}

			public static Ability ability;
		}
		public abstract class CreateCardsOnSlots : AbilityBehaviour
		{
			protected abstract string SpawnedCardId { get; }
			protected abstract string CannotSpawnDialogue { get; }
			protected abstract List<CardSlot> CardSlots { get; }
			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				ranOnce = false;
				foreach(CardSlot cardSlot in CardSlots)
                {
					if(cardSlot.Card == null)
					{
						yield return new WaitForSeconds(0.1f);
						yield return this.SpawnCardOnSlot(cardSlot);
						ranOnce = true;
					}
                }
				yield return base.PreSuccessfulTriggerSequence();
				if (ranOnce)
				{
					yield return base.LearnAbility(0f);
				}
				else if (!base.HasLearned)
				{
					yield return Singleton<TextDisplayer>.Instance.ShowUntilInput(this.CannotSpawnDialogue, -0.65f, 0.4f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null, true);
				}
				yield break;
			}
			private IEnumerator SpawnCardOnSlot(CardSlot slot)
			{
				CardInfo cardByName = CardLoader.GetCardByName(this.SpawnedCardId);
				this.ModifySpawnedCard(cardByName);
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(cardByName, slot, 0.15f, true);
				yield break;
			}
			private void ModifySpawnedCard(CardInfo card)
			{
				List<Ability> abilities = base.Card.Info.Abilities;
				foreach (CardModificationInfo cardModificationInfo in base.Card.TemporaryMods)
				{
					abilities.AddRange(cardModificationInfo.abilities);
				}
				abilities.RemoveAll((Ability x) => x == this.Ability);
				if (abilities.Count > 4)
				{
					abilities.RemoveRange(3, abilities.Count - 4);
				}
				CardModificationInfo cardModificationInfo2 = new CardModificationInfo();
				cardModificationInfo2.fromCardMerge = true;
				cardModificationInfo2.abilities = abilities;
				card.Mods.Add(cardModificationInfo2);
			}

			private bool ranOnce;
			private const int MAX_ABILITIES_FOR_SPAWNED_CARD = 4;
		}
        public class GiveCreateFirsOnPlayer : CreateCardsOnSlots
        {
			public override Ability Ability
			{
				get
				{
					return Sigils.GiveCreateFirsOnPlayer.ability;
				}
			}

			protected override string SpawnedCardId
			{
				get
				{
					if(RunState.Run.regionTier >= 0 && RunState.Run.regionTier < RunState.Run.regionOrder.Length && RunState.Run.regionOrder[RunState.Run.regionTier] == 2)
                    {
						return "Tree_SnowCovered";
					}
					return "Tree";
				}
			}

			protected override string CannotSpawnDialogue
			{
				get
				{
					return "No empty spaces. No firs for you.";
				}
			}

			protected override List<CardSlot> CardSlots
			{
				get
				{
					return base.Card.IsPlayerCard() ? Singleton<BoardManager>.Instance.PlayerSlotsCopy : Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				}
			}
			public static Ability ability;
		}
		public class GiveSwapStatsTurnEnd : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveSwapStatsTurnEnd.ability;
				}
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
                return playerTurnEnd != base.Card.OpponentCard;
            }
            public override IEnumerator OnTurnEnd(bool playerTurnEnd)
            {
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.5f);
				this.swapped = !this.swapped;
				if (this.swapped && base.Card.Info.alternatePortrait != null && Singleton<BoardManager>.Instance is BoardManager3D)
				{
					base.Card.SwitchToAlternatePortrait();
				}
				else
				{
					base.Card.SwitchToDefaultPortrait();
				}
				yield return SwapStats(base.Card);
				yield return base.LearnAbility(0.25f);
				yield break;
			}

			private bool swapped;

			public static Ability ability;
		}
		public abstract class GemLocked : SigilEffectsBase, IOnGemChange
        {
			protected virtual GemType GemType { get; }
			public bool RespondsToGemChange()
			{
				return true;
			}
			public IEnumerator OnGemChange()
            {
				active = Singleton<ResourcesManager>.Instance.HasGem(GemType) ? true : false;
				yield break;
			}

			public bool active;
		}
		public class GiveFleetingDraw : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveFleetingDraw.ability;
				}
			}
            protected override Ability DrawAbility
			{
				get
				{
					return GiveFleeting.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return DrawCard(true);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveOutcast : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveOutcast.ability;
				}
			}
            public override bool RespondsToTurnEnd(bool playerTurnEnd)
            {
                return playerTurnEnd != Card.OpponentCard;
            }
            public override IEnumerator OnTurnEnd(bool playerTurnEnd)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return MoveCardIntoDeck(base.Card);
				yield return base.LearnAbility(0.25f);
				yield break;
			}

            public static Ability ability;
		}
		public class GiveHallowedRepeater : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveHallowedRepeater.ability;
				}
			}
            public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
            {
				return true;
            }
            public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
			{
				if (Card.TriggerHandler.RespondsToTrigger(Trigger.ResolveOnBoard, Array.Empty<object>()))
				{
					yield return Card.TriggerHandler.OnTrigger(Trigger.ResolveOnBoard, Array.Empty<object>());
				}
				yield break;
            }
            public static Ability ability;
		}
		public class GiveSquirrelDeck : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveSquirrelDeck.ability;
				}
			}
			public override bool RespondsToResolveOnBoard()
			{
				return true;
			}
			public override IEnumerator OnResolveOnBoard()
			{
				yield return base.PreSuccessfulTriggerSequence();
				for(int i = 0; i < 200; i++)
				{
					CreateCardInDeck(CardLoader.GetCardByName("Squirrel"), true);
				}
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveReplenish : SigilEffectsBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveReplenish.ability;
				}
			}
            public override bool RespondsToDrawn()
            {
				return true;
			}
            public override IEnumerator OnDrawn()
            {
				yield return base.PreSuccessfulTriggerSequence();
				yield return DrawCard(false);
				yield return base.LearnAbility(0.25f);
				yield break;
			}
			public static Ability ability;
		}
		public class GiveTargetedStatSwap : TargetSlotBase
		{
			public override Ability Ability
			{
				get
				{
					return GiveTargetedStatSwap.ability;
				}
			}
            public override bool RespondsToResolveOnBoard()
            {
				return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
				List<CardSlot> slots = Singleton<BoardManager>.Instance.AllSlotsCopy;
				slots.RemoveAll((CardSlot x) => x.Card == null);
				if(slots.Count > 0)
                {
					if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
					{
						yield return new WaitForSeconds(0.2f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
						yield return new WaitForSeconds(0.2f);
					}
					yield return TargetSlot(slots, slots);
                }
				yield break;
            }

            public override IEnumerator TargetSelectedCallback(CardSlot slot)
            {
				yield return SwapStats(slot.Card);
				yield break;
            }

            public static Ability ability;
		}

		public class GiveGemify : AbilityBehaviour, IOnGemChange
		{
			public override Ability Ability
			{
				get
				{
					return GiveGemify.ability;
				}
			}
			private void Start()
			{
				this.modHealth.healthAdjustment = 2;
				this.modHealth.singletonId = "bitty_gemHealth";
				this.modHealth.nonCopyable = true;
				this.modAttack.attackAdjustment = 1;
				this.modAttack.singletonId = "bitty_gemAttack";
				this.modAttack.nonCopyable = true;
				this.modCost.energyCostAdjustment = -1;
				this.modCost.bonesCostAdjustment = -1;
				this.modCost.bloodCostAdjustment = -1;
				this.modCost.nullifyGemsCost = true;
				this.modCost.singletonId = "bitty_gemCost";
				this.modCost.nonCopyable = true;
				this.modCostRemove.energyCostAdjustment = -1;
				this.modCostRemove.bonesCostAdjustment = -1;
				this.modCostRemove.bloodCostAdjustment = -1;
				this.modCostRemove.nullifyGemsCost = true;
				this.modCostRemove.singletonId = "bitty_gemCost";
				this.modCostRemove.nonCopyable = true;
			}
			public override bool RespondsToDrawn()
			{
				return true;
			}
			public override IEnumerator OnDrawn()
			{
				UpdateGemify();
				yield break;
			}
            public override bool RespondsToResolveOnBoard()
            {
				return true;
            }
            public override IEnumerator OnResolveOnBoard()
			{
				UpdateGemify();
				yield break;
            }
            public bool RespondsToGemChange()
			{
				return true;
			}
			public IEnumerator OnGemChange()
			{
				UpdateGemify();
				if (base.Card.OnBoard && base.Card.Health <= 0)
				{
					yield return base.Card.Die(false, null, true);
				}
				yield break;
			}


			public void UpdateGemify()
			{
				Plugin.Log.LogInfo("UpdateStart");
				CardModificationInfo gemAttack = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_gemAttack");
				CardModificationInfo gemHealth = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_gemHealth");
				CardModificationInfo gemCost = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_gemCost");

				Plugin.Log.LogInfo("GemAttack");
				GemAddRemove(HasOrange(), gemAttack, modAttack);
				Plugin.Log.LogInfo("GemHealth");
				GemAddRemove(HasGreen(), gemHealth, modHealth);
				Plugin.Log.LogInfo("GemCost"); 

				if (HasBlue() && gemCost == null)
				{
					base.Card.TransformIntoCard(AdjustCost(true, gemCost));
				}
				else if (!HasBlue() && gemCost != null)
				{
					base.Card.TransformIntoCard(AdjustCost(false, gemCost));
				}

				Plugin.Log.LogInfo("Render");
				base.Card.RenderCard(); 
				base.Card.Anim.StrongNegationEffect();
			}
			private CardInfo AdjustCost(bool adding, CardModificationInfo gemCost)
            {
				CardInfo info = CardLoader.Clone(Card.Info);
				info.Mods = Card.Info.Mods;
                if (adding)
				{
					info.Mods.Add(modCost);
				}
                else
                {
					gemCost = modCostRemove;
					info.Mods.Remove(gemCost);
				}
				return info;
            }
			private void GemAddRemove(bool hasGem, CardModificationInfo cardMod, CardModificationInfo modToAdd)
			{
				if (hasGem && cardMod == null)
				{
					base.Card.AddTemporaryMod(modToAdd);
				}
				else if (!hasGem && cardMod != null)
				{
					base.Card.RemoveTemporaryMod(cardMod);
				}
			}
			public virtual bool HasOrange()
            {
				return Singleton<ResourcesManager>.Instance.HasGem(GemType.Orange);
			}
			public virtual bool HasGreen()
			{
				return Singleton<ResourcesManager>.Instance.HasGem(GemType.Green);
			}
			public virtual bool HasBlue()
			{
				return Singleton<ResourcesManager>.Instance.HasGem(GemType.Blue);
			}

			public CardModificationInfo modHealth = new CardModificationInfo();
			public CardModificationInfo modAttack = new CardModificationInfo();
			public CardModificationInfo modCost = new CardModificationInfo();
			public CardModificationInfo modCostRemove = new CardModificationInfo();

			public static Ability ability;
		}
	}
	public class Patches
    {
		[HarmonyPatch(typeof(CardMergeSequencer), "GetValidCardsForHost")]
		[HarmonyPostfix]
		public static void HostCardsPatch(ref List<CardInfo> __result)
        {
			__result.RemoveAll((CardInfo x) => x.HasAbility(Sigils.GiveOnlyTransfer.ability) || x.HasAbility(Sigils.GiveNoStones.ability) || x.HasAbility(Sigils.GiveNoModifications.ability));
        }

		[HarmonyPatch(typeof(CardMergeSequencer), "GetValidCardsForSacrifice")]
		[HarmonyPostfix]
		public static void SacCardsPatch(ref List<CardInfo> __result)
		{
			__result.RemoveAll((CardInfo x) => x.HasAbility(Sigils.GiveNoTransfer.ability) || x.HasAbility(Sigils.GiveNoStones.ability) || x.HasAbility(Sigils.GiveNoModifications.ability));
		}
		[HarmonyPatch(typeof(CardStatBoostSequencer), "GetValidCards")]
		[HarmonyPostfix]
		public static void StatBuffCardsPatch(ref List<CardInfo> __result, bool forAttackMod)
		{
			__result.RemoveAll((CardInfo x) => x.HasAbility(Sigils.GiveNoCampfire.ability) || x.HasAbility(Sigils.GiveNoModifications.ability));
            if (forAttackMod)
			{
				__result.RemoveAll((CardInfo x) => x.HasAbility(Sigils.GiveNoCampfireAttack.ability));
            }
            else
            {
				__result.RemoveAll((CardInfo x) => x.HasAbility(Sigils.GiveNoCampfireHealth.ability));
			}
		}

		[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
		[HarmonyPrefix]
		public static bool CantAttackPatch(ref CombatPhaseManager __instance, CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			if (attackingSlot.Card != null && attackingSlot.Card.HasAbility(Sigils.GiveCantAttack.ability))
			{
				attackingSlot.Card.Anim.StrongNegationEffect();
				return false;
			}
			return true;
		}
		[HarmonyPatch(typeof(ResourcesManager), "AddGem")]
		[HarmonyPostfix]
		public static IEnumerator AddGemPatch(IEnumerator values)
		{
			yield return values;
			yield return CustomTriggerFinder.TriggerAll<IOnGemGain>(false, (IOnGemGain x) => x.RespondsToGemGain(), (IOnGemGain x) => x.OnGemGain());
			yield return CustomTriggerFinder.TriggerAll<IOnGemChange>(false, (IOnGemChange x) => x.RespondsToGemChange(), (IOnGemChange x) => x.OnGemChange());
		}
		[HarmonyPatch(typeof(ResourcesManager), "LoseGem")]
		[HarmonyPostfix]
		public static IEnumerator LoseGemPatch(IEnumerator values)
		{
			yield return values;
			yield return CustomTriggerFinder.TriggerAll<IOnGemLoss>(false, (IOnGemLoss x) => x.RespondsToGemLoss(), (IOnGemLoss x) => x.OnGemLoss());
			yield return CustomTriggerFinder.TriggerAll<IOnGemChange>(false, (IOnGemChange x) => x.RespondsToGemChange(), (IOnGemChange x) => x.OnGemChange());
		}
		[HarmonyPatch(typeof(PlayableCard), "TakeDamage")]
		[HarmonyPrefix]
		private static void DamagePatch(ref PlayableCard __instance, ref int damage, ref PlayableCard attacker)
		{
			if (__instance.HasAbility(Sigils.GiveChampion.ability))
			{
				CardModificationInfo mods = __instance.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "bitty_champion");
				bool activate = false;
				if (attacker != null)
				{
					Plugin.Log.LogInfo("Checking Champion Tag for " + __instance.name + " in slot " + __instance.slot);
					activate = mods == null;
				}
				else
				{
					activate = true;
				}
				if (activate)
				{
					damage = 0;
				}
			}
		}
		public static void ReplaceSwapper()
		{
			Ability vanilla;
			Ability port;

			vanilla = Ability.SwapStats; 
			port = Sigils.GiveSwapStats.ability;
			var card = ScriptableObjectLoader<CardInfo>.AllData;
			var ability = AbilityManager.BaseGameAbilities;

			for (int index = 0; index < card.Count; index++)
			{
				CardInfo info = card[index];
				if (info.HasAbility(vanilla))
				{
					Plugin.Log.LogMessage("Switching Abilities on Card: " + info.name);
					info.DefaultAbilities.Remove(vanilla);
					info.DefaultAbilities.Add(port);
				}
			}

			for (int index = 0; index < ability.Count; index++)
			{
				AbilityInfo info = ability[index].Info;
				if (info.ability == vanilla)
				{
					info.metaCategories.Clear();
					info.powerLevel = 8;
					info.opponentUsable = false;
					Plugin.Log.LogMessage("Removing vanilla version of Swapper from the rulebook, metacategories...");
				}
			}
		}
	}
}
namespace BittysSigils.Triggers
{
	public interface IOnStatSwap
	{
		bool RespondsToStatSwap(PlayableCard card);

		IEnumerator OnStatSwap(PlayableCard card);
	}
	public interface IOnOtherStatSwap
	{
		bool RespondsToOtherStatSwap(PlayableCard card);

		IEnumerator OnOtherStatSwap(PlayableCard card);
	}
	public interface IOnGemGain
	{
		bool RespondsToGemGain();

		IEnumerator OnGemGain();
	}
	public interface IOnGemLoss
	{
		bool RespondsToGemLoss();

		IEnumerator OnGemLoss();
	}
	public interface IOnGemChange
	{
		bool RespondsToGemChange();

		IEnumerator OnGemChange();
	}
	public interface IOnDiscard
	{
		bool RespondsToThisDiscard(PlayableCard card);

		IEnumerator OnThisDiscard(PlayableCard card);
	}
	public interface IOnOtherDiscard
	{
		bool RespondsToOtherDiscard(PlayableCard card);

		IEnumerator OnOtherDiscard(PlayableCard card);
	}
}
