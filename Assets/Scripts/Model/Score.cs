using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Score
{
	public string id;
	public string username;
	public string hand;
	public double prediction;
	public double prediction_2;
	public double prediction_3;
	public double actual;

	public override string ToString()
	{
		return "Person: { " 
			+ "username: " + username 
			+ ", hand: " + hand 
			+ ", actual: " + actual 
			+ ", prediction: " + prediction 
			+ " }";
	}
}
