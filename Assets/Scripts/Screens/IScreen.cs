    /// <summary>
    /// Screen interface
    /// Each UI screen must implement this interface
    /// </summary>
    interface IScreen {

        /// <summary>
        /// Screen name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Show screen
        /// </summary>
        void Show ();

        /// <summary>
        /// Hide screen
        /// </summary>
        void Hide ();
    }