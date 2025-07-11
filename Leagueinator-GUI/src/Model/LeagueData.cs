

using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Diagnostics.Tracing;

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
            Debug.WriteLine($" * Add Event {eventData.UID}");
            return eventData;
        }

        public static LeagueData FromString(string s) {
            LeagueData leagueData = [];
            List<string> lines = [.. s.Split('\n')];

            string line = lines.Dequeue();
            var parts = line.Split('|');
            if (parts.Length != 2) throw new FormatException($"Invalid LeagueData string format: {line}.'");

            int eventCount = int.Parse(parts[0]);
            leagueData.NextUID = int.Parse(parts[1]);

            AddEvents(eventCount, lines, leagueData);
            return leagueData;
        }

        private static void AddEvents(int eventCount, List<string> lines, LeagueData leagueData) {
            for (int i = 0; i < eventCount; i++) {
                EventRecord eventRecord = EventRecord.FromString(lines.Dequeue());
                EventData eventData = EventData.FromRecord(eventRecord);
                leagueData.Add(eventData);
                AddRounds(lines, eventData, eventRecord);    
            }
        }

        private static void AddRounds(List<string> lines, EventData eventData, EventRecord eventRecord) {
            for (int i = 0; i < eventRecord.RoundCount; i++) {
                RoundData roundData = eventData.AddRound(false);

                int matchCount = NextInt(lines);
                for (int j = 0; j < matchCount; j++) {
                    MatchRecord matchRecord = MatchRecord.FromString(lines.Dequeue());
                    MatchData matchData = MatchData.FromRecord(matchRecord);
                    roundData.Add(matchData);
                }

                int playerCount = NextInt(lines);
                for (int j = 0; j < playerCount; j++) {
                    RoundRecord roundRecord = RoundRecord.FromString(lines.Dequeue());
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
    }
}
