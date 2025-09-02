using Leagueinator.GUI.Model;
using Utility.Collections;

namespace Leagueinator.GUI.Controllers.Modules {


    public static class EventResultExtensions {
        public static T Sum<T>(this IEnumerable<T> results) where T : IResult<T> {
            T sum = T.CreateResult();
            foreach (T result in results) {
                sum = sum + result;
            }
            return sum;
        }

        public static (int Win, int Loss, int Tie) WLT<T>(this IEnumerable<T> results) where T : IResult<T> {
            (int Win, int Loss, int Tie) wlt = (0, 0, 0);
            foreach (T result in results) {
                switch (result.Result) {
                    case Model.Enums.GameResult.Vacant:
                        break;
                    case Model.Enums.GameResult.Loss:
                        wlt.Loss++;
                        break;
                    case Model.Enums.GameResult.Tie:
                        wlt.Tie++;
                        break;
                    case Model.Enums.GameResult.Win:
                        wlt.Win++;
                        break;
                }
            }
            return wlt;
        }
    }

    public class ResultsCollection {
        public static object Construct(Type resultType) {
            ArgumentNullException.ThrowIfNull(resultType);

            // Must implement IResult<resultType>
            bool ok = resultType.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IResult<>) &&
                i.GenericTypeArguments.Length == 1 &&
                i.GenericTypeArguments[0] == resultType
            );

            if (!ok) throw new ArgumentException($"Type must implement IResult<T> with T={resultType.Name}, got {resultType.FullName}");

            Type generic = typeof(ResultsCollection<>);  // open generic type
            Type closed = generic.MakeGenericType(resultType); // closed generic type
            return Activator.CreateInstance(closed)!;
        }
    }


    public class ResultsCollection<T> where T : IResult<T> {
        private readonly MultiMap<Players, T> TeamDict = [];
        private readonly MultiMap<string, T> PlayerDict = [];
        private List<Players> TeamRanks = [];
        private List<string> PlayerRanks = [];

        public ResultsCollection() { }

        public ResultsCollection(IHasTeams hasTeams) {
            this.AddTeams(hasTeams);
        }

        public void AddTeams(IHasTeams hasTeams) {
            foreach (TeamData teamData in hasTeams.AllTeams()) {
                if (teamData.IsEmpty()) continue;
                T t = T.CreateResult(teamData);
                TeamDict[teamData.ToPlayers()].Add(t);

                foreach (string name in teamData.ToPlayers()) {
                    PlayerDict[name].Add(t);
                }
            }

            List<(Players Players, T Result)> teamList = [];
            foreach ((Players players, List<T> results) in this.TeamDict) {
                T sum = T.CreateResult();
                foreach (T result in results) sum += result;
                teamList.Add((players, sum));
            }

            List<(string Player, T Result)> playerList = [];
            foreach ((string player, List<T> results) in this.PlayerDict) {
                T sum = T.CreateResult();
                foreach (T result in results) sum += result;
                playerList.Add((player, sum));
            }

            this.TeamRanks = teamList.OrderBy(tuple => tuple.Result)
                                     .Select(tuple => tuple.Players)
                                     .ToList();

            this.PlayerRanks = playerList.OrderBy(tuple => tuple.Result)
                                         .Select(tuple => tuple.Player)
                                         .ToList();
        }

        public List<T> this[Players players] => this.TeamDict[players];

        public List<T> this[string name] => this.PlayerDict[name];

        public int Rank(Players players) => this.TeamRanks.IndexOf(players) + 1;

        public int Rank(string name) => this.PlayerRanks.IndexOf(name) + 1;

        public IEnumerable<Players> Teams => this.TeamRanks;

        public IEnumerable<string> Players => this.PlayerRanks;

    }
}
