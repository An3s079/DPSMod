using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx;

namespace DPSMod
{
	[BepInPlugin("an3s.etg.dpscounter", "DPS Counter", "1.0.0")]
	[BepInDependency("etgmodding.etg.mtgapi", BepInDependency.DependencyFlags.HardDependency)]
	public class DPSmod : BaseUnityPlugin
	{

		public static readonly string MOD_NAME = "DPS Counter Mod";
		public static readonly string VERSION = "1.0.0";
		public static int FontSize = 34;
		public static UnityEngine.UI.Text DPsText;
		private static string ConfigDirectory = Path.Combine(ETGMod.ResourcesDirectory, "dpsconfig");
		private static string SaveFilePath = Path.Combine(DPSmod.ConfigDirectory, "dps.json");
		private DpsFile dpsObj;
		public static bool ConstantlyUpdated = false;
		public void Start()
		{
			try
			{
				new Hook(typeof(PlayerController).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public), typeof(DPSmod).GetMethod("OnNewPlayer"));
				GUI.Init();
                ETGModConsole.Commands.AddGroup("dps", args =>
                {
                });
				if (!Directory.Exists(DPSmod.ConfigDirectory))
				{
					Directory.CreateDirectory(DPSmod.ConfigDirectory);
				}
				if (!File.Exists(DPSmod.SaveFilePath))
				{
					File.Create(DPSmod.SaveFilePath).Close();
				}
				this.dpsObj = new DpsFile();
				if (!string.IsNullOrEmpty(File.ReadAllText(DPSmod.SaveFilePath)))
				{
					this.dpsObj = JsonUtility.FromJson<DpsFile>(File.ReadAllText(DPSmod.SaveFilePath));
					FontSize = dpsObj.fontSize;
					DPSmod.ConstantlyUpdated = dpsObj.constantlyUpdating;

                }
                else
                {
					dpsObj.constantlyUpdating = false;
					dpsObj.fontSize = 34;
					File.WriteAllText(DPSmod.SaveFilePath, JsonUtility.ToJson(dpsObj));
                }
				ETGModConsole.Commands.AddGroup("dps");
				ETGModConsole.Commands.GetGroup("dps").AddUnit("fontsize", new Action<string[]>(this.setFontsize));
				ETGModConsole.Commands.GetGroup("dps").AddUnit("constantlyupdating", new Action<string[]>(this.setConstantUpdate));
				ETGModConsole.Commands.GetGroup("dps").AddUnit("togglevisibility", new Action<string[]>(this.togglevisibility));
				
				DPsText = GUI.CreateText(null, new Vector2(15f, 265), "", TextAnchor.MiddleLeft, font_size: FontSize);
				gameObject.AddComponent<DPSTeller>();

				ETGModConsole.LogButton("", null, GetTextureFromResource("DPSMod/Resources/icon.png"));
				ETGModConsole.Log($"{MOD_NAME} v{VERSION} started successfully.").Colors[0] = Color.green;
			}
			catch (Exception e)
			{
				Debug.Log("DPSmod Broke heres why: " + e);
			}

		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002C54 File Offset: 0x00000E54
		private void togglevisibility(string[] obj)
		{
			DPSmod.DPsText.gameObject.SetActive(!DPSmod.DPsText.gameObject.activeSelf);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002C7C File Offset: 0x00000E7C
		private void setConstantUpdate(string[] obj)
		{
				DPSmod.ConstantlyUpdated = !ConstantlyUpdated;
				this.dpsObj.constantlyUpdating = DPSmod.ConstantlyUpdated;
				File.WriteAllText(DPSmod.SaveFilePath, JsonUtility.ToJson(dpsObj));
				ETGModConsole.Log("DPS display: constantly updating is set to " + ConstantlyUpdated, false);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002D10 File Offset: 0x00000F10
		private void setFontsize(string[] obj)
		{
			bool flag = obj.Length == 0;
			if (flag)
			{
				ETGModConsole.Log("Must have at least one argument, try `dps fontsize <number>`", false);
			}
			else
			{
				FontSize = Mathf.RoundToInt(float.Parse(obj[0]));
				this.dpsObj.fontSize = FontSize;
				File.WriteAllText(DPSmod.SaveFilePath, JsonUtility.ToJson(dpsObj));
				ETGModConsole.Log("DPS display: Font Size is set to " + FontSize, false);
			}
		}

		public static byte[] ExtractEmbeddedResource(String filePath)
		{
			filePath = filePath.Replace("/", ".");
			filePath = filePath.Replace("\\", ".");
			var baseAssembly = Assembly.GetCallingAssembly();
			using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filePath))
			{
				if (resFilestream == null)
				{
					return null;
				}
				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}

		public static Texture2D GetTextureFromResource(string resourceName)
		{
			string file = resourceName;
			byte[] bytes = ExtractEmbeddedResource(file);
			if (bytes == null)
			{
				ETGModConsole.Log("No bytes found in " + file).Colors[0] = Color.red;
				return null;
			}
			Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			ImageConversion.LoadImage(texture, bytes);
			texture.filterMode = FilterMode.Point;

			string name = file.Substring(0, file.LastIndexOf('.'));
			if (name.LastIndexOf('.') >= 0)
			{
				name = name.Substring(name.LastIndexOf('.') + 1);
			}
			texture.name = name;

			return texture;
		}
		public static void OnNewPlayer(Action<PlayerController> orig, PlayerController self)
		{
			orig(self);
			self.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(self.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(delegate (float damageAmount, bool fatal, HealthHaver target)
			{		
				addDamageToDPS(damageAmount);
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
	}
}
