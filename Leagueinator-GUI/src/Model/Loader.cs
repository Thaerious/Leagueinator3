using Leagueinator.GUI.Model.Enums;
using System.Diagnostics;

namespace Leagueinator.GUI.Model {

    public record Line(int Num, string Value);

    public class LineList {

        private List<Line> Lines = [];
        public int LineNum => this.Lines.Count > 0 ? this.Lines[0].Num : 0;

        public LineList(string source) {
            string[] rawLines = [.. source.Split(['\n'], StringSplitOptions.TrimEntries)];

            int lineNumber = 1;
            foreach (string rawLine in rawLines) {
                var parts = rawLine.Split('|', StringSplitOptions.TrimEntries);

                foreach (string part in parts) {
                    if (string.IsNullOrEmpty(part)) continue;   
                    if (part.StartsWith('#')) continue;
                    this.Lines.Add(new Line(lineNumber, part));
                }

                lineNumber++;
            }
        }

        public string Head => this.Lines[0].Value;

        public int Count => this.Lines.Count;

        public string PeekLabel() {
            string line = this.Lines[0].Value;
            int splitAt = line.IndexOf(':');
            string label = line[..splitAt];
            return label;
        }

        public void Next() {
            if (!this.HasNext()) throw new ParseException("No more lines.");
            this.Lines.RemoveAt(0);

            if (this.HasNext()) {
                Debug.WriteLine($"{this.LineNum}> {this.Head}");
            }
            else {
                Debug.WriteLine("[END]");
            }
        }

        public bool HasNext() {
            return this.Count > 0;
        }

        public void Seek(string label) {
            while (this.HasNext()) {
                if (this.Lines[0].Value.StartsWith(label)) break;
                this.Next();
            }
        }

        public void Process<T>(string label, out T value) {
            try {
                if (!this.Head.StartsWith(label)) throw new ParseException($"Mismatched label on line {this.LineNum}: expected {label}.");
                this._Process(label, out value);
                this.Next();
            }catch(Exception ex){
                throw new ParseException($"Exception on line '{this.LineNum}', expected label: '{label}'.", ex);
            }
        }

        public void Process<T>(string label, out T value, T defValue) {
            if (!this.Head.StartsWith(label)) {
                value = defValue;
                return;
            }

            this._Process(label, out value);
            this.Next();
        }

        public void Process<T>(string label, out T value, Func<string, T> func) {
            if (!this.Head.StartsWith(label)) throw new ParseException($"Mismatched label: expected {label}.");
            string line = this.Head[label.Length..];
            value = func(line);
            this.Next();
        }

        private void _Process<T>(string label, out T value) {
            string line = this.Head[label.Length..];

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
    }


    public class Loader {

        private LineList Lines = new("");

        private LeagueData? LeagueData;

        public LeagueData Load(string source) {
            this.LeagueData = new();
            this.Lines = new(source);
            this.Lines.Seek("No. of Events:");
            while (this.Lines.HasNext()) this.Process();
            return this.LeagueData;
        }

        public void Process() {
            this.Lines.Process("No. of Events:", out int eventCount);
            for (int i = 0; i < eventCount; i++) this.ProcessEvent();
        }

        public void ProcessEvent() {
            this.Lines.Process("Event Name:", out string eventName);
            this.Lines.Process("Number of Lanes:", out int laneCount);
            this.Lines.Process("Default Ends:", out int defaultEnds);
            this.Lines.Process("Match Format:", out MatchFormat matchFormat);
            this.Lines.Process("Event Type:", out EventType eventType);
            this.Lines.Process("Match Scoring:", out MatchScoring matchScoring);
            this.Lines.Process("Head to Head:", out bool headToHead);

            EventData eventData = this.LeagueData!.AddEvent(eventName);
            eventData.DefaultEnds = defaultEnds;
            eventData.LaneCount = laneCount;
            eventData.DefaultMatchFormat = matchFormat;
            eventData.EventType = eventType;
            eventData.MatchScoring = matchScoring;
            eventData.HeadToHeadScoring = headToHead;

            this.Lines.Process("No. of Rounds:", out int roundCount);
            for (int i = 0; i < roundCount; i++) {
                this.Lines.Seek($"Round {i}:");
                this.ProcessRound(eventData);
            }
        }

        private void ProcessRound(EventData eventData) {
            this.Lines.Seek($"Match 0:");
            RoundData roundData = eventData.AddRound(false);

            for (int i = 0; i < eventData.LaneCount; i++) {
                this.Lines.Seek($"Match {i}:");
                this.Lines.Seek($"MatchFormat:");

                this.Lines.Process("MatchFormat:", out MatchFormat matchFormat);
                this.Lines.Process("Players:", out string[][] players, ParseStringMatrix);
                this.Lines.Process("Score:", out int[] scores, ParseIntArray);
                this.Lines.Process("TB:", out int tieBreaker);
                this.Lines.Process("Ends:", out int ends);

                MatchData matchData = new(roundData) {
                    MatchFormat = matchFormat,
                    Ends = ends,
                    TieBreaker = tieBreaker
                };

                for (int j = 0; j < scores.Length; j++) {
                    matchData.Score[j] = scores[j];
                }

                for (int t = 0; t < players.Length; t++) {
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
            foreach (string s in split) result.Add(ParseStringArray(s));
            return [.. result];
        }
    }
}
