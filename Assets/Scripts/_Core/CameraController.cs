
/**************************************************************************************************
 *    CameraController.cs   [birdUP]                                                      ver 1.4.0 
 *                                                                                       2018.04.18
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Controls the camera in order to maintain a tracking focus on incomming active
 *                  seagulls and the active anchor point using pan and zoom. 
 *
 *    NOTE:         For initial PC build.
 *
 *    CHANGELOG:    [1.4 2018.04.18]  Dynamic vertical offset.
 *                  [1.3 2018.03.29]  Refactored, commented.
 *                  [1.2 2018.03.28]  Changed to add seagull focus at halfway point along path.
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

/// <summary>
/// Controls the camera in order to maintain a tracking focus on incomming active seagulls and the 
/// active anchor point using pan and zoom.
/// </summary>
public class CameraController : MonoBehaviour 
{
	#region Statics & Classes

	/// <summary>
	/// Public static reference to the single active CameraController in the current scene.
	/// </summary>
	public static CameraController main;

	/// <summary>
	/// Holds the information of a single shake action.
	/// </summary>
	private class Shake
	{
		public Vector2 magnitude;
		public float duration;

		public Shake(Vector2 magnitude, float duration)
		{
			this.magnitude = magnitude;
			this.duration = duration;
		}
	}

	#endregion

	#region Inspector Fields

	[Space(10)][Header("Pan")][Space(3)]
	
	[SerializeField]
	[Tooltip("Multiplied against distance to determine a frame's move step. Smaller = slower to " +
		"catch up to target.")]
	private float moveSmoothing;

	[SerializeField]
	[Tooltip("Ensures a minimum step to allow tracking to more quickly reach final position. " +
		"Smaller = more time to reach stop upon reaching target.")]
	private float moveMinStep;

	[SerializeField]
	[Tooltip("The ratio of smoothing and minStep movement to have when moving towards an anchor " +
		"instead of a gull.")]
	private float trackAnchorRatio;

	[Space(5)]
	[SerializeField]
	[Tooltip("The vertical offset from anchor focus point at the start of the game.")]
	private float startOffsetY;

	[SerializeField]
	[Tooltip("The vertical offset from anchor focus point after reaching the target stack number.")]
	private float endOffsetY;

	[SerializeField]
	[Tooltip("The stack height required to reach the maximum vertical offset.")]
	private int offsetYTargetStack;

	[Space(10)][Header("Zoom")][Space(3)]

	[SerializeField]
	[Tooltip("Multiplied against distance to determine a frame's zoom step. Smaller = slower to " +
		"catch up to target.")]
	private float zoomSmoothing;

	[SerializeField]
	[Tooltip("Ensures a minimum step to allow tracking to more quickly reach final zoom. " +
	"Smaller = more time to reach stop upon reaching target.")]
	private float zoomMinStep;

	[SerializeField]
	[Tooltip("The farthest that the camera can zoom out.")]
	private float zoomMax;

	[SerializeField]
	[Tooltip("The farthest that the camera can zoom in.")]
	private float zoomMin;

	[Space(10)][Header("Screen Shake")][Space(3)]

	[Tooltip("The base magnitude of a default screen shake.")]
	public Vector2 defaultMagnitude;

	[SerializeField]
	[Tooltip("The typical amount of shakes in a default shake.")]
	private int defaultCount;

	[SerializeField]
	[Tooltip("The duration of a full shake action in a default shake.")]
	private float defaultDuration;

	[Tooltip("The magnitude of a fail shake (horizontal, 4 shakes reducing).")]
	public float failShakeMagnitude;

	[Space(10)][Header("Gizmo")][Space(3)]
	[SerializeField]
	private AnchorPoint refAnchor;

	[SerializeField]
	private Color gizmoColorScreen;
	
	[SerializeField]
	private Color gizmoColorOffsetStart;

	[SerializeField]
	private Color gizmoColorOffsetEnd;

	[SerializeField]
	private Vector2 screenDisplayScale;

	#endregion

	#region Run-time Fields

	private List<Shake> shakeQueue;
	private Coroutine activeShakeRoutine;
	private Vector2 shakeOffset;
	private Vector3 targetTrack;
	private Vector3 targetAnchor;
	private float targetZoom;
	private float targetVerticalOffset;
	private float activeVerticalOffset;
	private bool isShaking;
	private bool isTrackingGull;

	#endregion

	#region Monobehaviours

	private void Awake()
	{
		main = this;
		shakeQueue = new List<Shake>();
		targetTrack = transform.position;
	}

	private void LateUpdate()
	{
		UpdateShake();

		UpdateVerticalOffset();

		UpdateTrackAndZoom();

		Move();
	}

	private void OnDrawGizmos()
	{
		if (refAnchor == null)
			return;

		Vector3 focusPos = refAnchor.transform.position + refAnchor.anchorPoints[0];

		DrawGizmoRect(gizmoColorOffsetStart, focusPos - Vector3.up * startOffsetY);

		DrawGizmoRect(gizmoColorOffsetEnd, focusPos - Vector3.up * endOffsetY);

		DrawGizmoRect(gizmoColorScreen, focusPos);
	}

	#endregion

	#region Private Methods

	private void DrawGizmoRect(Color color, Vector3 centerPos)
	{
		Gizmos.color = color;
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
		Gizmos.DrawLine
		(
			new Vector3
			(
				centerPos.x - (screenDisplayScale.x / 2),
				centerPos.y + (screenDisplayScale.y / 2),
				centerPos.z
			),
			new Vector3
			(
				centerPos.x + (screenDisplayScale.x / 2),
				centerPos.y - (screenDisplayScale.y / 2),
				centerPos.z
			)
		);
	}

	private void UpdateShake()
	{
		if (!isShaking && shakeQueue.Count > 0)
		{
			if (activeShakeRoutine != null)
				StopCoroutine(activeShakeRoutine);
			StartCoroutine(ShakeSingleRoutine(shakeQueue[0]));
		}
	}

	private void UpdateVerticalOffset()
	{
		activeVerticalOffset = Mathf.Lerp
		(
			startOffsetY, 
			endOffsetY,
			Mathf.Clamp01(GameController.main.stackCount / (float)offsetYTargetStack)
		);
	}

	private void UpdateTrackAndZoom()
	{
		isTrackingGull = GameController.main.activeGull != null;

		if (isTrackingGull)
		{
			// Midpoint.
			Vector3 A = GameController.main.activeGull.transform.position -
				(Vector3.up * activeVerticalOffset);

			Vector3 B = GameController.main.activeAnchor.GetAnchorPos() - 
				(Vector3.up * activeVerticalOffset);

			targetTrack = A + ((B - A) / 2);

			targetZoom = Mathf.Lerp
			(
				zoomMax, 
				zoomMin, 
				1f - GameController.main.refRing.progress) + (transform.position.z - 
					GameController.main.activeGull.transform.position.z
			) * 0.1f;
		}
		else
		{
			targetTrack = GameController.main.activeAnchor.GetAnchorPos() - 
				(Vector3.up * activeVerticalOffset);

			targetZoom = Mathf.Lerp
			(
				zoomMax, 
				zoomMin, 
				1f - GameController.main.refRing.progress) + (transform.position.z -
					GameController.main.activeAnchor.GetAnchorPos().z) * 0.1f;
		}
	}

	private void Move()
	{
		// Zoom.
		float step         = Mathf.Abs((transform.position.z - targetZoom) * zoomSmoothing);
		float targetZ      = Mathf.MoveTowards(transform.position.z, targetZoom,
									   Mathf.Clamp(step, zoomMinStep, float.MaxValue));
		// Track.
		step               = Vector3.Distance(transform.position, targetTrack) * moveSmoothing;

		// Apply.
		transform.position = new Vector3
		(
			Mathf.MoveTowards
			(
				transform.position.x, 
				targetTrack.x, 
				Mathf.Clamp
				(
					step * (isTrackingGull ? 1 : trackAnchorRatio), 
					moveMinStep * (isTrackingGull ? 1 : trackAnchorRatio), 
					float.MaxValue
				)
			),
			Mathf.MoveTowards
			(
				transform.position.y, 
				targetTrack.y, 
				Mathf.Clamp
				(
					step * (isTrackingGull ? 1 : trackAnchorRatio), 
					moveMinStep * (isTrackingGull ? 1 : trackAnchorRatio), 
					float.MaxValue
				)
			),
			targetZ
		) + (Vector3)shakeOffset;
	}

	#endregion

	#region Public Methods

	public void ShakeScreen(Vector2 magnitude, float duration, int count)
	{
		float individualDuration = duration / (float)count;

		for (int i = 0; i < count; i++)
		{
			Shake newShake = new Shake
			(
				magnitude,
				individualDuration
			);

			shakeQueue.Add(newShake);
		}
	}
	public void ShakeScreen()
	{
		ShakeScreen(defaultMagnitude, defaultDuration, defaultCount);
	}
	public void ShakeScreen(Vector2 magnitude)
	{
		ShakeScreen(magnitude, defaultDuration, defaultCount);
	}
	public void ShakeScreen(Vector2 magnitude, float duration)
	{
		ShakeScreen(magnitude, duration, defaultCount);
	}
	public void ShakeScreen(Vector2 magnitude, int count)
	{
		ShakeScreen(magnitude, defaultDuration, count);
	}

	#endregion
	
	#region IEnumerators

	private IEnumerator ShakeSingleRoutine(Shake shake)
	{
		isShaking = true;

		shakeOffset = Vector2.zero;

		// Shake outwards.
		float startTime = Time.time;
		while (Time.time - startTime < shake.duration)
		{
			float t = (Time.time - startTime) / (shake.duration / 2f);
			t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

			shakeOffset = Vector2.Lerp
			(
				Vector2.zero, shake.magnitude, t
			);

			yield return 1;
		}
		shakeOffset = shake.magnitude;

		// Shake back inwards.
		startTime = Time.time;
		while (Time.time - startTime < (shake.duration / 2f))
		{
			float t = (Time.time - startTime) / (shake.duration / 2f);
			t = Mathf.Sin(t * Mathf.PI * 0.5f);

			shakeOffset = Vector2.Lerp
			(
				shake.magnitude, Vector2.zero, t
			);

			yield return 1;
		}
		shakeOffset = Vector2.zero;

		shakeQueue.RemoveAt(0);
		isShaking = false;
	}

	#endregion
}