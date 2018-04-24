
/**************************************************************************************************
 *    GameController.cs   [birdUP]                                                        ver 1.2.1 
 *                                                                                       2018.04.03
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Game and player controller. Takes input, manages game state and other objects 
 *                  for scoring and state management.
 *
 *    CHANGELOG:    [1.2 - 2018.04.03]  Late routine removed.
 *                  [1.1 - 2018.03.28]  Tapping. Seagull interaction.
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
/// Game and player controller. Takes input, manages game state and other objects for scoring and 
/// state management.
/// </summary>
public class GameController : MonoBehaviour 
{
	#region Static

	public static GameController main;

	#endregion

	#region Inspector Fields

	[Header("References")][Space(3)]
	[Tooltip("A reference to the seagull prefab, to spawn.")]
	public GameObject refSeagullPrefab;

	[Tooltip("Reference to the UI ring object.")]
	public Ring refRing;

	[Tooltip("The first anchor (likely pole) to start off the game.")]
	public AnchorPoint firstAnchor;

	[Space(5)][Header("Settings")][Space(3)]
	[Tooltip("The base speed for seagulls. They get faster over time and vary, using this.")]
	public float seagullBaseSpeed;

	[Tooltip("The target amount of progression ratio to set the timing ring. Anything past is" +
		"scaled in the late routine to close the ring fully. Sets the size of the target ring.")]
	public float targetRingRatio;
	
	[Space(5)][Header("Spawner")][Space(3)]
	[Tooltip("The duration to wait before beginning to spawn seagulls, after beginning the game.")]
	public float initialDelay;

	#endregion

	#region Run-time Fields

	[HideInInspector] public List<Seagull> seagulls;
	[HideInInspector] public AnchorPoint activeAnchor;
	[HideInInspector] public Seagull activeGull;
	[HideInInspector] public Spawnpoint[] spawnpoints;
	[HideInInspector] public float lastArrivalTime = -1f;
	[HideInInspector] public float instability = 0f;
	[HideInInspector] public float progress;
	[HideInInspector] public int stackCount;
	[HideInInspector] public int breadCount;
	[HideInInspector] public bool hasActiveGull;

	private float spawnTimer;
	private bool hasInitialSpawned;

	#endregion

	#region Monobehaviours

	private void Awake()
	{
		main = this;

		activeAnchor = firstAnchor;
	}

	private void Start()
	{
		spawnpoints = FindObjectsOfType<Spawnpoint>();
		spawnTimer = initialDelay;
		stackCount = 0;
	}

	private void Update()
	{
		Spawner();

		GetInput();

		UpdateUI();

		progress = refRing.progress;
	}

	#endregion

	#region Private Methods

	private void Spawner()
	{
		if (!hasInitialSpawned)
		{
			spawnTimer -= Time.deltaTime;
			if (spawnTimer <= 0f)
			{
				SpawnGull();
				StartCoroutine(SpawnRoutine(5f));
				hasInitialSpawned = true;
			}
			return;
		}

		if (!hasActiveGull && seagulls.Count > 0)
		{
			PullGull();
		}

		if (hasActiveGull)
		{
			if (activeGull == null)
			{
				hasActiveGull = false;
				return;
			}
			else
			{
				// Track Gull.
				Vector3 targetPoint = Camera.main.WorldToScreenPoint(activeGull.transform.position);
				refRing.transform.position = targetPoint;
				refRing.refTargetRing.transform.position = targetPoint;
			}
		}
	}

	private void PullGull()
	{
		// Pull new gull.
		activeGull = seagulls[0];
		seagulls.RemoveAt(0);
		hasActiveGull = true;

		refRing.Initialize();
	}

	private void GetInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (activeGull != null)
			{
				if (!activeGull.isAtPost)
					activeGull.Tap();
				//else
				//LateTap();
			}
		}
	}

	private void UpdateUI()
	{
		HUDController.main.SetScoreText(breadCount.ToString());
	}

	#endregion

	#region Public Methods

	public void SpawnGull()
	{
		int roll = Random.Range(0, spawnpoints.Length);
		seagulls.Add
		(
			Instantiate
			(
				refSeagullPrefab,
				spawnpoints[roll].transform.position,
				Quaternion.identity,
				null
			).GetComponent<Seagull>()
		);
	}

	public void ResetGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
	}

	public void AddToInstability(float instabilityRatio)
	{
		instability += (1f - instabilityRatio);
	}

	#endregion

	#region IEnumerators

	private IEnumerator SpawnRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);

		SpawnGull();

		StartCoroutine(SpawnRoutine(Mathf.Clamp(delay * 0.99f, 2f, float.MaxValue)));
	}

	#endregion
}