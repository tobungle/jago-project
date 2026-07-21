extends Control

@export var singleplayer_btn : Button
@export var multiplayer_btn : Button
@export var settings_btn : Button
@export var quit_btn : Button

func _ready() -> void:
	singleplayer_btn.pressed.connect(on_singleplayer_pressed)
	multiplayer_btn.pressed.connect(on_multiplayer_pressed)
	quit_btn.pressed.connect(func() -> void: get_tree().quit())

func on_singleplayer_pressed() -> void:
	# Go to test scene.
	get_tree().change_scene_to_file("uid://18g6qsau6q1h")

func on_multiplayer_pressed() -> void:
	pass
