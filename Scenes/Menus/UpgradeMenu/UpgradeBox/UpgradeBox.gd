extends Control
class_name UpgradeBox

@export var upgradeName: String
@export var upgradeNameLabel: RichTextLabel
@export var upgradeStatLabel: RichTextLabel
var player: Player

func _ready():
	upgradeNameLabel.text = "[center][color=white][font_size=30]%s" % upgradeName
	match upgradeName:
		"Speed Boost":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+25% Speed"
		"Double Jump":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+1 Extra jump"
		"Extra Health":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+50% HP"
		"Faster Reload":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+25% reload speed"
		"Magnetic Pull":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+100% drop pickup area"
		"Piercing Bullets":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+1 Piercing"
		"Shotgun Shells":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+1 Bullet per shot"
		"More Bullets":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+3 Bullets in magazine"
		"More Damage":
			upgradeStatLabel.text = "[center][color=white][font_size=30]+1 Damage per bullet"
		"Faster Fire Rate":
			upgradeStatLabel.text = "[center][color=white][font_size=30]25% Faster fire rate"

func _onChooseButtonPressed():
	player.UpgradeStat(upgradeName)
