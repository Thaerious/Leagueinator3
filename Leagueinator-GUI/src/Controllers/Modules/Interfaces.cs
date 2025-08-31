using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules {
    public interface IModule {
        public void LoadModule(Window targetWindow, MainController mainController);

        public void UnloadModule();
    }

    public interface IResult<T> : IAddable<T>, ICreateResult<T> where T : IResult<T> {
        public static string[] Labels { get; } = [];
        public string[] Cells();

        public readonly static int[] ColSizes = [];

        public GameResult Result { get; set; }
    }

    public interface IAddable<T> where T : IAddable<T> {
        static abstract T operator +(T left, T right);
    }

    public interface ICreateResult<T> where T : IResult<T>{
        static abstract T CreateResult();

        static abstract T CreateResult(TeamData teamData);
    }
}
