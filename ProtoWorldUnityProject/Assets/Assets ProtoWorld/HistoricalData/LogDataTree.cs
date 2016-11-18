﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogDataTree
{
	public string Key;
	public string Value;
	private LinkedList<LogDataTree> children;

	public LogDataTree(string key, string value)
	{
		this.Key = key;
		this.Value = value;
		children = new LinkedList<LogDataTree>();
	}

	public LogDataTree AddChild(LogDataTree child)
	{
		children.AddLast (child);
		return children.Last.Value;
	}

	public LogDataTree GetChild(string childKey)
	{
		foreach (LogDataTree n in children)
			if (n.Key.Equals(childKey))
				return n;
		return null;
	}

	public LinkedList<LogDataTree> getChildren(){
		return children;
	}

}
