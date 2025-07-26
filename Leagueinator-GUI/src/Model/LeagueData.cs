using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.Model {
    public class LeagueData : List<EventData> {

        private int NextUID = 1;
        public int GetNextUID() {
            return this.NextUID++;
        }

        public EventData GetEvent(int eventUID) {
            foreach (EventData @event in this) {
                if (@event.UID == eventUID) return @event;
            }
            throw new KeyNotFoundException();
        }

        internal EventData AddEvent() {
            EventData eventData = new() {
                UID = this.GetNextUID()
            };
            this.Add(eventData);
            return eventData;
        }

        public static LeagueData FromString(string s) {
            LeagueData leagueData = [];
            List<string> srcLines = [.. s.Split('\n')];

            string line = srcLines.Dequeue();
            var parts = line.Split('|');
            if (parts.Length != 2) throw new FormatException($"Invalid LeagueData string format: {line}.'");

            int eventCount = int.Parse(parts[0]);
            leagueData.NextUID = int.Parse(parts[1]);

            AddEvents(eventCount, srcLines, leagueData);
            return leagueData;
        }

        private static void AddEvents(int eventCount, List<string> srcLines, LeagueData leagueData) {
            for (int i = 0; i < eventCount; i++) {
                EventRecord eventRecord = EventRecord.FromString(srcLines.Dequeue());
                EventData eventData = EventData.FromRecord(eventRecord);
                leagueData.Add(eventData);
                AddRounds(srcLines, eventData, eventRecord);
            }
        }

        private static void AddRounds(List<string> srcLines, EventData eventData, EventRecord eventRecord) {
            for (int i = 0; i < eventRecord.RoundCount; i++) {
                RoundData roundData = eventData.AddRound(false);

                int matchCount = NextInt(srcLines);
                for (int j = 0; j < matchCount; j++) {
                    MatchRecord matchRecord = MatchRecord.FromString(srcLines.Dequeue());
                    MatchData matchData = MatchData.FromRecord(matchRecord);
                    roundData.Add(matchData);
                }

                int playerCount = NextInt(srcLines);
                for (int j = 0; j < playerCount; j++) {
                    RoundRecord roundRecord = RoundRecord.FromString(srcLines.Dequeue());
                    MatchData matchData = roundData[roundRecord.Lane];
                    TeamData teamData = matchData[roundRecord.Team];
                    teamData[roundRecord.Pos] = roundRecord.Name;
                }
            }
        }

        private static int NextInt(List<string> lines) {
            string line = lines.Dequeue();
            return int.Parse(line);
        }

        public override string ToString() {
            string sb = $"{this.Count}|{this.NextUID}\n";

            foreach (EventData @event in this) {
                sb += EventData.ToRecord(@event).ToString() + "\n";

                foreach (RoundData round in @event) {
                    RoundRecordList roundRecordList = new RoundRecordList(@event, round);

                    sb += $"{roundRecordList.Matches.Count}\n";
                    foreach (MatchRecord matchRecord in roundRecordList.Matches) {
                        sb += matchRecord.ToString() + "\n";
                    }

                    sb += $"{roundRecordList.Players.Count}\n";
                    foreach (RoundRecord roundRecord in roundRecordList.Players) {
                        sb += roundRecord.ToString() + "\n";
                    }
                }
            }

            return sb;
        }

        internal void RemoveEventByUID(int eventUID) {
            EventData? eventData = this.Where(e => e.UID == eventUID).FirstOrDefault()
                                ?? throw new KeyNotFoundException();

            this.Remove(eventData);
        }

        public IEnumerable<Record> Records() {
            foreach (EventData evenData in this) {
                foreach (RoundData roundData in evenData) {
                    foreach (MatchData matchData in roundData) {
                        foreach (TeamData teamData in matchData.Teams) {
                            foreach (string name in teamData) {
                                yield return new Record(evenData, roundData, matchData, teamData, name);
                            }
                        }
                    }
                }
            }
        }
    }
}
