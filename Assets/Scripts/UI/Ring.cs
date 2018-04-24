
/**************************************************************************************************
 *    Ring.cs   [birdUP]                                                                  ver 1.2.1 
 *                                                                                       2018.03.29
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Handles the behavior of the timing ring UI elements, including tracking,
 *                  alpha, and size along the entire action period from instantiation to 
 *                  completion from tap or miss.
 *
 *    CHANGELOG:    [1.2 2018.03.28]  Refactored, commented.
 *                  [1.1 2018.03.27]  Late behavior method and routine added.
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
using UnityEngine.UI;

/// <summary>
/// Handles the behavior of the timing ring UI elements, including tracking, alpha, and size along 
/// the entire action period from instantiation to completion from tap or miss.
/// </summary>
public class Ring : MonoBehaviour
{
	#region Inspector Fields

	[Header("Dependancies")]
	[Space(3)]

	[Tooltip("A reference to the target ring (smaller, inner ring) object's image component.")]
	public Image refTargetRing;

	[SerializeField]
	[Tooltip("A reference to the skinnier ring sprite, to be used by the outer collpasing ring.")]
	private Sprite refSpriteThin;

	[SerializeField]
	[Tooltip("A reference to the thicker ring sprite, to be used by the inner target ring.")]
	private Sprite refSpriteThick;

	#endregion

	#region Run-time Fields

	[HideInInspector]
	public float progress;

	private Image refRing;               // Reference to the base ring image. Grabbed at start.
	private Coroutine latePhaseRoutine;  // Stores the late routine.
	private Vector3 savedStartSize;      // The saved max localScale of the ring, at start. 
	private float savedStartDistance;    // The saved distance from the target, at initialize.

	// States.
	private bool isActive;               // If true, ring is enabled and tracking progress.
	private bool isLate;                 // If true, the player is late to tap. Time-tracking.

	#endregion

	#region Monobehaviours

	private void Start()
	{
		refRing = GetComponent<Image>();

		savedStartSize = transform.localScale;
		refRing.enabled = false;
		refTargetRing.enabled = false;
		isActive = false;
	}

	private void LateUpdate()
	{
		if (!isActive || isLate)
		{
			return;
		}

		if (GameController.main.activeGull == null)
		{
			Disengage();
			return;
		}

		UpdateProgress();

		UpdateSize();

		UpdateAlpha();
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Finds the percentage of progress from initialize to target.
	/// </summary>
	private void UpdateProgress()
	{
		progress = Mathf.Clamp
		(
			Vector3.Distance
			(
				GameController.main.activeGull.transform.position,
				GameController.main.activeAnchor.GetAnchorPos()
			) / savedStartDistance,
			0f,
			1f
		);
	}

	/// <summary>
	/// Using the updated progress, shrinks the size of the ring as it approaches.
	/// </summary>
	private void UpdateSize()
	{
		transform.localScale = Vector3.Lerp
		(
			Vector3.one * 54f * GameController.main.targetRingRatio,
			savedStartSize,
			progress
		);
	}

	/// <summary>
	/// Using the updated progress, increases the alpha of the outer ring as it approaches.
	/// </summary>
	private void UpdateAlpha()
	{
		refRing.color = new Color
		(
			refRing.color.r,
			refRing.color.g,
			refRing.color.b,
			1f - progress
		);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Initializes and begins a new ring tracking beahviour on the active gull.
	/// </summary>
	public void Initialize()
	{
		isActive = true;
		isLate = false;
		refRing.enabled = true;
		refTargetRing.enabled = true;

		refRing.sprite = refSpriteThin;

		transform.localScale = savedStartSize;
		refTargetRing.transform.localScale = savedStartSize * GameController.main.targetRingRatio;

		savedStartDistance = Vector3.Distance
		(
			GameController.main.activeGull.transform.position,
			GameController.main.activeAnchor.GetAnchorPos()
		);

		// Update to initial size and alpha to prevent incorrect startup frame.
		UpdateSize();
		UpdateAlpha();
	}

	/// <summary>
	/// Disengages the ring, hding its elements and waiting for a new initialization.
	/// </summary>
	public void Disengage()
	{
		isActive = false;
		isLate = false;
		refRing.enabled = false;
		refTargetRing.enabled = false;
	}

	/// <summary>
	/// Transitions the behaviour to the late phase, starting the late phase coroutine which shrinks the 
	/// ring over time through the late input window.
	/// </summary>
	/// <param name="lateWindow">The maximum time to spend in the late phase.</param>
	public void BeginLatePhase(float lateWindow)
	{
		refRing.sprite = refSpriteThick;

		if (latePhaseRoutine != null)
		{
			StopCoroutine(latePhaseRoutine);
		}

		latePhaseRoutine = StartCoroutine(LatePhaseRoutine(lateWindow));
	}

	#endregion

	#region Routines

	/// <summary>
	/// Handles late phase (after landing, still no tap) behavior over the late tap window.
	/// </summary>
	/// <param name="lateWindow">The duration of the late tap window.</param>
	private IEnumerator LatePhaseRoutine(float lateWindowDuration)
	{
		isLate = true;

		Vector3 startScale = transform.localScale;

		// Linear shrink to 0 over duration.
		float startTime = Time.time;
		while ((Time.time - startTime) < lateWindowDuration)
		{
			float t = (Time.time - startTime) / lateWindowDuration;

			transform.localScale = Vector3.Lerp
			(
				startScale,
				Vector3.zero,
				t
			);

			yield return 1;
		}
		refTargetRing.transform.localScale = Vector3.zero;

		isLate = false;

		Disengage();
	}

	#endregion
}