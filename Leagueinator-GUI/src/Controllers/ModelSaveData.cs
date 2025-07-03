using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;

namespace Leagueinator.GUI.Controllers {
    public class ModelSaveData(EventData eventData, RoundDataCollection roundDataCollection) {
        public RoundDataCollection RoundDataCollection { get; } = roundDataCollection;
        public EventData EventData { get; } = eventData;
    }
}
