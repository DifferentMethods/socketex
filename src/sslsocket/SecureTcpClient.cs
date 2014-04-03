using System.IO;
using System.Net;
using System.Net.Sockets;
using Org.BouncyCastle.Crypto.Tls;

namespace SocketEx
{
	public class SecureTcpClient : TcpClient
	{
		private readonly TlsClient tlsClient;
		private Stream secureStream;

		public SecureTcpClient (string host, int port)
            : this (host, port, new LegacyTlsClient (new AlwaysValidVerifyer ()))
		{

		}

		public SecureTcpClient (string host, int port, TlsClient tlsClient)
            : base (AddressFamily.InterNetwork)
		{
			this.tlsClient = tlsClient;
			this.host = host;
			this.port = port;
			InnerConnect ();
		}

		public override Stream GetStream ()
		{
			return secureStream == null ? base.GetStream () : this.secureStream;
		}

		protected override void HandleConnectionReady ()
		{
			var stream = this.GetStream ();
			var handler = new TlsProtocolHandler (stream);
			handler.Connect (tlsClient);
			this.secureStream = handler.Stream;
		}
	}
}
