using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexPosition : System.IEquatable<HexPosition>
{
	private int u;
	private int v;
	public const int SIZE = 1;
	private const float SQRT32 = 0.86602540378443864676372317075294f;
	//sqrt(3)/2
	private static GameObject marker;

	private static Dictionary<HexEntry, object> dictionary = new Dictionary<HexEntry, object> ();
	private static Dictionary<string, Pair<Color, int>> selectionTypes = new Dictionary<string, Pair<Color, int>> ();
	//Carries the color and layer of each kind of hex selector.
	//private static Dictionary<string, HashSet<HexPosition>> selectedCells = new Dictionary<string, List<HexPosition>>();
	private static Dictionary<HexPosition, HashSet<string>>	selectedTypes = new Dictionary<HexPosition, HashSet<string>> ();
	private static Dictionary<HexPosition, string> currentSelection = new Dictionary<HexPosition, string> ();
	private static Dictionary<HexPosition, GameObject> markers = new Dictionary<HexPosition, GameObject> ();
	//I just made this public for debugging reasons.

	public static int[] pathToIntString (HexPosition[] path)
	{
		int[] intString = new int[path.Length * 2];
		for (int i = 0; i < path.Length; ++i) {
			intString [2 * i] = path [i].u;
			intString [2 * i + 1] = path [i].v;
		}
		return intString;
	}

	public static HexPosition[] intStringToPath (int[] intString)
	{
		HexPosition[] path = new HexPosition[intString.Length / 2];
		for (int i = 0; i < path.Length; ++i) {
			path [i] = new HexPosition (intString [2 * i], intString [2 * i + 1]);
		}
		return path;
	}

	public static int[] hexToIntPair (HexPosition hex)
	{
		return new int[2] { hex.u, hex.v };
	}

	public static HexPosition intPairToHex (int[] pair)
	{
		return new HexPosition (pair [0], pair [1]);
	}

	private class Pair<S, T>
	{
		private S first;
		private T second;

		public Pair (S first, T second)
		{
			this.first = first;
			this.second = second;
		}

		public S First { get { return this.first; } }

		public T Second { get { return this.second; } }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HexPosition"/> class using the coordinates. This method is not very user-friendly.
	/// </summary>
	/// <param name="u">Distance north of the origin.</param>
	/// <param name="v">Distance south-east of the origin.</param>
	public HexPosition (int u, int v)
	{
		this.u = u;
		this.v = v;
	}

	/// <summary>
	/// Given x and y position, finds the coordinates of the hex they're on.
	/// </summary>
	/// <param name="position">Position.</param>
	public HexPosition (Vector3 position)
	{
		float yy = 1 / SQRT32 * position.z / SIZE + 1;
		float xx = position.x / SIZE + yy / 2 + 0.5f;
		u = Mathf.FloorToInt ((Mathf.Floor (xx) + Mathf.Floor (yy)) / 3);
		v = Mathf.FloorToInt ((xx - yy + u + 1) / 2);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HexPosition"/> class at the origin.
	/// </summary>
	public HexPosition ()
	{
		u = 0;
		v = 0;
	}

	public int U {
		get {
			return this.u;
		}
		set {
			u = value;
		}
	}

	public int V {
		get {
			return this.v;
		}
		set {
			v = value;
		}
	}

	/// <summary>
	/// Gets or sets the object used to select cells.
	/// </summary>
	/// <value>The object used to select cells.</value>
	public static GameObject Marker {
		get {
			return marker;
		}
		set {
			marker = value;
		}
	}

	//The adjacent hexes. It's not obvious how to find the neighbors based on coordinates, so we have this.
	public HexPosition N {	get { return new HexPosition (u + 1,	v); } }

	public HexPosition NE {	get { return new HexPosition (u + 1,	v + 1); } }

	public HexPosition SE {	get { return new HexPosition (u, v + 1); } }

	public HexPosition S {	get { return new HexPosition (u - 1,	v); } }

	public HexPosition SW {	get { return new HexPosition (u - 1,	v - 1); } }

	public HexPosition NW {	get { return new HexPosition (u, v - 1); } }

	public HexPosition[] Neighbors { get { return new HexPosition[6] {
			this.N,
			this.NE,
			this.SE,
			this.S,
			this.SW,
			this.NW
		}; } }

	//Gives a hex n in a given direction. You can get to any hex in two steps.
	//This will be more understandable than trying to give coordinates.
	public HexPosition goN (int n)
	{
		return new HexPosition (u + n, v);
	}

	public HexPosition goNE (int ne)
	{
		return new HexPosition (u + ne,	v + ne);
	}

	public HexPosition goSE (int se)
	{
		return new HexPosition (u, v + se);
	}

	public HexPosition goS (int s)
	{
		return new HexPosition (u - s, v);
	}

	public HexPosition goSW (int sw)
	{
		return new HexPosition (u - sw,	v - sw);
	}

	public HexPosition goNW (int nw)
	{
		return new HexPosition (u, v - nw);
	}

	private static int max (int a, int b)
	{
		if (a > b) {
			return a;
		} else {
			return b;
		}
	}

	private static int abs (int n)
	{
		if (n > 0) {
			return n;
		} else {
			return -n;
		}
	}

	/// <summary>
	/// Length of the minimum path from this position to p. The distance from a hex to itself is zero, the distance to a neighbor is one, etc.
	/// </summary>
	/// <param name="p">Position to find the distance to.</param>
	public int dist (HexPosition p)
	{
		int du = p.u - u;
		int dv = p.v - v;
		return max (max (abs (du), abs (dv)), abs (du - dv));
	}

	//Noted that these coordinates are in between the ones for adjacent hexes.
	//This means for example that NE is closer to north, as opposed to closer to east.
	//If you wanted distance north, the best you could average NE and NW. This gives half integers.
	//Note that distance is the maximum of these.
	public int distNE (HexPosition p)
	{
		return p.u - u;
	}

	public int distE (HexPosition p)
	{
		return p.u - v;
	}

	public int distSE (HexPosition p)
	{
		return distE (p) - distNE (p);
	}

	public int distSW (HexPosition p)
	{
		return -distNE (p);
	}

	public int distW (HexPosition p)
	{
		return -distE (p);
	}

	public int distNW (HexPosition p)
	{
		return -distSE (p);
	}

	/// <summary>
	/// Returns the center if this hex.
	/// </summary>
	/// <returns>The center of this hex.</returns>
	public Vector3 getPosition ()
	{
		float x = SIZE * v * 1.5f;
		float y = SIZE * (2 * u - v) * SQRT32;
		return new Vector3 (x, 0, y);
	}

	/// <summary>
	/// Add the key value pair to the dictionary corresponding to this cell.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <param name="key">The name of the variable.</param>
	/// <param name="value">The value of the varaible.</param>
	public void add (string key, object value)
	{
		HexEntry hexKey = new HexEntry (this, key);
		dictionary [hexKey] = value;
	}

	/// <summary>
	/// Tries to get the value from the dictionary corresponding to this cell. Returns true and assignes the value to the "value" parameter if successful. Returns false and does not modify the "value" parameter if unsuccessful.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <returns><c>true</c>, if the dictionary contained the key, <c>false</c> otherwise.</returns>
	/// <param name="key">The name of the variable searched for.</param>
	/// <param name="value">The variable in which to place the value of the variable searched for.</param>
	public bool tryGetValue (string key, out object value)
	{
		return dictionary.TryGetValue (new HexEntry (this, key), out value);
	}

	/// <summary>
	/// Tries to get the value from the dictionary corresponding to this cell. Returns null if it isn't found. This method cannot distinguish between a value of null and the lack of a corresponding entry, so don't use it in cases where the value might be null.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <returns>The value of the varaible.</returns>
	/// <param name="key">The name of the variable.</param>
	public object getValue (string key)
	{
		object value = new object ();
		if (dictionary.TryGetValue (new HexEntry (this, key), out value)) {
			return value;
		} else {
			return null;
		}
	}

	/// <summary>
	/// Clears all instances of the given key from the dictionary.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <param name="key">Key to clear from the dictionary.</param>
	public static void clear (string key)
	{
		foreach (KeyValuePair<HexEntry, object> kvp in dictionary) {
			if (kvp.Key.Name == key) {
				dictionary.Remove (kvp.Key);
			}
		}
	}

	/// <summary>
	/// Clears the dictionary corresponding to this entry.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	public void clear ()
	{
		foreach (KeyValuePair<HexEntry, object> kvp in dictionary) {
			if (kvp.Key.Position == this) {
				dictionary.Remove (kvp.Key);
			}
		}
	}

	/// <summary>
	/// Purges the specified key from the dictionary.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <param name="key">The name of the variable to be purged.</param>
	public bool remove (string key)
	{
		return dictionary.Remove (new HexEntry (this, key));
	}

	/// <summary>
	/// Assigns the given key to the dictionary with a null value. This is useful as something like a boolean value. In this case, all that matters is whether or not the key is present.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <param name="key">Name of the variable to assign a null value to.</param>
	public void flag (string key)
	{
		add (key, null);
	}

	/// <summary>
	/// Checks to see if the dictionary contains this key. This is especially useful in conjunction with <see cref="flag (string key)"/>, but it can also be used for non-boolean entries.
	/// This and other methods allow you to make the hex hold whatever variables you need. This is set to the position, not the specific object, and can be read by a new HexPosition with the same coordinates.
	/// </summary>
	/// <returns><c>true</c>, if the dictionary contains the key, <c>false</c> otherwise.</returns>
	/// <param name="key">The name of the variable to check the dictionary for.</param>
	public bool containsKey (string key)
	{
		return dictionary.ContainsKey (new HexEntry (this, key));
	}
	
	//Returns false if the set already contains that value.
	private bool addToSetInDictionary<K,E> (Dictionary<K,HashSet<E>> dictionary, K key, E entry)
	{
		HashSet<E> set = null;
		if (dictionary.TryGetValue (key, out set)) {
			if (set.Contains (entry)) {
				return false;
			}
			set.Add (entry);
			return true;
		} else {
			set = new HashSet<E> ();
			set.Add (entry);
			dictionary.Add (key, set);
			return true;
		}
	}
	
	//Returns true if there's anything left.
	private bool removeFromSetInDictionary<K,E> (Dictionary<K,HashSet<E>> dictionary, K key, E entry)
	{
		HashSet<E> set = null;
		if (!dictionary.TryGetValue (key, out set)) {
			return false;
		}
		set.Remove (entry);
		if (set.Count == 0) {
			dictionary.Remove (key);
			return false;
		}
		return true;
	}

	/// <summary>
	/// Returns the name of the type of selection for this position, or null if it's not selected. If the cell is selected in more than one way, it returns the name for the currently visible selection marker.
	/// </summary>
	/// <value>The name of the type of selection for this position, or null if it's not selected.</value>
	public string SelectedType {
		get {
			string type = null;
			currentSelection.TryGetValue (this, out type);
			return type;
		}
		//Sets currentSelection[this] and updates the physical marker. Does nothing with the list of selections.	
		private set {
			//MonoBehaviour.print(value);
			GameObject thisMarker = null;
			if (value == null) {
				if (markers.TryGetValue (this, out thisMarker)) {
					Object.Destroy (thisMarker);
					markers.Remove (this);
					currentSelection.Remove (this);
					return;
				} else {
					return;
				}
			}
		
			Pair<Color, int> colorAndLayer = null;
			if (!selectionTypes.TryGetValue (value, out colorAndLayer)) {
				return;	//That type does not exist.
			}
			
			currentSelection [this] = value;
			Color color = colorAndLayer.First;
			int layer = colorAndLayer.Second;
			if (markers.TryGetValue (this, out thisMarker)) {
				thisMarker.GetComponent<Renderer> ().material.color = color;
				thisMarker.transform.position = getPosition () + new Vector3 (0f, 0.01f * layer, 0f);
			} else {
				thisMarker = (GameObject)Object.Instantiate (marker, getPosition () + new Vector3 (0f, 0.01f * layer, 0f), Quaternion.identity);
				thisMarker.GetComponent<Renderer> ().material.color = color;
				markers [this] = thisMarker;
			}
		}
	}

	/// <summary>
	/// Returns the color of the type of selection for this position, or Color.clear if it's not selected. If the cell is selected in more than one way, it returns the color for the currently visible selection marker.
	/// </summary>
	/// <value>The color of the type of selection for this position, or Color.clear if it's not selected.</value>
	public Color SelectedColor {
		get {
			string type = SelectedType;
			if (type == null) {
				return Color.clear;
			}
			Pair<Color, int> colorAndLayer = null;
			if (selectionTypes.TryGetValue (type, out colorAndLayer)) {
				return colorAndLayer.First;
			} else {
				return Color.clear;
			}
		}
	}

	/// <summary>
	/// Returns the layer of the type of selection for this position, or int.MinValue if it's not selected. If the cell is selected in more than one way, it returns the layer for the currently visible selection marker.
	/// </summary>
	/// <value>The layer of the type of selection for this position, or int.MinValue if it's not selected.</value>
	public int SelectedLayer {
		get {
			string type = SelectedType;
			if (type == null) {
				return int.MinValue;
			}
			Pair<Color, int> colorAndLayer = null;
			if (selectionTypes.TryGetValue (type, out colorAndLayer)) {
				return colorAndLayer.Second;
			} else {
				return int.MinValue;
			}
		}
	}

	/// <summary>
	/// Selects the hex using the given type of selection. If the hex is already selected, it stays selected the other way, and only the highest layer is visible. If that kind of selection has not been defined, the operation fails and returns false. If this hex is already selected in this manner, this method does nothing and returns true.
	/// </summary>
	/// <returns><c>true</c>, if that type has been defined, <c>false</c> otherwise, indicating that the operation has failed.</returns>
	/// <param name="type">The type of selection to use.</param>
	public bool select (string type)
	{
		Pair<Color, int> colorAndLayer = null;
		if (!selectionTypes.TryGetValue (type, out colorAndLayer)) {
			return false;		//Error: you never defined that type.
		}
		if (!addToSetInDictionary (selectedTypes, this, type)) {
			return true;
		}
		if (SelectedLayer < colorAndLayer.Second) {
			SelectedType = type;
		}
		return true;
	}

	/// <summary>
	/// Selects the hexes using the given type of selection. If the hex are already selected, it stays selected the other way, and only the highest layer is visible. If that kind of selection has not been defined, the operation fails and returns false. If some or all of these hexes are already selected in this manner, they are unaffected.
	/// </summary>
	/// <returns><c>true</c>, if that type has been defined, <c>false</c> otherwise, indicating that the operation has failed.</returns>
	/// <param name="type">The type of selection to use.</param>
	/// <param name="set">The hexes to select.</param>
	public static void select (string type, IEnumerable<HexPosition> set)
	{
		foreach (HexPosition position in set) {
			position.select (type);
		}
	}

	/// <summary>
	/// Unselect this hex in the given manner. Does nothing if the hex is not already selected in that manner.
	/// </summary>
	/// <param name="type">The type of selection to remove.</param>
	public void unselect (string type)
	{
		removeFromSetInDictionary (selectedTypes, this, type);
		if (SelectedType == type) {
			if (!selectedTypes.ContainsKey (this)) {
				SelectedType = null;
				return;
			}
			string top = null;
			int layer = int.MinValue;
			foreach (string newType in selectedTypes[this]) {
				int newLayer = selectionTypes [newType].Second;
				if (newLayer > layer) {
					layer = newLayer;
					top = newType;
				}
			}
			SelectedType = top;
		}
	}

	/// <summary>
	/// Clears all selections of any type.
	/// </summary>
	public static void clearSelection ()
	{
		selectedTypes.Clear ();
		currentSelection.Clear ();
		foreach (GameObject marker in markers.Values) {
			Object.Destroy (marker);
		}
		markers.Clear ();
	}
	
	//This could be optimized.
	/// <summary>
	/// Clears all selections of the specified type.
	/// </summary>
	/// <param name="type">Type of selection to remove.</param>
	public static void clearSelection (string type)
	{
		foreach (HexPosition hex in getSelection(type)) {
			hex.unselect (type);
		}
	}

	/// <summary>
	/// Gets a set of all positions with a given type of selection.
	/// </summary>
	/// <returns>A set containing all of the positions with that type of selection.</returns>
	/// <param name="type">The type of selection to look for.</param>
	public static HashSet<HexPosition> getSelection (string type)
	{
		HashSet<HexPosition> selection = new HashSet<HexPosition> ();
		foreach (KeyValuePair<HexPosition, HashSet<string>> kvp in selectedTypes) {
			if (kvp.Value.Contains (type)) {
				selection.Add (kvp.Key);
			}
		}
		return selection;
	}

	/// <summary>
	/// Returns a single hex selected in the given way, or null if there are no such hexes. It is recommended to use this in cases where there is only a single hex selected in that manner.
	/// </summary>
	/// <returns>A single hex selected in the given way, or null if there are no such hexes.</returns>
	/// <param name="type">Type of selection to search for.</param>
	public static HexPosition getOne (string type)
	{
		foreach (KeyValuePair<HexPosition, HashSet<string>> kvp in selectedTypes) {
			if (kvp.Value.Contains (type)) {
				return kvp.Key;
			}
		}
		return null;
	}

	/// <summary>
	/// Returns <c>true</c> if this hex is selected in the given manner, <c>false</c> otherwise.
	/// </summary>
	/// <returns><c>true</c> if this hex is selected in the given manner, <c>false</c> otherwise.</returns>
	/// <param name="type">Type of selection to check for.</param>
	public bool isSelected (string type)
	{
		HashSet<string> value = null;
		if (selectedTypes.TryGetValue (this, out value)) {
			return value.Contains (type);
		} else {
			return false;
		}
	}

	/// <summary>
	/// Sets the color and layer for each type of selection. Layer should be unique.
	/// </summary>
	/// <param name="name">Name of the selection type.</param>
	/// <param name="color">Color of the selection type.</param>
	/// <param name="layer">Layer of the selection type.</param>
	public static void setColor (string name, Color color, int layer)
	{
		selectionTypes.Add (name, new Pair<Color, int> (color, layer));
	}

	/// <summary>
	/// Gets the unit at the specified position.
	/// </summary>
	/// <returns>The unit at the specified position.</returns>
	/// <param name="position">Position to return the unit on.</param>
	public Unit getUnit ()
	{
		return (Unit)getValue ("Unit");
	}

	public override string ToString ()
	{
		return string.Format ("(u={0},\tv={1})", u, v);
	}

	/*public override bool Equals (object obj) {
		if (obj is HexPosition) {
			HexPosition pos = (HexPosition) obj;
			return u == pos.u && v == pos.v;
		} else {
			return false;
		}
	}*/

	public bool Equals (HexPosition obj)
	{
		return u == obj.U && v == obj.V;
	}

	public override int GetHashCode ()
	{
		return ((u << 16) | (v & 0xFFFF)) * 0x55555555;	//The last half of u concatenated with the last half of v, then multiplied by some random number
	}
}
