using UnityEngine;
using System.Collections;

public interface IPlayerInterface
{
	void moveComplete ();

	void attackComplete ();

	void removeUnit (Unit unit);
}
