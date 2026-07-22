extends Control

@export var start_menu : Control
@export var multiplayer_menu : Control
@export var host_menu : Control
@export var join_menu : Control

@export var lobby_button_container : VBoxContainer

func set_menu(which : String) -> void:
	start_menu.visible = which == "start"
	multiplayer_menu.visible = which == "mp"
	host_menu.visible = which == "host"
	join_menu.visible = which == "join"

func _ready() -> void:
	set_menu("start")
	Network.got_lobbies.connect(_spawn_lobby_buttons)

func _on_singleplayer_pressed() -> void:
	get_tree().change_scene_to_file("uid://18g6qsau6q1h")

func _on_multiplayer_pressed() -> void:
	set_menu("mp")

func _on_quit_pressed() -> void:
	get_tree().quit()

func _on_to_host_menu_pressed() -> void:
	set_menu("host")

func _on_to_join_menu_pressed() -> void:
	set_menu("join")
	populate_lobbies()

func _on_back_pressed() -> void:
	set_menu("start")

func _on_host_confirm_pressed() -> void:
	_on_singleplayer_pressed()
	Network.steam_host($HostMenu/VBoxContainer/LineEdit.text)

func populate_lobbies() -> void:
	for i : Node in lobby_button_container.get_children():
		i.queue_free()
	Network.request_lobbies()

func _spawn_lobby_buttons(lobbies : Array) -> void:
	for lobby_id in lobbies:
		var lobby_name = Steam.getLobbyData(lobby_id, "lobby_name")
		if lobby_name == "":
			lobby_name = "Lobby %s" % lobby_id
		var lobby_btn : Button = Button.new()
		lobby_btn.text = lobby_name
		lobby_button_container.add_child(lobby_btn)
