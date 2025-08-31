using Leagueinator.GUI.Model.Enums;
using Utility.Extensions;

namespace Leagueinator.GUI.Model {

    public static class Parser {

        public static void GetNextLine<T>(this List<string> lines, string label, out T value) {
            string line = lines.Dequeue().Trim();
            while (string.IsNullOrEmpty(line) || line.StartsWith("#")) {

                try {
                    line = lines.Dequeue().Trim();
                }
                catch (Exception e) {
                    throw new ParseException($"Exception while getting next line: {label}", e);
                }
            }

            line = line[label.Length..];

            try {
                if (typeof(T).IsEnum) {
                    value = (T)Enum.Parse(typeof(T), line, ignoreCase: true);
                }
                else {
                    value = (T)Convert.ChangeType(line, typeof(T));
                }
            }
            catch (Exception e) {
                throw new ParseException($"Exception while getting next line: {label}", e);
            }
        }

        public static void GetNextLine<T>(this List<string> lines, string label, out T value, Func<string, T> parser) {
            string line = lines.Dequeue().Trim();
            while (string.IsNullOrEmpty(line) || line.StartsWith("#")) {
                line = lines.Dequeue().Trim();
            }

            line = line[label.Length..];
            value = parser(line);
        }

        public static void Seek(this List<string> lines, string seek) {
            while (lines.Count > 0) {
                if (lines[0].StartsWith(seek)) break;
                lines.Dequeue();
            }
        }
    }

    public class Loader {

        private List<string> Lines = [];

        private LeagueData? LeagueData;

        public LeagueData Load(string source) {
            this.LeagueData = new();
            Lines = [.. source.Split("\n")];

            this.Lines.Seek("No. of Events:");
            this.Process();

            return this.LeagueData;
        }

        public void Process() {
            this.Lines.GetNextLine("No. of Events:", out int eventCount);
            for (int i = 0; i < eventCount; i++) this.ProcessEvent();
        }

        public void ProcessEvent() {
            this.Lines.GetNextLine("Event Name:", out string eventName);
            this.Lines.GetNextLine("Number of Lanes:", out int laneCount);
            this.Lines.GetNextLine("Default Ends:", out int defaultEnds);
            this.Lines.GetNextLine("Match Format:", out MatchFormat matchFormat);
            this.Lines.GetNextLine("Event Type:", out EventType eventType);
            this.Lines.GetNextLine("Match Scoring:", out MatchScoring matchScoring);

            EventData eventData = this.LeagueData!.AddEvent(eventName);
            eventData.DefaultEnds = defaultEnds;
            eventData.LaneCount = laneCount;
            eventData.DefaultMatchFormat = matchFormat;            
            eventData.EventType = eventType;           
            eventData.MatchScoring = matchScoring;

            this.Lines.GetNextLine("No. of Rounds:", out int roundCount);
            for (int i = 0; i < roundCount; i++) {
                this.Lines.Seek($"Round {i}:");
                this.ProcessRound(eventData);
            }
        }

        private void ProcessRound(EventData eventData) {
            this.Lines.Seek($"Match 0:");
            RoundData roundData = eventData.AddRound(false);
            
            for (int i = 0; i < eventData.LaneCount; i++) {
                this.Lines.GetNextLine($"Match {i}:", out string line);
                List<string> roundLines = [.. line.Split("|")];

                roundLines.GetNextLine("MatchFormat:", out MatchFormat matchFormat);
                roundLines.GetNextLine("Players:", out string[][] players, ParseStringMatrix);
                roundLines.GetNextLine("Score:", out int[] scores, ParseIntArray);
                roundLines.GetNextLine("TB:", out int tieBreaker);
                roundLines.GetNextLine("Ends:", out int ends);

                MatchData matchData = new (roundData){
                    MatchFormat = matchFormat,
                    Ends = ends,
                    TieBreaker = tieBreaker
                };

                for (int j = 0; j < scores.Length; j++) {
                    matchData.Score[j] = scores[j];
                }

                for (int t = 0; t < players.Length; t++){
                    for (int u = 0; u < players[t].Length; u++) {
                        matchData.Teams[t].SetPlayer(u, players[t][u]);
                    }
                }

                roundData.AddMatch(matchData);
            }
        }

        private static int[] ParseIntArray(string source) {
            source = source.Trim();
            return source.Trim('[', ']')
                         .Split(',')
                         .Select(int.Parse)
                         .ToArray();            
        }

        private static string[] ParseStringArray(string source) {
            source = source.Trim();
            string inner = source.Trim().TrimStart('[').TrimEnd(']');
            string[] result = [.. inner.Split(',', StringSplitOptions.TrimEntries)];
            return [.. result];
        }

        private static string[][] ParseStringMatrix(string source) {            
            List<string[]> result = [];
            string[] split = [.. source.TrimStart('[').TrimEnd(']').Split("][").Select(s => s.Trim())];
            foreach(string s in split) result.Add(ParseStringArray(s));
            return [.. result];
        }
    }
}
