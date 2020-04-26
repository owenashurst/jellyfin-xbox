namespace Jellyfin.Models
{
    /// <summary>
    /// Indicates from View Model to View on what other actions
    /// the UI should do after the business logic.
    /// </summary>
    public class ControllerButtonHandledResult
    {
        #region Additional methods

        public bool ShouldOsdOpen { get; set; }
        
        #endregion
    }
}
