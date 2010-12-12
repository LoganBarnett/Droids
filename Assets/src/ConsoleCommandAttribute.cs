using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ConsoleCommandAttribute : Attribute
{
	string commandName;
	
	public ConsoleCommandAttribute(string commandName)
	{
		CommandName = commandName;
	}
	
	public string CommandName {
		get { return commandName ?? this.TypeId.ToString(); }
		set { commandName = value; }
	}
}
