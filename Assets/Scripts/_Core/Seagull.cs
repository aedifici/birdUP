
/**************************************************************************************************
 *    Seagull.cs   [birdUP]                                                               ver 1.2.0 
 *                                                                                       2018.04.03
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Handles the automatic behavior of a seagull that's spawned. Also handles the 
 *                  setting of an active seagull and interacts with GameController for player
 *                  scoring alone activeGull progress.
 *
 *    CHANGELOG:    [1.2 2018.04.03]  Late removed, in-air fail added.
 *                  [1.1 2018.03.28]  Partcile emission on landing.
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
/// Handles the automatic behavior of a seagull that's spawned. Also handles the setting of an 
/// active seagull and interacts with GameController for player scoring alone activeGull progress.
/// </summary>
public class Seagull : MonoBehaviour 
{
	#region Inspector Fields

	[Header("Settings:")][Space(3)]
	[Tooltip("The speed at which the seagull rotates towards the target anchor.")]
	public float rotateSpeed;

	[Tooltip("The maximum distance from the post considered a successful tap.")]
	public float maxDistance;

	[Tooltip("The maximum number of particles that can be spawned by a landing.")]
	public int maxParticles;

	[Space(10)][Header("Gizmo Settings")][Space(3)]
	public float attachGizmoSize;

	#endregion

	#region Run-time Fields

	[HideInInspector]
	/// <summary>
	/// If true, the gull has reached the post.
	/// </summary>
	public bool isAtPost = false;

	private BreadBlaster refBreadBlaster;
	private Rigidbody refBody;
	private float speed;
	private float score;
	private float distanceToPost;
	private float failTimer;
	private bool isFallen;
	private bool isLanded;

	#endregion

	#region Monobehaviours

	private void Start()
	{
		speed           = GameController.main.seagullBaseSpeed;
		refBody         = GetComponent<Rigidbody>();
		refBreadBlaster = GetComponentInChildren<BreadBlaster>();

		refBody.isKinematic = true;

		score = -1f;
	}

	private void Update()
	{
		if (isFallen || isLanded)
		{
			return;
		}

		if (isAtPost)
		{
			failTimer -= Time.deltaTime;
			if (failTimer <= 0)
				Fall();
			return;
		}

		MoveTowardsAnchor();

		CheckDistanceToPost();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attachGizmoSize);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, 0.05f);
	}

	#endregion

	#region Private Methods

	private void MoveTowardsAnchor()
	{
		if (GameController.main.activeAnchor == null)
			return;

		transform.position = Vector3.MoveTowards
		(
			transform.position,
			GameController.main.activeAnchor.GetAnchorPos(),
			speed * Time.deltaTime
		);
	}

	private void CheckDistanceToPost()
	{
		distanceToPost = Vector3.Distance
		(
			transform.position,
			GameController.main.activeAnchor.GetAnchorPos()
		);

		if (distanceToPost <= 0)
		{
			ReachPost();
		}
		else if (distanceToPost > 0.2f)
		{
			LookAtTarget();
		}
	}

	private void LookAtTarget()
	{
		Vector3 lookPos = GameController.main.activeAnchor.transform.position - transform.position;
		lookPos.y = 0;

		Quaternion rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
	}

	private void ReachPost()
	{
		isAtPost = true;
		GameController.main.lastArrivalTime = Time.time;

		if (score > 0f)
		{
			Celebrate();
		}
		else
		{
			CameraController.main.ShakeScreen(Vector2.left * CameraController.main.failShakeMagnitude, 0.08f);
			CameraController.main.ShakeScreen(Vector2.right * CameraController.main.failShakeMagnitude * 0.75f, 0.06f);
			CameraController.main.ShakeScreen(Vector2.left * CameraController.main.failShakeMagnitude * 0.5f, 0.02f);
			CameraController.main.ShakeScreen(Vector2.right * CameraController.main.failShakeMagnitude * 0.25f, 0.01f);

			Disengage();

			Fall();

			refBody.AddForce(transform.forward * 5f, ForceMode.Impulse);
			refBody.AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.Impulse);
		}
	}

	private void Celebrate()
	{
		isLanded = true;

		float breadCount = Mathf.Lerp(0f, maxParticles, score);

		CameraController.main.ShakeScreen(CameraController.main.defaultMagnitude * (score * 1.5f));
		CameraController.main.ShakeScreen(-CameraController.main.defaultMagnitude * 0.5f * (score * 1.5f));

		refBreadBlaster.Blast((int)breadCount);

		Disengage();

		GameController.main.activeAnchor = GetComponent<AnchorPoint>();
		GameController.main.stackCount++;
	}

	#endregion

	#region Public Methods

	public void Tap()
	{
		if (score >= 0f)
			return;

		GameController.main.activeGull = null;

		float scoreRatio = Mathf.Clamp(1f - (distanceToPost / maxDistance), 0f, 1f);

		if (scoreRatio <= 0f)
		{
			CameraController.main.ShakeScreen(Vector2.left  * CameraController.main.failShakeMagnitude,         0.08f);
			CameraController.main.ShakeScreen(Vector2.right * CameraController.main.failShakeMagnitude * 0.75f, 0.06f);
			CameraController.main.ShakeScreen(Vector2.left  * CameraController.main.failShakeMagnitude * 0.5f,  0.02f);
			CameraController.main.ShakeScreen(Vector2.right * CameraController.main.failShakeMagnitude * 0.25f, 0.01f);

			Fall();

			refBody.AddForce(transform.forward * 5f, ForceMode.Impulse);
			refBody.AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.Impulse);
		}

		GameController.main.AddToInstability(scoreRatio);

		SetScore(scoreRatio);
	}

	public void SetScore(float score)
	{
		this.score = score;

		GameController.main.activeGull = null;

		if (isAtPost)
			Celebrate();
	}

	public void Fall()
	{
		isFallen = true;
		refBody.isKinematic = false;
		GameController.main.activeGull = null;

		Disengage();
	}

	public void Disengage()
	{
		GameController.main.activeGull = null;
		GameController.main.hasActiveGull = false;
	}

	#endregion
}
