using System;
using System.Collections.Generic;
using System.IO;

namespace vfBattleTechMod_Core.Utils.MetaDataHelpers
{
    public class MechModel
    {
        private bool _selected;

        public string BaseModel { get; set; }

        public long BattleValue { get; set; }

        public string Chassis { get; set; }

        public long Cost { get; set; }

        public long DbId { get; set; }

        public string Designer { get; set; }

        public string Era { get; set; }

        public bool Extinct { get; set; }

        public string HeroName { get; set; } = string.Empty;

        public string Name { get; set; }

        public string PossibleClanName { get; set; } = string.Empty;

        public string PrimitiveBaseModel
        {
            get
            {
                var iterativeIndex = BaseModel.IndexOf(" II", StringComparison.OrdinalIgnoreCase);
                return iterativeIndex == -1 ? BaseModel : BaseModel.Substring(0, iterativeIndex);
            }
        }

        public string Rating { get; set; }

        public bool ResidentInCollection { get; set; } = false;

        public string RulesLevel { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected)
                {
                    return;
                }

                _selected = value;
            }
        }

        public string TechnologyBase { get; set; }

        public long Tonnage { get; set; }

        public string Variant { get; set; }

        public int Year { get; set; }

        public static List<MechModel> ProcessAvailabilityFile(string availabilityFilePath)
        {
            var mechList = new List<MechModel>();
            using (var reader = new StreamReader(availabilityFilePath))
            {
                // First line headers...
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var mech = FromCsv(line);
                    mechList.Add(mech);
                }
            }

            return mechList;
        }

        public static MechModel FromCsv(string csvData, char separator = ',')
        {
            var parts = csvData.Split(separator);
            if (parts.Length != 12)
            {
                throw new ArgumentException(
                    @"Insufficient columns in CSV data to construct new Mech.",
                    nameof(csvData));
            }

            var mech = new MechModel
            {
                DbId = long.Parse(parts[0]),
                Name = parts[1], // .Trim(' ', '"'),
                Era = parts[2].Trim(' ', '"'),
                Year = int.Parse(parts[3]),
                TechnologyBase = parts[4].Trim(' ', '"'),
                Chassis = parts[5],
                RulesLevel = parts[6].Trim(' ', '"'),
                Tonnage = long.Parse(parts[7]),
                Designer = parts[8].Trim(' ', '"'),
                BattleValue = long.Parse(parts[9]),
                Cost = long.Parse(parts[10]),
                Rating = parts[11],
                Extinct = parts[6].Contains("Extinct"),
                Selected = false
            };
            var name = mech.Name;
            var heroIndex = name.IndexOf('(');
            var delim = ")";

            // var isClanName = heroIndex != -1 && name.Substring(0, heroIndex).Count(c => c == ' ') == 1; // Can't do this... Black Hawk has 2 spaces ffs
            if (heroIndex == -1)
            {
                heroIndex = name.IndexOf("\"\"");
                delim = "\"\"";
            }

            if (heroIndex != -1)
            {
                var delimLen = delim.Length;
                var heroEndIndex = name.IndexOf(delim, heroIndex + delimLen, StringComparison.Ordinal);
                var heroName = name.Substring(heroIndex + delimLen, heroEndIndex - heroIndex - delimLen);
                name = name.Substring(0, heroIndex) + name.Substring(heroEndIndex + delimLen);
                name = name.Trim(' ', '\"');
                mech.HeroName = heroName;
            }

            var index = name.IndexOf(' ');
            var variantIndex = index;
            if (index != -1)
            {
                var potentialVariant = name.Substring(++index);

                // variantIndex += index;
                while (potentialVariant.Contains(" "))
                {
                    index = potentialVariant.IndexOf(' ');
                    if (index == -1)
                    {
                        break;
                    }

                    potentialVariant = potentialVariant.Substring(++index);
                    variantIndex += index;
                }

                mech.BaseModel = name.Substring(0, variantIndex).Trim('"');
                mech.Variant = name.Substring(++variantIndex).Trim('"');
            }
            else
            {
                mech.BaseModel = mech.Name.Trim('"');
                mech.Variant = "N/A";
            }

            // We have to do this because the file format is ambiguous, as in the case of the Archer (Wolf) which is a hero mech of Archer, and Baboon (Howler) which is a clan mech named Howler, referred to as Baboon by IS.
            mech.PossibleClanName = mech.HeroName;

            return mech;
        }
    }
}