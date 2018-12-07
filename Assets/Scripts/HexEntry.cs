using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class HexEntry : System.IEquatable<HexEntry> {
	private LegacyHexPosition position;
	private string name;

	public HexEntry (LegacyHexPosition position, string name)
	{
		this.position = position;
		this.name = name;
	}

	public LegacyHexPosition Position {
		get {
			return this.position;
		}
		set {
			position = value;
		}
	}

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public override int GetHashCode () {
		return position.GetHashCode() ^ name.GetHashCode ();
	}
	
	/*public override bool Equals (object obj) {
		if (obj.GetType is HexEntry) {
			HexEntry entry = (HexEntry) obj;
			return Position == entry.Position && Name == entry.Name;
		} else {
			return false;
		}
	}*/
	
	public bool Equals (HexEntry hexEntry) {
		return position.Equals(hexEntry.Position) && name == hexEntry.Name;
	}
}
