extends Control

@export var start_menu : Control
@export var multiplayer_menu : Control
@export var host_menu : Control

func set_menu(which : String) -> void:
	start_menu.visible = which == "start"
	multiplayer_menu.visible = which == "mp"
	host_menu.visible = which == "host"

func _on_singleplayer_pressed() -> void:
	get_tree().change_scene_to_file("uid://18g6qsau6q1h")

func _on_multiplayer_pressed() -> void:
	set_menu("mp")

func _on_quit_pressed() -> void:
	get_tree().quit()

func _on_to_host_menu_pressed() -> void:
	set_menu("host")

func _on_to_join_menu_pressed() -> void:
	set_menu("mp")

func _on_back_pressed() -> void:
	set_menu("start")

func _on_host_confirm_pressed() -> void:
	_on_singleplayer_pressed()
	Network.steam_host($HostMenu/VBoxContainer/LineEdit.text)
