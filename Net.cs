namespace Steamworks
{
	using Steamworks;

	public static class Net
	{
		public static void ClientSendTCPData(Packet _packet)
		{
			ClientSend.bytesSent += _packet.Length();
			ClientSend.packetsSent++;
			_packet.WriteLength();
			if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
			{
				LocalClient.instance.tcp.SendData(_packet);
				return;
			}
			SteamPacketManager.SendPacket(LocalClient.instance.serverHost.Value, _packet, P2PSend.Reliable, SteamPacketManager.NetworkChannel.ToServer);
		}
		public static void ServerSendTCPData(int toClient, Packet packet)
		{
			Packet packet2 = new();
			packet2.SetBytes(packet.CloneBytes());
			packet2.WriteLength();
			if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
			{
				Server.clients[toClient].tcp.SendData(packet2);
			}
			else
			{
				SteamPacketManager.SendPacket(Server.clients[toClient].player.steamId.Value, packet2, P2PSend.Reliable, SteamPacketManager.NetworkChannel.ToClient);
			}
		}
	}
}