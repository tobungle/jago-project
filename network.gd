extends Node

signal got_lobbies(lobbies : Array)

var lobby_id : int
var steam_peer : SteamMultiplayerPeer
var lobby_name : String
var persona_name : String

var lobby_players : Dictionary = {}

func _ready() -> void:
	Steam.lobby_match_list.connect(on_got_lobbies)
	Steam.lobby_joined.connect(on_lobby_joined)
	Steam.join_requested.connect(_on_lobby_join_requested)
	persona_name = Steam.getPersonaName()

func setup_steam_peer() -> void:
	steam_peer = SteamMultiplayerPeer.new()
	steam_peer.peer_connected.connect(on_peer_connected)
	steam_peer.peer_disconnected.connect(on_peer_disconnected)

# Request a lobby be created with steam networking
func steam_host(_lobby_name : String) -> void:
	# Set the lobby name parameter (to be used later in _on_lobby_created),
	# - set to a default value if there is no given name
	lobby_name = _lobby_name
	if _lobby_name == "":
		lobby_name = "%s's lobby" % persona_name
	# Parameters are visbility and max members, make this customizeable later
	Steam.createLobby(Steam.LOBBY_TYPE_PUBLIC, 2)

# Called when a lobby is created, do setup here
func _on_lobby_created(connected : int, _this_lobby_id : int) -> void:
	if connected == 1:
		# Create host peer
		setup_steam_peer()
		steam_peer.create_host(0)
		steam_peer.server_relay = true
		multiplayer.set_multiplayer_peer(steam_peer)
		# Steam lobby setup
		lobby_id = _this_lobby_id
		Steam.setLobbyData(_this_lobby_id, "lobby_name", lobby_name)

# Called when a lobby is joined, do setup here
func _on_lobby_joined(this_lobby_id : int, _permissions : int, _locked : bool, response : int) -> void:
	if response == Steam.CHAT_ROOM_ENTER_RESPONSE_SUCCESS:
		# Do other lobby initialization here
		var id = Steam.getLobbyOwner(this_lobby_id)
		# Only create a client if this player is not the host.
		if id != Steam.getSteamID():
			# Create a client peer and connect to the host
			setup_steam_peer()
			var server_steam_id : int = Steam.getLobbyOwner(this_lobby_id)
			steam_peer.create_client(server_steam_id, 0)
			steam_peer.server_relay = true
			multiplayer.set_multiplayer_peer(steam_peer)

# Ask Steam for lobbies, will call Steam.lobby_match_list eventually
func request_lobbies() -> void:
    # Apply filters here
	Steam.addRequestLobbyListFilterSlotsAvailable(1)
	Steam.requestLobbyList()

# Triggered upon Steam.lobby_match_list
func on_got_lobbies(lobbies : Array) -> void:
	got_lobbies.emit(lobbies)

# Connect to a lobby manually (thru entering a lobby id or clicking on a lobby button)
func join_lobby(lobby : int) -> void:
	Steam.joinLobby(lobby)

# Connect to a lobby thru invite, clicking 'join game' on profile, etc.
func _on_lobby_join_requested(lobby : int, friend_id : int) -> void:
	var friend_joining: String = Steam.getFriendPersonaName(friend_id)
	print("Joining lobby with..." % friend_joining)
	Steam.joinLobby(lobby)
	lobby_id = lobby

# Called once Steam gives a response regarding lobby joining, erm unimplemented right now
func on_lobby_joined(lobby : int, permissions : int, locked : bool, response : int) -> void:
	# I think 1 is success but idk
	if response != 1:
		print_debug("Fucked!")
		get_tree().quit()
		return

func on_peer_connected(peer_id : int) -> void:
	# Peers that have connected have two ids
	# A randomly generated 'peer id' that Godot uses; is only relevant for this session
	# And a permanent 'steam id', which tells you which Steam user they are
	# We associate both of these together in a dictionary :3

	var steam_id = steam_peer.get_steam_id_for_peer_id(peer_id)
	lobby_players[peer_id] = {
		"steam_id" : steam_id,
		"steam_name" : Steam.getFriendPersonaName(steam_id)
	}
	# To get someone's Steam id from their session only peer id, you'd do
	# var steam_id = lobby_players[peer_id]["steam_id"]

func on_peer_disconnected(peer_id : int) -> void:
	lobby_players.erase(peer_id)
