using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeciesFactory : MonoBehaviour
{
	// Omnivore = 0, Carnivore = 1, Herbivore = 2, these are the constants used in the database.
	// Added Plant = 3 and TreeOfLife = 4, for easier programming of collision and attack behaviors.
	public enum SpeciesType {Omnivore, Carnivore, Herbivore, Plant, TreeOfLife};


	public static SpeciesType getRandomAnimalType()
	{
		System.Array values = System.Enum.GetValues (typeof(SpeciesType));
		// limit to animal types only
		int max = values.Length - 2;
		// Omnivore = 0, Carnivore = 1, Herbivore = 2
		SpeciesType specie = (SpeciesType)(((int[])values)[Random.Range(0,max)]);
		return specie;
	}


	/**
	* food web hard coded for testing of code concepts
	* 
	* Omnivore -> Omnivore, Carnivore, Herbivore, Plant, TreeOfLife
	* Carnivore -> Omnivore, Carnivore, Herbivore
	* Herbivore -> Plant, TreeOfLife
	* Plant -> empty list
	* 
	* TODO: Lion, Buffalo, Grass and Herbs, Bush Pig
	* Tree Mouse, Cockroach, Decaying Material, 
	* Trees and Shrubs
	* 
	* **/
	public static ArrayList setSpeciesPreyHardCoded(SpeciesFactory.SpeciesType species)
	{
		ArrayList prey = new ArrayList();

		switch (species) 
		{
		case SpeciesFactory.SpeciesType.Omnivore:
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			prey.Add (SpeciesType.Plant);
			break;
		case SpeciesFactory.SpeciesType.Carnivore: 
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			break;
		case SpeciesFactory.SpeciesType.Herbivore:
			prey.Add (SpeciesType.Plant);
			break;
		case SpeciesFactory.SpeciesType.Plant:
			// do nothing return an empty list
			break;
		}

		return prey;
	}
}
