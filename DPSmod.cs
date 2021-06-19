using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPSMod
{
	public class DPSmod : ETGModule
	{
		public static ETGModuleMetadata metadata;

		public static readonly string MOD_NAME = "DPS Counter Mod";
		public static readonly string VERSION = "1.0.0";
		private int FontSize = 34;
		public static UnityEngine.UI.Text DPsText;


		public override void Start()
		{
			try
			{
				metadata = this.Metadata;
				

				new Hook(typeof(PlayerController).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public), typeof(DPSmod).GetMethod("OnNewPlayer"));
				GUI.Init();
				ETGModConsole.Commands.AddGroup("dps", args =>
				{
				});

				DPsText = GUI.CreateText(null, new Vector2(15f, 350), "", TextAnchor.MiddleLeft, font_size:FontSize);
				ETGModMainBehaviour.Instance.gameObject.AddComponent<DPSTeller>();
				AdvancedLogging.Log("@(DPSMod/Resources/icon)");
				AdvancedLogging.Log($"{MOD_NAME} v{VERSION} started successfully.", new Color32(235, 232, 52, 255));
			}
			catch (Exception e)
			{
				AdvancedLogging.LogError("mod Broke heres why: " + e);
			}

		}

		public static void OnNewPlayer(Action<PlayerController> orig, PlayerController self)
		{
			orig(self);
			self.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(self.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(delegate (float damageAmount, bool fatal, HealthHaver target)
			{
				var dmg = damageAmount;
				addDamageToDPS(dmg);
			}));
		}


		public static DateTime start, End, LastHit;

		public static float Damage;

		public static bool Started = true;

		public static void addDamageToDPS(float dmg)
		{
			if (Started)
			{
				LastHit = DateTime.Now;
				Damage += dmg;
				End = DateTime.Now;
			}
			else
			{
				Started = true;
				start = DateTime.Now;
				End = DateTime.Now;
				LastHit = DateTime.Now;
				Damage = dmg;
			}
		}

		
		public static float DPS()
		{
			TimeSpan timeSpan = End - start;
			float num = timeSpan.Milliseconds / 1000f;

			num += timeSpan.Seconds;
			num += timeSpan.Minutes / 60f;

			if (num >= 3f)
			{
				start = DateTime.Now;
				start = start.AddSeconds(-1.0);
				Damage = (Damage / num);
				timeSpan = End - start;
				num = timeSpan.Milliseconds / 1000f;
				num += timeSpan.Seconds;
				num += timeSpan.Minutes / 60f;

			}
			if (num < 1f)
				num = 1f;

			return (float)Math.Round(Damage / num, 2);
			
		}


		public override void Exit() { }
		public override void Init() { }
	}
}
