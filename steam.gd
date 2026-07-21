extends Node

func _init() -> void:
    # Set game's Steam app ID here
    OS.set_environment("SteamAppId", str(480))
    OS.set_environment("SteamGameId", str(480))

func _ready() -> void:
    initialize_steam()

func initialize_steam() -> void:
    var initialize_response: Dictionary = Steam.steamInitEx()
    print("Did Steam initialize?: %s " % initialize_response)

func _process(_delta: float) -> void:
    Steam.run_callbacks()
