
/**************************************************************************************************
 *    CustomTracking.cs   [birdUP]                                                       ver 1.0.0 
 *                                                                                       2018.04.18
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Tracks an object's position each LateUpdate, like a child object, but with
 *                  optional axis restraints.
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
/// Tracks an object's position each LateUpdate, like a child object, but with optional axis 
/// restraints.
/// </summary>
public class CustomTracking : MonoBehaviour 
{
	#region Inspector Fields

	public Transform target;

	[Space(10)]
	[SerializeField]
	private bool trackX;
	
	[SerializeField]
	private bool trackY;
	
	[SerializeField]
	private bool trackZ;

	#endregion

	#region Monobehaviours

	private void LateUpdate()
	{
		if (!trackX && !trackY && !trackZ)
			return;

		Vector3 targetPos = new Vector3
		(
			trackX ? target.position.x : transform.position.x,
			trackY ? target.position.y : transform.position.y,
			trackZ ? target.position.z : transform.position.z
		);

		transform.position = targetPos;
	}

	#endregion
}
