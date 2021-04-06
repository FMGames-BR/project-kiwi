using System.Collections.Generic;
using UnityEngine;
public class Trajectory : MonoBehaviour
{
	static float dotsTimeStamp;
	public static Vector3[] OnUpdateTrajectory(Vector3 playerPosition, Vector3 movementForce, int totalPoints, float spacing = 1f)
	{
		Vector3 pos = Vector3.zero;
		dotsTimeStamp = spacing;
		List<Vector3> points = new List<Vector3>();
		for (int i = 0; i < totalPoints; i++)
		{
			pos.x = (playerPosition.x + movementForce.x * dotsTimeStamp);
			pos.y = (playerPosition.y + movementForce.y * dotsTimeStamp) - (Physics.gravity.magnitude * dotsTimeStamp * dotsTimeStamp) / 2f;
			pos.z = (playerPosition.z + movementForce.z * dotsTimeStamp);
			if (i > 0)
			{
				Vector3 hitPoint = HitPosition(points[points.Count - 1], pos);
				if (hitPoint != Vector3.zero)
				{
					points.Add(hitPoint);
					break;
				}
			}
			points.Add(pos);
			dotsTimeStamp += spacing;
		}

		return points.ToArray();
	}
	private static Vector3 HitPosition(Vector3 lastPosition, Vector3 currentPosition)
	{
		RaycastHit hitInfo;
		if (Physics.Linecast(lastPosition, currentPosition, out hitInfo))
		{
			//Debug.DrawLine(lastPosition, currentPosition);
			//Debug.DrawLine(lastPosition, hitInfo.point, Color.red);
			return hitInfo.point;
		}
		return Vector3.zero; //hit nothing
	}
}