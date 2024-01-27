namespace Kickstarter.Inputs
{
    /// <summary>
    /// Provides methods to enable a Player component to register and deregister inputs.
    /// </summary>
    public interface IInputReceiver
    {
        /// <summary>
        /// Registers inputs for a specific player.
        /// </summary>
        /// <param name="playerIdentifier">Identifier for the player whose inputs are being registered.</param>
        public void RegisterInputs(Player.PlayerIdentifier playerIdentifier);

        /// <summary>
        /// Deregisters inputs for a specific player.
        /// </summary>
        /// <param name="playerIdentifier">Identifier for the player whose inputs are being deregistered.</param>
        public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier);
    }
}
