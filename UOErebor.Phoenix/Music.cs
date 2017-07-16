using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Music
    {
        readonly string[] musicDoneCalls = { " have no musical instrument ", "uklidneni se povedlo.", " neni co uklidnovat!", "uklidnovani nezabralo.", "tohle nemuzes ", "you play poorly.", "oslabeni uspesne.", "oslabeni se nepovedlo.", " tve moznosti." };
        private DateTime StartMusic;
        public static bool MusicDone { get; private set; }
        private Serial T1, T2;
        private System.Timers.Timer FS;

        public Music()
        {
            MusicDone = true;

        }

        [Command, BlockMultipleExecutions]
        public void peace()
        {
            Peace_Entic(StandardSkill.Peacemaking, Aliases.GetObject("laststatus"));
        }
        [Command, BlockMultipleExecutions]
        public void entic()
        {
            Peace_Entic(StandardSkill.Discordance_Enticement, Aliases.GetObject("laststatus"));
        }


        [Command, BlockMultipleExecutions]
        public void setprovo()
        {
            UO.PrintInformation("Zamer cil 1");
            T1 = UIManager.TargetObject();
            UO.PrintInformation("Zamer cil 2");
            T2 = UIManager.TargetObject();
        }

        [Command, BlockMultipleExecutions]
        public void provo()
        {
            Provo(T1, T2);
        }


        private void Peace_Entic(StandardSkill Skill, Serial Target)
        {
            if (Target == null)
            {
                MusicDone = true;
                return;
            }
            MusicDoneFailSafe();
            UOCharacter tmp = new UOCharacter(Target);
            MusicDone = false;
            if (tmp.Distance > 18)
            {
                MusicDone = true;
                return;
            }
            tmp.WaitTarget();
            UO.UseSkill(Skill);

        }

        private void Provo(Serial Target1, Serial Target2)
        {
            if (Target1 == null || Target2 == null)
            {
                MusicDone = true;
                return;
            }
            MusicDoneFailSafe();
            UOCharacter tmp1 = new UOCharacter(Target1);
            UOCharacter tmp2 = new UOCharacter(Target2);
            MusicDone = false;
            if (tmp1.Distance > 18 || tmp2.Distance > 18)
            {
                MusicDone = true;
                return;
            }
            tmp1.WaitTarget();
            UO.Say(".provo");
            tmp2.WaitTarget();


        }

        private void MusicDoneFailSafe()
        {
            StartMusic = DateTime.Now;
            Core.RegisterServerMessageCallback(0x1C, OnCalls);
            FS = new System.Timers.Timer(200);
            FS.Elapsed += FS_Elapsed;
            FS.Start();
        }

        private void FS_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(MusicDone)
            {
                FS.Elapsed -= FS_Elapsed;
                FS.Stop();
                FS.Dispose();
                FS = null;
            }
            if(DateTime.Now-StartMusic>TimeSpan.FromSeconds(5))
            {
                MusicDone = true;
                FS.Elapsed -= FS_Elapsed;
                FS.Stop();
                FS.Dispose();
                FS = null;
            }
        }

        CallbackResult OnCalls(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);


            foreach (string s in musicDoneCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    MusicDone = true;
                    Core.UnregisterServerMessageCallback(0x1C, OnCalls);
                    return CallbackResult.Normal;

                }
            }
            return CallbackResult.Normal;
        }
    }
}
