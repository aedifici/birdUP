
/**************************************************************************************************
 *    Breadblaster.cs   [birdUP]                                                          ver 1.0.1 
 *                                                                                       2018.03.29
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  3D prefab particle emitter, from a point.
 *
 *    CHANGELOG:    
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
/// 3D prefab particle emitter, from a point.
/// </summary>
public class BreadBlaster : MonoBehaviour 
{
	#region Inspector Fields

	[Tooltip("Reference to the prefab to spawn upon emitting.")]
	public GameObject refBreadPrefab;

	#endregion

	#region Public Methods

	/// <summary>
	/// Emit an integer number of prefabs randomly, in a range, from a point.
	/// </summary>
	/// <param name="count">The number of prefab objects to spawn.</param>
	public void Blast(int count)
	{
		for (int i=0; i<count; i++)
		{
			GameObject breadTemp = Instantiate(refBreadPrefab);
			breadTemp.transform.position = transform.position;

			GameController.main.breadCount++;
		}
	}

	#endregion
}
