using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers {
    public class ModelSaveData(EventData eventData, List<RoundData> roundDataCollection) {
        public List<RoundData> RoundDataCollection { get; } = roundDataCollection;
        public EventData EventData { get; } = eventData;
    }
}
