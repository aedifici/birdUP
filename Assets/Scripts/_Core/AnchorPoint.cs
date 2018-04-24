
/**************************************************************************************************
 *    AnchorPoint.cs   [birdUP]                                                           ver 1.1.0 
 *                                                                                       2018.03.29
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  An anchorpoint component with an offset position that can be put on a post or
 *                  bird, which if set to be the active anchor point can become the focus of the
 *                  camera panner and the landing point of the active seagull.
 *
 *    CHANGELOG:    [1.1 2018.03.30]  Refactored, commented.
 *             
 *                                                ---
 *              
 *                                          LEGAL NOTICE:                
 *                                This work is licensed under the 
 *       Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License. 
 *     To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-nd/4.0/ 
 *         or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
 *
 *************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An anchorpoint component with an offset position that can be put on a post or bird, which if 
/// set to be the active anchor point can become the focus of the camera panner and the landing 
/// point of the active seagull.
/// </summary>
public class AnchorPoint : MonoBehaviour 
{
	#region Inspector Fields

	[Tooltip("The list of anchor points on this object. When polled, a random one is selected.")]
	public Vector3[] anchorPoints;

	[Space(5)][Header("Gizmo Settings")][Space(3)]
	[Tooltip("The size of the gizmo circle.")]
	public float gizmoSize = 0.3f;

	#endregion

	#region Runtime Fields

	private Vector3 activeAnchor;

	#endregion

	#region Monobehaviours

	private void Start()
	{
		int roll = Random.Range(0, anchorPoints.Length);
		activeAnchor = anchorPoints[roll];
	}

	private void OnDrawGizmos()
	{
		if (anchorPoints.Length < 1)
			return;

		foreach(Vector3 anchor in anchorPoints)
		{ 
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position + anchor, gizmoSize);

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position + anchor, 0.05f);
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Returns the position of the anchorpoint, which is the position of the parent 
	/// object with the anchorpoint offset added to it.
	/// </summary>
	/// <returns>The position of the anchor point in world space.</returns>
	public Vector3 GetAnchorPos()
	{
		return transform.position + activeAnchor;
	}

	#endregion
}
