namespace ITS.Propagation
{
    public static partial class EHata
    {
        /// <summary>
        /// Clutter classification environment
        /// </summary>
        public enum ClutterEnvironment : int
        {
            /// <summary>
            /// Urban clutter environment
            /// </summary>
            Urban = 24,

            /// <summary>
            /// Suburban clutter environment
            /// </summary>
            Suburban = 22,

            /// <summary>
            /// Rural clutter environment
            /// </summary>
            Rural = 20
        }
    }
}
