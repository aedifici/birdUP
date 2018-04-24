
/**************************************************************************************************
 *    Bread.cs   [birdUP]                                                                 ver 1.0.1 
 *                                                                                       2018.03.29
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Manages the behavior of an indiviual bread particle, like lifetime.
 *
 *    CHANGELOG:    ---
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
/// Manages the behavior of an indiviual bread particle, like lifetime.
/// </summary>
public class Bread : MonoBehaviour 
{
	#region Inspector Fields

	[Space(5)]
	[Tooltip("The range of velocities possible upon spawn, negative included.")]
	public Vector3 maxVelocityRange;

	[Tooltip("The amount of time after spawning before destroying itself.")]
	public float lifeTime;

	#endregion

	#region Run-time Fields 

	private Rigidbody refBody;
	private Vector3 startScale;
	private float lifeTimer = 0f;

	#endregion

	#region Monobehaviours

	private void Start()
	{
		refBody = GetComponent<Rigidbody>();

		refBody.velocity = new Vector3
		(
			Random.Range(-maxVelocityRange.x, maxVelocityRange.x),
			Random.Range(0f, maxVelocityRange.y),
			Random.Range(-maxVelocityRange.z, maxVelocityRange.z)
		);

		startScale = transform.localScale;

		transform.rotation = Random.rotation;
	}

	private void Update()
	{
		lifeTimer += Time.deltaTime;

		if (lifeTimer >= lifeTime)
		{
			Destroy(gameObject);
		}
	}

	#endregion
}
