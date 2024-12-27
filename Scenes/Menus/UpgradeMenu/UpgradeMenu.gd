extends CanvasLayer
class_name UpgradeMenu

@export var upgradeContainer: HBoxContainer
@export var player: Player
@export var rerollButton: Button
@export var rerollLabel: RichTextLabel
const UPGRADE_BOX: PackedScene = preload("res://Scenes/Menus/UpgradeMenu/UpgradeBox/UpgradeBox.tscn")

var allUpgrades: Array[String] = [
	"Speed Boost",
	"Double Jump",
	"Extra Health",
	"Faster Reload",
	"Magnetic Pull",
	"Piercing Bullets",
	"Shotgun Shells",
	"More Bullets",
	"More Damage",
	"Faster Fire Rate"
]

var currentUpgrades: Array[String] = []
var random: RandomNumberGenerator = RandomNumberGenerator.new()

func ShowUpgradeMenu():
	currentUpgrades = allUpgrades.duplicate()
	rerollLabel.text = "[center][color=white][font_size=36]REROLL (1)"
	rerollButton.disabled = false
	GenerateBoxes()
	get_tree().paused = true
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	Globals.hasMenu = true
	show()

func HideUpgradeMenu():
	get_tree().paused = false
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	Globals.hasMenu = false
	hide()

func GenerateBoxes():
	ClearBoxes()
	for i in range(3):
		var upgradeBoxInstance: UpgradeBox = UPGRADE_BOX.instantiate()
		var choosen: int = random.randi_range(0, currentUpgrades.size() - 1)
		upgradeBoxInstance.upgradeName = currentUpgrades[choosen]
		upgradeContainer.add_child(upgradeBoxInstance)
		upgradeBoxInstance.player = player
		currentUpgrades.remove_at(choosen)

func ClearBoxes():
	for child in upgradeContainer.get_children():
		upgradeContainer.remove_child(child)

func _onRerollButtonPressed():
	rerollButton.disabled = true
	rerollLabel.text = "[center][color=white][font_size=36]REROLL (0)"
	currentUpgrades = allUpgrades.duplicate()
	GenerateBoxes()
