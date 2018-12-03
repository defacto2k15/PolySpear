using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
	private HexPosition position;

	public enum State
	{
		Move,
		Attack,
		Wait
	}

	private State state = State.Move;
	public int Player;
	public int MaxHP;
	private int hp;
	public float Strength;
	public float Variation;
	public int Speed;
	public int Range;
	private int hpBarWidth = 64;
	private int hpBarHeight = 16;
	private bool moving = false;
	private float t;
	private Vector3[] path;
	private int n;
	//position on the path
	private const float MOTION_SPEED = 0.05f;
	private IPlayerInterface player = null;
	private bool localPlayer = false;

	public void setPlayerInterface (IPlayerInterface player, bool localPlayer)
	{
		this.player = player;
		this.localPlayer = localPlayer;
	}

	public int HP {
		get {
			return hp;
		}
	}
	
	/*void SetPosition (HexPosition position) {
		this.position = position;
		transform.position = position.getPosition ();
		position.add ("Unit", this);
		grid.SendMessage ("ActionComplete");
	}
	
	public void Move (HexPosition desitination) {
		grid.SendMessage ("MessageRecieved");
		if (desitination.containsKey ("Unit")) {
			grid.SendMessage ("ActionComplete");
			return;
		}
		position.delete ("Unit");
		desitination.add ("Unit", this);
		transform.position = desitination.getPosition();
		position = desitination;
		grid.SendMessage ("ActionComplete");
	}*/
	
	public HexPosition Coordinates {
		get {
			return position;
		}
		set {
			if (position != null) {
				position.remove ("Unit");
			}
			position = value;
			transform.position = value.getPosition ();
			value.add ("Unit", this);
		}
	}

	public State Status {
		get { return state; }
	}

	public void move (HexPosition[] path)
	{
		if (path.Length < 2) {
			state = State.Attack;
			return;
		}
		HexPosition destination = path [path.Length - 1];
		this.path = new Vector3[path.Length];
		for (int i = 0; i < path.Length; ++i) {
			this.path [i] = path [i].getPosition ();
		}
		state = State.Attack;
		if (destination.containsKey ("Unit")) {
			print ("ERROR: Space occupied.");
			moveComplete ();
			return;
		}
		position.remove ("Unit");
		destination.add ("Unit", this);
		//transform.position = desitination.getPosition();
		t = 0;
		n = 0;
		moving = true;
		position = destination;
	}

	public void skipMove ()
	{
		state = State.Attack;
	}

	public void skipAttack ()
	{
		state = State.Wait;
	}

	public int getDamage ()
	{
		return NegativeBinomialDistribution.fromMeanAndStandardDeviation (Strength - 1, Variation) + 1;
	}

	public void attack (HexPosition enemy, int damage)
	{
		state = State.Wait;
		enemy.getUnit ().defend (damage);
		attackComplete ();
	}

	public void newTurn ()
	{
		state = State.Move;
	}

	private void defend (int damage)
	{
		hp -= damage;
		if (hp <= 0) {
			die ();
		}
	}

	public void die()
	{
		position.remove ("Unit");
		player.removeUnit (this);
		Object.Destroy (gameObject);
	}

	private Vector3 bezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t)
	{
		return (1 - t) * (1 - t) * (1 - t) * p0 + 3 * (1 - t) * (1 - t) * t * c0 + 3 * (1 - t) * t * t * c1 + t * t * t * p1;
	}

	private Vector3 dbezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t)
	{
		Vector3 dir = 3 * (1 - t) * (1 - t) * (c0 - p0) + 6 * (1 - t) * t * (c1 - c0) + 3 * t * t * (p1 - c1);
		if (dir.magnitude < 0.001) {
			return  6 * (1 - t) * (c1 - 2 * c0 + p0) + 6 * t * (p1 - 2 * c1 + c0);
		}
		return dir;
	}
	
	/*private Quaternion bezierRotation (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t) {
		Vector3 dir = dbezier (p0, c0, c1, p1, t);
		dir.y = 0;
		Quaternion rotation = new Quaternion ();
		rotation.SetLookRotation (dir);
		return rotation;
	}*/
	
	private Quaternion horizontalLookRotation (Vector3 dir)
	{
		dir.y = 0;
		Quaternion rotation = new Quaternion ();
		rotation.SetLookRotation (dir);
		return rotation;
	}

	private void fullBezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t, out Vector3 position, out Quaternion rotation)
	{
		position = bezier (p0, c0, c1, p1, t);
		rotation = horizontalLookRotation (dbezier (p0, c0, c1, p1, t));
	}

	private void moveComplete ()
	{
		if (localPlayer)
			player.moveComplete ();
	}

	private void attackComplete ()
	{
		if (localPlayer)
			player.attackComplete ();
	}

	// Use this for initialization
	void Start ()
	{
		hp = MaxHP;
		if (Coordinates == null) {
			Coordinates = new HexPosition (transform.position);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//There has to be a better way to do this. Especially if I want to stick rotations in there.
		if (moving) {
			if (path.Length < 2) {	//Shouldn't happen.
				moving = false;
				moveComplete ();
				return;
			} else if (path.Length == 2) {
				if (t >= 1) {
					transform.position = path [1];
					moving = false;
					moveComplete ();
					return;
				} else {
					transform.position = (1 - t) * path [0] + t * path [1];
					transform.rotation = horizontalLookRotation (path [1] - path [0]);
					t += MOTION_SPEED;
				}
				
			} else if (path.Length == 3) {
				if (t >= 2) {
					transform.position = path [2];
					moving = false;
					moveComplete ();
					return;
				} else {
					//transform.position = bezier (path[0], (2*path[1]+path[0])/3, (2*path[1]+path[2])/3, path[2], t/2);
					Vector3 position;
					Quaternion rotation;
					fullBezier (path [0], (2 * path [1] + path [0]) / 3, (2 * path [1] + path [2]) / 3, path [2], t / 2, out position, out rotation);
					transform.position = position;
					transform.rotation = rotation;
					t += MOTION_SPEED;
				}
			} else {
				if (n == 0) {
					if (t >= 0.5f) {
						t -= 0.5f;
						++n;	//Falls through.
					} else {
						transform.position = (1 - t) * path [0] + t * path [1];
						transform.rotation = horizontalLookRotation (path [1] - path [0]);
						t += MOTION_SPEED;
					}
				}
				if (n > 0 && n < path.Length - 1) {
					if (t >= 1) {
						t -= 1;
						++n;	//Falls through.
					} else {
						//transform.position = bezier ((path[n-1]+path[n])/2, (5*path[n]+path[n-1])/6, (5*path[n]+path[n+1])/6, (path[n+1]+path[n])/2, t);
						Vector3 position;
						Quaternion rotation;
						fullBezier ((path [n - 1] + path [n]) / 2, (5 * path [n] + path [n - 1]) / 6, (5 * path [n] + path [n + 1]) / 6, (path [n + 1] + path [n]) / 2, t, out position, out rotation);
						transform.position = position;
						transform.rotation = rotation;
						t += MOTION_SPEED;
					}
				}
				if (n == path.Length - 1) {
					if (t >= 0.5f) {
						transform.position = path [n];
						moving = false;
						moveComplete ();
						return;
					} else {
						transform.rotation = horizontalLookRotation (path [n] - path [n - 1]);
						transform.position = (0.5f - t) * path [n - 1] + (t + 0.5f) * path [n];
						t += MOTION_SPEED;
					}
				}
			}
		}
	}

	void OnGUI ()
	{	//TODO: Get rid of magic numbers.
		Vector3 coordinates = Camera.main.WorldToScreenPoint (transform.position + new Vector3 (0, 1.5f, 0) + 0.5f * Camera.main.transform.up);	//TODO: Make this some kind of constant.
		coordinates.y = Screen.height - coordinates.y;
		//print (coordinates);
		Texture2D red = new Texture2D (1, 1);
		red.SetPixel (0, 0, Color.red);
		red.wrapMode = TextureWrapMode.Repeat;
		red.Apply ();
		Texture2D green = new Texture2D (1, 1);
		green.SetPixel (0, 0, Color.green);
		green.wrapMode = TextureWrapMode.Repeat;
		green.Apply ();
		GUI.DrawTexture (new Rect (coordinates.x - hpBarWidth / 2, coordinates.y + hpBarHeight / 2, hpBarWidth, hpBarHeight), red);
		GUI.DrawTexture (new Rect (coordinates.x - hpBarWidth / 2, coordinates.y + hpBarHeight / 2, hpBarWidth * hp / MaxHP, hpBarHeight), green);
		GUIStyle centered = new GUIStyle ();
		centered.alignment = TextAnchor.MiddleCenter;
		GUI.Label (new Rect (coordinates.x - hpBarWidth / 2, coordinates.y + hpBarHeight / 2, hpBarWidth, hpBarHeight), hp.ToString (), centered);
	}
}
