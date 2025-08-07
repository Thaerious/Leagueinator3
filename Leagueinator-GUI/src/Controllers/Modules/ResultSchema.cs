using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers.Modules {
    public abstract class ResultSchema {

        public abstract List<string> Fields { get; }

        public abstract List<object?> Values { get; }
    }
}
