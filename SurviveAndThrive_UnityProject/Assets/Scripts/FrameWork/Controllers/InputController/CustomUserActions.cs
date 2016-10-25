using InControl;

public class CustomUserActions : PlayerActionSet {

	// TODO: Make more allround -> move == leftStick etc.
	public PlayerAction RotateCameraRight;
	public PlayerAction RotateCameraLeft;

	public PlayerAction moveUp;
	public PlayerAction moveDown;
	public PlayerAction moveLeft;
	public PlayerAction moveRight;

	public PlayerAction rotateUp;
	public PlayerAction rotateDown;
	public PlayerAction rotateLeft;
	public PlayerAction rotateRight;

	public PlayerAction walk;
	public PlayerAction harvest;
	public PlayerAction use;

	public PlayerAction pause;

	public OneAxisInputControl MoveHorizontal;
	public OneAxisInputControl MoveVertical;

	public OneAxisInputControl RotateHorizontal;
	public OneAxisInputControl RotateVertical;

	public CustomUserActions() {

		// -------------- Camera Controls --------------
		RotateCameraRight = CreatePlayerAction("Rotate Camera Right");
		RotateCameraLeft = CreatePlayerAction("Rotate Camera Left");

		// -------------- Movement --------------
		moveUp = CreatePlayerAction("MoveUp");
		moveDown = CreatePlayerAction("MoveDown");
		moveLeft = CreatePlayerAction("MoveLeft");
		moveRight = CreatePlayerAction("MoveRight");

		MoveHorizontal = CreateOneAxisPlayerAction(moveLeft, moveRight);
		MoveVertical = CreateOneAxisPlayerAction(moveDown, moveUp);

		// -------------- Speed --------------
		walk = CreatePlayerAction("Walk");

		// -------------- Events --------------
		harvest = CreatePlayerAction("Harvest");
		use = CreatePlayerAction("Use");
		pause = CreatePlayerAction("Pause");

		// -------------- Rotation --------------
		rotateUp = CreatePlayerAction("RotateUp");
		rotateDown = CreatePlayerAction("RotateDown");
		rotateLeft = CreatePlayerAction("RotateLeft");
		rotateRight = CreatePlayerAction("RotateRight");

		RotateHorizontal = CreateOneAxisPlayerAction(rotateLeft, rotateRight);
		RotateVertical = CreateOneAxisPlayerAction(rotateDown, rotateUp);
	}
}
