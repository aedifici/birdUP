
/**************************************************************************************************
 *    Spawnpoint.cs   [birdUP]                                                            ver 1.0.1 
 *                                                                                       2018.03.29
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Spawnpoint class to label and set up spawn point positions in the scene.
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
/// Spawnpoint class to label and set up spawn point positions in the scene.
/// </summary>
public class Spawnpoint : MonoBehaviour 
{
	#region Monobehaviours

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.6f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, 0.4f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.233f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, 0.066f);
	}

	#endregion
}
