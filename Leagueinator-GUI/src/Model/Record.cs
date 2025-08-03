namespace Leagueinator.GUI.Model {
    public record Record {
        public readonly EventData? EventData;
        public readonly RoundData RoundData;
        public readonly MatchData MatchData;
        public readonly TeamData TeamData;
        public readonly string Name;

        public Record(EventData eventData, RoundData roundData, MatchData matchData, TeamData teamData, string name) {
            this.EventData = eventData;
            this.RoundData = roundData;
            this.MatchData = matchData;
            this.TeamData = teamData;
            this.Name = name;
        }

        public Record(RoundData roundData, MatchData matchData, TeamData teamData, string name) {
            this.RoundData = roundData;
            this.MatchData = matchData;
            this.TeamData = teamData;
            this.Name = name;
        }

        public string ToCSV() {
            if (this.EventData == null) throw new NotSupportedException("Record must have event data to export CSV");

            int teamIndex = Array.IndexOf(this.MatchData.Teams, this.TeamData);
            int[] score = this.MatchData.Score;
            char[] wlt = LabelWinLossTie(score);
            return $"{this.EventData.Date},{this.EventData.EventName},{this.EventData.IndexOf(this.RoundData)},{this.MatchData.Lane},{teamIndex},{this.MatchData.Score[teamIndex]},{this.MatchData.TieBreaker},{this.MatchData.Ends},{this.Name},{wlt[teamIndex]}\n";
        }

        static char[] LabelWinLossTie(int[] values) {
            if (values.Length == 2) {
                if (values[0] == values[1]) return ['T', 'T'];
                if (values[0] > values[1]) return ['W', 'L'];
                return ['L', 'W'];
            }
            else if (values.Length == 4) {
                int[] indices = Enumerable.Range(0, 4)
                    .OrderByDescending(i => values[i])
                    .ToArray();

                char[] result = new char[4];

                result[indices[0]] = 'W';
                result[indices[1]] = 'T';
                result[indices[2]] = 'L';
                result[indices[3]] = 'L';

                return result;
            }
            else throw new Exception($"Length {values.Length} not supported");
        }
    }
}
