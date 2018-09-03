using System;
namespace SundaysApp
{
    public class SundaysAuth
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
    }
}
