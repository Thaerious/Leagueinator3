namespace Leagueinator.GUI.Controllers.NamedEvents {
    
    /// <summary>
    /// Classes marked with NamedEntityContext will share Pause, Unpause state.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NamedEventContext : Attribute {
        public NamedEventContext(string contextName) { 
            this.ContextName = contextName;
        }
        public string ContextName { get; }
    }
}
