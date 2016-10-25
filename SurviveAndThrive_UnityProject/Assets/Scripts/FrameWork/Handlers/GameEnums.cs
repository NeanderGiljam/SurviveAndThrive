using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public enum CameraSchemes {
    BaseFirstPersonView = 0
}

[Serializable]
public enum RotationAxes { 
    MouseXAndY = 0, 
    MouseX = 1, 
    MouseY = 2 
}

[Serializable]
public enum Direction {
	In = 0,
	Out = 1
}

[Serializable]
public enum InputType {
    Joystick,
    Keyboard
}

[Serializable]
public enum BiomeType {
	/* 
	http://www.ucmp.berkeley.edu/glossary/gloss5/biome/
	*/

	// Undefined
	Undefined = -1,

	// Forests
	PineForest = 0,
	Jungle = 1,

	// Grasslands
	Plains = 100,
	Savannah = 101,

	// Deserts
	Desert = 300,

	// Tundra
	Snow = 400,

	// Aquatic
	River = 500,
	Lake = 501,
	Ocean = 502,
	Swamp = 503
}

[Serializable]
public enum SpawnSettingType {
	Tree = 0,
    Stone = 1,
	Plant = 2,
	Grass = 3,
	Life = 4,
	Food = 5,
	Resources = 6
}

[Serializable]
public enum AnimalState {
	None = 0,
	Idle = 1,
	Alerted = 2,
	FindPath = 3,
	Moving = 4,
	Running = 5,
	Grazing = 6,
	Dying = 7,
	Dead = 8
}

//[Serializable]
//public enum PlayerState {
//	Still = 0,
//	Idle = 1,
//	Walk = 2,
//	Run = 3,
//	Collect = 4,
//	Attack = 5
//}