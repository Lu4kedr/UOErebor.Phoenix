using Phoenix.WorldData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Phoenix.Plugins.Autoheal
{
    [Serializable]
    public class HealedPlayers : IEnumerable<Patient>
    {
        private List<int> avaibleEquips;
        [XmlArray]
        public List<Patient> Patients { get; set; }

        [XmlArray]
        public List<int> AvaibleEquips
        {
            get
            {
                return avaibleEquips;
            }

            set
            {
                avaibleEquips = value;
            }
        }


        public HealedPlayers()
        {
            Patients = new List<Patient>();
            avaibleEquips = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        }
        public void Add(Patient p)
        {
            Patients.Add(p);
            avaibleEquips.Remove(p.Equip);
        }


        public void Add(UOCharacter ch)
        {
            if (Patients.Any(chk => chk.Serial == ch.Serial))
            {
                UO.PrintError("Uz je v seznamu");
                return;
            }
            if (Patients.Count >= 14)
            {
                UO.PrintError("Plny seznam");
                return;
            }

            int selectedEq = avaibleEquips.First();
            avaibleEquips.Remove(selectedEq);
            UO.WaitTargetObject(ch);
            UO.Say(".setequip" + selectedEq);
            Patients.Add(new Patient() { Serial = ch.Serial, Equip = selectedEq });
        }

        public void Remove(int index)
        {
            avaibleEquips.Add(Patients[index].Equip);
            Patients.Remove(Patients[index]);
        }



        /// <summary>
        /// Return patient that needs attention. If no need attention returns null.
        /// </summary>
        /// <param name="MinimalHits"> Under this value of HP decide if needs bandage</param>
        /// <returns></returns>
        public Patient GetPatient(int MinimalHits)
        {
            foreach (var p in Patients.Where(x => x.Player.Distance < 10 && (x.Player.Hits < 0 || x.Player.Hits > 100)).ToList()) 
            {
                    p.Player.RequestStatus(10);
            }
            List<Patient> temp = Patients.Where(x=>x.Player.Hits<MinimalHits && x.Player.Distance<10).ToList();
            temp.Sort((x, y) => x.Player.Hits.CompareTo(y.Player.Hits));
            if (temp.Count > 0)
                return temp[0];
            else return null;
        }
        public Patient this[int index]
        {
            get
            {
                if (index >= 0 && index < Patients.Count)
                {
                    return Patients[index];
                }
                else return null;
            }
        }


 


        public IEnumerator<Patient> GetEnumerator()
        {
            return ((IEnumerable<Patient>)Patients).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Patient>)Patients).GetEnumerator();
        }


    }
}
