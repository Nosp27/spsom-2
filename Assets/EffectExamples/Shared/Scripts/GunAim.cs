﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class GunAim:MonoBehaviour
{
	public int borderLeft;
	public int borderRight;
	public int borderTop;
	public int borderBottom;

	private Camera parentCamera;
	private bool isOutOfBounds;

	void Start () 
	{
		parentCamera = GetComponentInParent<Camera>();
	}

	void Update()
	{
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		float mouseX = mousePosition.x;
		float mouseY = mousePosition.y;

		if (mouseX <= borderLeft || mouseX >= Screen.width - borderRight || mouseY <= borderBottom || mouseY >= Screen.height - borderTop) 
		{
			isOutOfBounds = true;
		} 
		else 
		{
			isOutOfBounds = false;
		}

		if (!isOutOfBounds)
		{
			transform.LookAt(parentCamera.ScreenToWorldPoint (new Vector3(mouseX, mouseY, 5.0f)));
		}
	}

	public bool GetIsOutOfBounds()
	{
		return isOutOfBounds;
	}
}

