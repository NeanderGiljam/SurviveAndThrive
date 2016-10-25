using System;

/// <summary>
///		From the 4th digit == Type number
///		First 3 digits == Itteration number
/// 
///		Example:	100:13 == Pine : Tree
///					101:13 == Spruce : Tree
/// 
///		!!! Dont forget to add the ',' !!!
/// </summary>
public enum GameItemType {
	EmptyItem_0 = 0,

	// Animals
	Animal_Deer = 1001,
	Animal_Rabbit = 1101,

	// Grasses
    Grass_1 = 10010,
    Grass_2 = 10110,
    Grass_3 = 10210,

    // Plants
    Plant_Verns_1_1 = 10011,
	Plant_Verns_1_2 = 10111,
	Plant_Candlegg = 10211,
    Plant_Spitter = 10311,
	Plant_Shield = 10411,
	Plant_Corn = 10511,

    // Rocks
    Rock_Stone_Middle_1_1 = 10012,
	Rock_Stone_Middle_1_2 = 10112,
	Rock_Stone_Middle_2_1 = 10212,
	Rock_Stone_Middle_2_2 = 10312,
	Rock_Stone_Middle_3_1 = 10412,
	Rock_Stone_Middle_3_2 = 10512,

	// Trees
	Tree_PineTree_Base = 10013,

	Tree_PlainsTree_Cloudleaf = 12013,

	Tree_JungleTree_Thick_Hermit = 14013,
	Tree_JungleTree_TwoTop_Hermit = 14113,
	Tree_JungleTree_Long_Hermit = 14213,
	Tree_JungleTree_Thin_Hermit = 14313,
	Tree_JungleTree_Long_Vern = 14413,
	Tree_JungleTree_Palm = 14513,
	Tree_JungleTree_MossTop_Root = 14613,

	// Foods
	Food_DeerMeat = 10014,
	Food_Mabu = 10114,
	Food_Shroom = 10214,
	Food_Corn = 10314,

	// Weapons
	Weapon_Bow_Basic = 10015,
	Weapon_Arrow_Orient = 10115,
	Weapon_Spear_Basic = 11015,

	// Tools
	Tool_Axe_Basic = 10016,
	Tool_PickAxe_Basic = 11016,

	// Resource Items
	Resource_Stick = 10017,
	Resource_Stone = 10117,
}