using System;
namespace SundaysApp.Model
{
    [System.Diagnostics.DebuggerDisplay("{DeviceName}")]
    public class Auth
    {
        /// <summary>
        /// Gets the device full name like "Lastname, City", for example
        /// </summary>
        /// <returns></returns>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets the username to log in.
        /// </summary>
        /// <returns></returns>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the password to log in
        /// </summary>
        /// <returns></returns>
        public string Password { get; set; }

        /// <summary>
        /// Gets the device location to query the data for.
        /// Format is like XXX00-000000000:0
        /// </summary>
        /// <returns></returns>
        public string DeviceLocation { get; set; }

        /// <summary>
        /// Gets or sets the code to access the Sundays function api.
        /// </summary>
        public string ApiCode { get; set; }

        /// <summary>
        /// Gets whether all values are set properly
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(DeviceName) &&
                                  !string.IsNullOrEmpty(UserName) &&
                                  !string.IsNullOrEmpty(Password) &&
                                  !string.IsNullOrEmpty(DeviceLocation) &&
                                  !string.IsNullOrEmpty(ApiCode);
    }
}
