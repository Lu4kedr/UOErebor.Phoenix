using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumber
{
	public class Check
	{

		private System.Timers.Timer t;
		public event EventHandler<OnLogAddedArgs> OnLogAdded;

		public string AlarmPath = Core.Directory + @"\afk.wav";

		public static bool NoLog = false;
		public static bool SkillDelay = false;
		public static bool MaxedWeight = false;




		private void Checking()
		{
			// Check AFK
			if (Journal.Contains(true, "afk", "AFK", "kontrola", "GM", "gm"))
			{
				System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
				my_wave_file.Play();
				UO.Wait(200);

			}
			// Check Light
			if (Journal.Contains(true, "Je spatne videt.") || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
			{
				World.Player.Layers[Layer.LeftHand].Use();
				UO.Wait(200);
				if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

			}
			// Skill delay
			if (Journal.Contains(true, "Jeste nemuzes pouzit skill"))
			{

				SkillDelay = true;

			}
			// Check Weight
			if (World.Player.Weight > (World.Player.Strenght * 4 + 15))
			{
				MaxedWeight = true;

			}

			// No Ore
			if (Journal.Contains(true, " no logs", " blow ", " prilis daleko.", " moc daleko.", "too far", "Tam nedosahnes."," immune "," cannot reach")) 

			{

				NoLog = true;

			}



			// Incoming Ore  
			if (Journal.Contains(true, "You put "))//, calls[1], calls[2], calls[3], calls[4]))
			{
				string type = "_";


				if (Journal.Contains(true, " Copper "))
				{
					type = "Copper";

				}
				else
				if (Journal.Contains(true, " Iron "))
				{
					type = "Iron";
				}
				else
				if (Journal.Contains(true, " Kremicity "))
				{
					type = "Kremicity";
				}
				else
				if (Journal.Contains(true, " Verite "))
				{
					type = "Verite";

				}
				else
				if (Journal.Contains(true, " Valorite "))
				{
					type = "Valorite";
				}
				else
				if (Journal.Contains(true, " Obsidian "))
				{
					type = "Obsidian";
				}
				else
				if (Journal.Contains(true, " Adamantium "))
				{
					type = "Adamantium";
				}
				var temp2 = OnLogAdded;
				if (temp2 != null && type != "_")
				{
					foreach (EventHandler<OnLogAddedArgs> ev in temp2.GetInvocationList())
					{
						ev.BeginInvoke(null, new OnLogAddedArgs() { Type = type }, null, null);
					}
				}

			}
			Journal.Clear();
			Journal.ClearAll();

		}

		internal void Stop()
		{
			t.Elapsed -= T_Elapsed;
			t.Stop();
			t.Dispose();
			t = null;

		}

		public void Start()
		{
			t = new System.Timers.Timer(200);
			t.Elapsed += T_Elapsed;
			t.Start();

		}

		private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			t.Elapsed -= T_Elapsed;
			Checking();
			t.Elapsed += T_Elapsed;
		}
	}
}
