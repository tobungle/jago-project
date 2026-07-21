extends Node

var steam_peer : SteamMultiplayerPeer

func _on_lobby_created(connected : int, _this_lobby_id : int) -> void:
	if connected == 1:
		# Do other lobby initialization here
		# Create host peer
		steam_peer.create_host(0)
		steam_peer.server_relay = true
		multiplayer.set_multiplayer_peer(steam_peer)

func _on_lobby_joined(this_lobby_id : int, _permissions : int, _locked : bool, response : int) -> void:
	if response == Steam.CHAT_ROOM_ENTER_RESPONSE_SUCCESS:
		# Do other lobby initialization here
		var id = Steam.getLobbyOwner(this_lobby_id)
		# Only create a client if this player is not the host.
		if id != Steam.getSteamID():
			# Create a client peer and connect to the host
			var server_steam_id : int = Steam.getLobbyOwner(this_lobby_id)
			steam_peer.create_client(server_steam_id, 0)
			steam_peer.server_relay = true
			multiplayer.set_multiplayer_peer(steam_peer)
