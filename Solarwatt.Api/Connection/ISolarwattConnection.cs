namespace Solarwatt.Api.Connection
{
	public interface ISolarwattConnection
	{
		/// <summary>
		/// Gets the username to log in.
		/// </summary>
		/// <returns></returns>
		string Username { get; }

		/// <summary>
		/// Gets the password to log in
		/// </summary>
		/// <returns></returns>
		string Password { get; }

		/// <summary>
		/// Gets the device location to query the data for.
		/// Format is like XXX00-000000000:0
		/// </summary>
		/// <returns></returns>
		string DeviceLocation { get; }

		/// <summary>
		/// Gets the proxy to be used for web request.
		/// Null for default behavior.
		/// </summary>
		System.Net.IWebProxy Proxy { get; }
	}
}