using System;
using UnityEngine;

public class MVGUIMenuHandler : MonoBehaviour
{
	public UXButton buildButton;

	public UXButton playButton;

	public UXTextField serverTextField;

	public UXTextField serverPortTextField;

	public UXTextField usernameTextField;

	public UXTextField passwordTextField;

	public UXTextField planetTextField;

	public UXText connState;

	public UXText gameState;

	private MVGameController gameController;

	private MVJoinState prevGameState = MVJoinState.Leaving;

	private MVConnState prevConnState;

	private void Start()
	{
		gameController = (MVGameController)(object)Object.FindObjectOfType(typeof(MVGameController));
		if ((Object)(object)gameController == (Object)null)
		{
			Debug.LogError((object)"No MVGameController found - GUI unavailable");
			return;
		}
		UXButton uXButton = buildButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(EditIslandOnClick));
		UXButton uXButton2 = playButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, new UXButton.OnClickDelegate(JoinIslandOnClick));
		serverTextField.Text = gameController.ip;
		UXTextField uXTextField = serverTextField;
		uXTextField.OnValueChanged = (UXTextField.OnValueChangedDelegate)Delegate.Combine(uXTextField.OnValueChanged, (UXTextField.OnValueChangedDelegate)((string value) =>
		{
			gameController.ip = value;
		}));
		serverPortTextField.Text = gameController.portDev.ToString();
		UXTextField uXTextField2 = serverPortTextField;
		uXTextField2.OnValueChanged = (UXTextField.OnValueChangedDelegate)Delegate.Combine(uXTextField2.OnValueChanged, (UXTextField.OnValueChangedDelegate)((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameController.portDev = result;
			}
		}));
		usernameTextField.Text = gameController.username;
		UXTextField uXTextField3 = usernameTextField;
		uXTextField3.OnValueChanged = (UXTextField.OnValueChangedDelegate)Delegate.Combine(uXTextField3.OnValueChanged, (UXTextField.OnValueChangedDelegate)((string value) =>
		{
			gameController.username = value;
		}));
		passwordTextField.Text = gameController.password;
		UXTextField uXTextField4 = passwordTextField;
		uXTextField4.OnValueChanged = (UXTextField.OnValueChangedDelegate)Delegate.Combine(uXTextField4.OnValueChanged, (UXTextField.OnValueChangedDelegate)((string value) =>
		{
			gameController.password = value;
		}));
		planetTextField.Text = gameController.planetName;
		UXTextField uXTextField5 = planetTextField;
		uXTextField5.OnValueChanged = (UXTextField.OnValueChangedDelegate)Delegate.Combine(uXTextField5.OnValueChanged, (UXTextField.OnValueChangedDelegate)((string value) =>
		{
			gameController.planetName = value;
		}));
	}

	public void Update()
	{
		if (!((Object)(object)gameController != (Object)null) || gameController.Game == null)
		{
			return;
		}
		MVConnState mVConnState = gameController.Game.ConnState;
		MVJoinState joinState = gameController.Game.JoinState;
		if (mVConnState != prevConnState)
		{
			prevConnState = mVConnState;
			connState.Text = $"Connection: {mVConnState.ToString()}";
		}
		if (joinState != prevGameState)
		{
			prevGameState = joinState;
			gameState.Text = $"Game: {joinState.ToString()}";
			if (joinState == MVJoinState.Playing)
			{
				gameState.Text = string.Empty;
				connState.Text = string.Empty;
			}
		}
	}

	public void JoinIslandOnClick()
	{
		gameController.JoinPlanet(fromWeb: false);
	}

	public void EditIslandOnClick()
	{
		gameController.EditPlanet(fromWeb: false);
	}

	public void DisconnectOnClick()
	{
		gameController.LeaveGame();
	}
}
