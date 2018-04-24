
/**************************************************************************************************
 *    HUDController.cs   [birdUP]                                                         ver 1.0.0 
 *                                                                                       2018.04.19
 *                                               
 *    AUTHOR:       Angel Rodriguez Jr.
 *
 *    DESCRIPTION:  Controls the UI elements of the HUD, providing a static instance access to the
 *                  elements that represent the GameController's status like score.
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
/// Controls the UI elements of the HUD, providing a static instance access to the elements that 
/// represent the GameController's status like score.
/// </summary>
public class HUDController : MonoBehaviour 
{
	#region Static

	public static HUDController main;

	#endregion

	#region Inspector Fields

	[Header("References")][Space(3)]
	public Text refScoreText;

	[Space(10)][Header("Settings")][Space(3)]
	public float scoreTickSpeed;

	#endregion

	#region Monobehaviours

	private void Awake()
	{
		main = this;
	}

	#endregion

	#region Public Methods

	public void SetScoreText(string text)
	{
		refScoreText.text = text;
	}

	#endregion
}
