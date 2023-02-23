namespace CustomerService.API
{
    /// <summary>
    /// Client application role that can be authorised
    /// </summary>
    public static class ClientAppRoles
    {
        /// <summary>
        /// Machine to machine type authorisation
        /// </summary>
        public const string M2M = "M2M";

        /// <summary>
        /// AD group required 
        /// </summary>
        public const string DigitalTeam = "Digital Team";
    }
}
