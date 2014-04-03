// Modified to work with Windows Phone by Mikael Koskinen. http://mikaelkoskinen.net

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketEx
{
	/// <summary>
	/// The TcpClient class provides simple methods for connecting, sending, and receiving stream data over a network in synchronous blocking mode.
	/// </summary>
	public class TcpClient : IDisposable
	{
		private readonly AutoResetEvent autoResetEvent;
		private readonly NetworkStream networkStream;
		private bool responsePending;
		protected string host;
		protected int port;

		public Socket Client { get; set; }

		public TcpClient ()
			: this (AddressFamily.InterNetwork)
		{
		}

		public TcpClient (string host, int port)
			: this (AddressFamily.InterNetwork)
		{
			this.host = host;
			this.port = port;
			InnerConnect ();
		}

		public TcpClient (AddressFamily addressFamily)
		{
			Client = new Socket (addressFamily, SocketType.Stream, ProtocolType.Tcp);
			networkStream = new NetworkStream (Client);
		}

		public int Available {
			get { throw new NotSupportedException (); }
		}

		public bool Connected {
			get { return Client != null && Client.Connected; }
		}

		public bool Active {
			get { return Connected; }
		}

		public bool ExclusiveAddressUse {
			get { return false; }
		}

		public bool NoDelay {
			get { return true; }
			set { throw new NotImplementedException (); }
		}

		#region IDisposable Members

		public void Dispose ()
		{
			var stream = GetStream ();
			stream.Dispose ();

			try {
				Client.Shutdown (SocketShutdown.Both);
				Client.Close ();
			} catch (ObjectDisposedException ex) {
				Debug.WriteLine (ex.Message);
			} catch (SocketException ex) {
				Debug.WriteLine (ex.Message);
			}
		}

		#endregion

		public void Connect (string host, int port)
		{
			this.host = host;
			this.port = port;
			InnerConnect ();
		}

		protected virtual void HandleConnectionReady ()
		{
		}

		protected void InnerConnect ()
		{
			Client.Connect (this.host, this.port);
			HandleConnectionReady ();
		}

		public virtual Stream GetStream ()
		{
			return networkStream;
		}
	}
}