﻿using System;
using System.Collections.Generic;
using System.Linq;
using UFZ.Interaction;
using UnityEngine;
using System.Collections;

public class VrMenuPlayable : MonoBehaviour
{
	private vrWidgetMenu _menu;

	private vrWidgetList _list;
	private vrWidgetGroup _group;
	private vrWidgetButton _beginButton;
	private vrWidgetButton _backButton;
	private vrWidgetButton _playButton;
	private vrWidgetButton _stopButton;
	private vrWidgetButton _forwardButton;
	private vrWidgetButton _endButton;
	private vrWidgetToggleButton _isPlayingCheckbox;

	private vrCommand _playableObjectChangedCommand;

	private List<IPlayable> _playables;

	// Start waits on VRMenu creation with a coroutine
	IEnumerator Start()
	{
		VRMenu middleVrMenu = null;
		while (middleVrMenu == null || middleVrMenu.menu == null)
		{
			// Wait for VRMenu to be created
			yield return null;
			middleVrMenu = FindObjectOfType(typeof(VRMenu)) as VRMenu;
		}
		
		_menu = new vrWidgetMenu("Play Menu", middleVrMenu.menu, "Play Menu");
		middleVrMenu.menu.SetChildIndex(_menu, 0);
		AddMenu(_menu);

		// End coroutine
		yield break;
	}

	private void AddMenu(vrWidgetMenu vrmenu)
	{
		var valueList = vrValue.CreateList();
		var tmp = FindObjectsOfType(typeof(IPlayable)) as IPlayable[];
		if (tmp != null)
		{
			_playables = tmp.ToList();
			_playables.RemoveAll(s => s.GetType() == typeof (TimeObjectSwitch));
			foreach (var player in _playables)
				valueList.AddListItem(player.name);
		}

		_playableObjectChangedCommand = new vrCommand("Playable object command", PlayableObjectChanged);
		_list = new vrWidgetList("Playable objects:", vrmenu, "Playable objects:", _playableObjectChangedCommand);
		_list.SetList(valueList);
		_list.SetSelectedIndex(0);

		_group = new vrWidgetGroup("Play controls", vrmenu);
		_beginButton = new vrWidgetButton("Begin", _group);
		_backButton = new vrWidgetButton("Back", _group);
		_playButton = new vrWidgetButton("Play", _group);
		//_isPlayingCheckbox = new vrWidgetToggleButton("Is Playing", vrmenu, "Is Playing:");
		_stopButton = new vrWidgetButton("Stop", _group);
		_forwardButton = new vrWidgetButton("Forward", _group);
		_endButton = new vrWidgetButton("End", _group);
	}

	vrValue PlayableObjectChanged(vrValue iValue)
	{
		var index = iValue.GetInt();
		if (index < _playables.Count - 1)
			return null;

		var player = _playables[index];

		// TODO Add remove command
		_beginButton.AddCommand(player.BeginCommand);
		_backButton.AddCommand(player.BackCommand);
		_playButton.AddCommand(player.PlayCommand);
		_stopButton.AddCommand(player.StopCommand);
		_forwardButton.AddCommand(player.ForwardCommand);
		_endButton.AddCommand(player.EndCommand);

		return null;
	}
}
